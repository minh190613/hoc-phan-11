using UnityEngine;
using System.Collections;
using TMPro;

public class gun : MonoBehaviour
{
    [Header("Cấu hình bắn đạn")]
    public GameObject AWM_Bullet;
    public Transform firePoint;
    public float bulletSpeed = 25f;
    public float cooldownTime = 0.15f;
    public ParticleSystem particleSystem;
    public GameObject muzzleFlashPrefab;
    public float muzzleFlashTime = 0.05f;

    public TextMeshProUGUI ammoText;

    [SerializeField] private int ammo = 30;
    [SerializeField] private bool isReload = false;
    private float nextFireTime = 0f;

    void Start()
    {
        UpdateAmmoUI();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (ammo > 0 && !isReload && Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + cooldownTime;
                PlayMuzzleEffect();
                ammo--;
                if (ammoText != null)
                    ammoText.text = ammo + " / 30";
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(ReloadCoroutine());
        }

        if (ammo == 0 && !isReload)
        {
            StartCoroutine(ReloadCoroutine());
        }
    }

    IEnumerator ReloadCoroutine()
    {
        isReload = true;
        if (ammoText != null)
            ammoText.text = "Reload";

        yield return new WaitForSeconds(1.5f);

        ammo = 30;
        isReload = false;
        UpdateAmmoUI();
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
            ammoText.text = ammo + " / 30";
    }

    void PlayMuzzleEffect()
    {
        if (particleSystem != null)
        {
            particleSystem.Play();
            return;
        }

        if (muzzleFlashPrefab != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab,
                firePoint != null ? firePoint.position : transform.position,
                firePoint != null ? firePoint.rotation : transform.rotation);
            Destroy(flash, muzzleFlashTime);
            return;
        }

        GameObject tempFlash = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        tempFlash.name = "MuzzleFlash";
        tempFlash.transform.position = firePoint != null ? firePoint.position : transform.position;
        tempFlash.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
        tempFlash.transform.SetParent(transform);

        Renderer renderer = tempFlash.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = new Material(Shader.Find("Unlit/Color"));
            renderer.material.color = Color.yellow;
        }

        if (tempFlash.GetComponent<Collider>() != null)
            Destroy(tempFlash.GetComponent<Collider>());

        Destroy(tempFlash, muzzleFlashTime);
    }

    void Shoot()
    {
        if (AWM_Bullet == null)
            return;

        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
        Quaternion spawnRotation = firePoint != null ? firePoint.rotation : transform.rotation;

        GameObject bulletClone = Instantiate(AWM_Bullet, spawnPosition, spawnRotation);

        Rigidbody rb = bulletClone.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = firePoint != null ? firePoint.forward : transform.forward;
            rb.linearVelocity = direction * bulletSpeed;
            rb.useGravity = false;
        }

        Destroy(bulletClone, 3f);
    }
}