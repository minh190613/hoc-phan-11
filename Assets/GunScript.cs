using UnityEngine;
using System.Collections;

// CODE MỚI
using UnityEngine.UI; 
using TMPro;

public class gun : MonoBehaviour
{
    [Header("Cấu hình bắn đạn")]
    public GameObject AWM_Bullet;    // Kéo Prefab viên đạn vào đây
    public Transform firePoint;        // Vị trí đạn bay ra (nếu không có, đạn sẽ tự ra từ tâm nhân vật)
    public float bulletSpeed = 25f;    // Tốc độ bay của viên đạn

    private float nextFireTime = 0f;   // Thời điểm tiếp theo được phép bắn
    private float cooldownTime = 1f;   // Thời gian hồi chiêu (5 giây)
    public Transform camera;
    public ParticleSystem particleSystem;

    // CODE MỚI
    public TextMeshProUGUI ammoText;

    [SerializeField]
    private int ammo = 30;

    [SerializeField]
    private bool isReload = false;

    // CODE MỚI
    void Start()
    {
        UpdateAmmoUI();
    }

    IEnumerator ReloadCoroutine()
    {
        isReload = true;

        yield return new WaitForSeconds(4f);

        ammo = 30;
        isReload = false;

        // CODE MỚI
        UpdateAmmoUI();
    }

    void Reload()
    {
        StartCoroutine(ReloadCoroutine());
    }

    void Update()
    {
        if  (Input.GetMouseButtonDown(0))
        {
            if (particleSystem != null)
            {
                if (ammo > 0 && isReload == false && Time.time >= nextFireTime)
                {
                    
                    Shoot();
                    // Cập nhật thời điểm tiếp theo được bắn = thời gian hiện tại + 5 giây
                     nextFireTime = Time.time + cooldownTime;
                    particleSystem.Play();
                    ammo -= 1;
                    ammoText.text = "" + ammo + " / 9";
                }
            }
           
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }

        if (ammo == 0)
        {
            Reload();
        }
    }

    // CODE MỚI
    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            if(isReload) 
            {ammoText.text="Reload";}
            else {ammoText.text = ""+ammo + " / 30";}
        }
    }
    void Shoot()
    {
        // 1. Xác định vị trí và góc quay để tạo đạn
        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
        Quaternion spawnRotation = firePoint != null ? AWM_Bullet.transform.rotation : transform.rotation;

        // 2. Tạo bản sao của viên đạn (Spawn)
        GameObject bulletClone = Instantiate(AWM_Bullet, spawnPosition, spawnRotation);

        // 3. Tìm thành phần Rigidbody (hoặc Rigidbody2D nếu là game 2D) để đẩy đạn bay về phía trước
        Rigidbody rb = bulletClone.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // rb.velocity =  camera.transform.forward * bulletSpeed; 
            rb.linearVelocity =  Vector3.back * bulletSpeed; 
            // Lưu ý: Nếu dùng Unity phiên bản cũ hơn 2023, thay 'linearVelocity' bằng 'velocity' nhé!
        }

        // 4. Tự động xóa bản sao này sau 3 giây
        Destroy(bulletClone, 3f); 

    }
}