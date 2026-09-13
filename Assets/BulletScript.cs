using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float damage = 3f;
    public float lifeTime = 3f;

    private void Awake()
    {
        if (tag == "Untagged")
            tag = "Bullet";

        if (GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        if (GetComponent<Collider>() == null)
        {
            SphereCollider col = gameObject.AddComponent<SphereCollider>();
            col.isTrigger = false;
        }
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        zombie z = other.GetComponent<zombie>();
        if (z == null)
            z = other.GetComponentInParent<zombie>();

        if (z != null)
        {
            z.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        zombie z = collision.collider.GetComponent<zombie>();
        if (z == null)
            z = collision.collider.GetComponentInParent<zombie>();

        if (z != null)
        {
            z.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
