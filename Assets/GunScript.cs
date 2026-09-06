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
            if (particleSystem != null || true)
            {
                if (ammo > 0 && isReload == false && Time.time >= nextFireTime)
                {
                    
                    Shoot();
                    // Cập nhật thời điểm tiếp theo được bắn = thời gian hiện tại + 5 giây
                     nextFireTime = Time.time + cooldownTime;
                    // particleSystem.Play();
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
        // 1. Dùng trực tiếp vị trí và hướng world của firePoint
        Vector3 spawnPosition = firePoint.position;
        Quaternion spawnRotation = firePoint != null ? firePoint.rotation : transform.rotation;

        // 2. Tạo bản sao của viên đạn (Spawn)
        GameObject bulletClone = Instantiate(AWM_Bullet, spawnPosition, spawnRotation);

        // 3. Tìm thành phần Rigidbody (hoặc Rigidbody2D nếu là game 2D) để đẩy đạn bay về phía trước
        Rigidbody rb = bulletClone.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 shootingDirection = firePoint != null ? firePoint.forward : transform.forward;
            rb.linearVelocity = shootingDirection * bulletSpeed;
        }

        // 4. Tự động xóa bản sao này sau 3 giây
        Destroy(bulletClone, 3f); 

    }
}