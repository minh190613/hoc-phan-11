using UnityEngine;

public class zombie : MonoBehaviour
{
    public Transform player;
    public float maxHealth = 10f;
    public float damagePerHit = 3f;
    public float hitCooldown = 0.2f;
    public Vector3 healthBarOffset = new Vector3(0f, 2.2f, 0f);

    private float currentHealth;
    private Animator animator;
    public UnityEngine.AI.NavMeshAgent agent;
    private float count = 0f;
    private float nextDamageTime = 0f;
    private Transform healthBarRoot;
    private Transform healthBarFill;

    public float CurrentHealth => currentHealth;
    public float HealthPercent => maxHealth > 0f ? Mathf.Clamp01(currentHealth / maxHealth) : 0f;
    public bool IsDead => currentHealth <= 0f;

    void Start()
    {
        if (tag == "Untagged")
            tag = "Zombie";

        currentHealth = maxHealth;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = GetComponent<Animator>();
        CreateHealthBar();
    }

    void Update()
    {
        UpdateHealthBar();

        if (player == null) return;

        if (count > 0.2f)
        {
            agent.SetDestination(player.position);
            count = 0f;
        }

        count += Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        ProcessHit(other);
    }

    void OnTriggerStay(Collider other)
    {
        if (Time.time < nextDamageTime)
            return;

        ProcessHit(other);
    }

    void OnCollisionEnter(Collision collision)
    {
        ProcessHit(collision.collider);
    }

    private Material CreateSolidMaterial(Color color)
    {
        Shader shader = Shader.Find("Unlit/Color");
        if (shader == null)
            shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null)
            shader = Shader.Find("Sprites/Default");
        if (shader == null)
            shader = Shader.Find("Standard");

        Material material = new Material(shader != null ? shader : Shader.Find("Standard"));
        material.SetColor("_Color", color);
        material.color = color;
        return material;
    }

    private void CreateHealthBar()
    {
        GameObject root = GameObject.CreatePrimitive(PrimitiveType.Cube);
        root.name = "ZombieHealthBar";
        root.transform.SetParent(transform);
        root.transform.localPosition = healthBarOffset;
        root.transform.localScale = new Vector3(1.6f, 0.18f, 0.08f);
        root.transform.localRotation = Quaternion.identity;

        Renderer rootRenderer = root.GetComponent<Renderer>();
        rootRenderer.material = CreateSolidMaterial(new Color(0.2f, 0.2f, 0.2f, 1f));
        Destroy(root.GetComponent<BoxCollider>());

        GameObject fill = GameObject.CreatePrimitive(PrimitiveType.Cube);
        fill.name = "HealthFill";
        fill.transform.SetParent(root.transform);
        fill.transform.localPosition = new Vector3(-0.5f, 0f, 0.06f);
        fill.transform.localScale = new Vector3(1f, 0.7f, 0.5f);
        fill.transform.localRotation = Quaternion.identity;

        Renderer fillRenderer = fill.GetComponent<Renderer>();
        fillRenderer.material = CreateSolidMaterial(Color.green);
        Destroy(fill.GetComponent<BoxCollider>());

        healthBarRoot = root.transform;
        healthBarFill = fill.transform;
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (healthBarRoot == null)
            return;

        healthBarRoot.position = transform.position + healthBarOffset;

        if (Camera.main != null)
        {
            Vector3 lookDirection = healthBarRoot.position - Camera.main.transform.position;
            healthBarRoot.rotation = Quaternion.LookRotation(lookDirection);
            healthBarRoot.Rotate(0f, 180f, 0f);
        }

        if (healthBarFill != null)
        {
            float ratio = HealthPercent;
            healthBarFill.localScale = new Vector3(Mathf.Clamp(ratio, 0f, 1f), 0.7f, 0.5f);
            healthBarFill.localPosition = new Vector3(-0.5f + ratio * 0.5f, 0f, 0.06f);

            Renderer fillRenderer = healthBarFill.GetComponent<Renderer>();
            if (fillRenderer != null)
            {
                Color lowHealthColor = Color.red;
                Color fullHealthColor = Color.green;
                Color currentColor = Color.Lerp(lowHealthColor, fullHealthColor, ratio);
                fillRenderer.material = CreateSolidMaterial(currentColor);
            }
        }

        healthBarRoot.gameObject.SetActive(!IsDead);
    }

    private void ProcessHit(Collider other)
    {
        if (other == null || IsDead)
            return;

        if (other.gameObject == gameObject)
            return;

        bool isBullet = IsBulletObject(other.gameObject);
        if (!isBullet)
            return;

        nextDamageTime = Time.time + hitCooldown;
        TakeDamage(damagePerHit);

        if (other.gameObject != null)
        {
            Destroy(other.gameObject);
        }
    }

    private bool IsBulletObject(GameObject obj)
    {
        if (obj == null)
            return false;

        string[] bulletTags = { "Bullet", "RifleBullet", "riflebullet", "Projectile", "WeaponBullet" };
        for (int i = 0; i < bulletTags.Length; i++)
        {
            if (obj.CompareTag(bulletTags[i]))
                return true;
        }

        if (obj.GetComponent<BulletScript>() != null)
            return true;
        if (obj.GetComponentInParent<BulletScript>() != null)
            return true;
        if (obj.GetComponentInChildren<BulletScript>() != null)
            return true;

        if (obj.name.ToLower().Contains("bullet") || obj.name.ToLower().Contains("rifle"))
            return true;

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (rb.GetComponent<BulletScript>() != null)
                return true;
            if (rb.name.ToLower().Contains("bullet") || rb.name.ToLower().Contains("rifle"))
                return true;
        }

        return false;
    }

    public void TakeDamage(float damage)
    {
        if (IsDead)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - damage);

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            if (animator != null)
                animator.SetTrigger("Die");

            if (healthBarRoot != null)
                healthBarRoot.gameObject.SetActive(false);

            Destroy(gameObject, 1f);
            return;
        }

        if (animator != null)
            animator.SetTrigger("Hit");
    }
}
