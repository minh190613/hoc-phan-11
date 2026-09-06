using UnityEngine;

public class zombie : MonoBehaviour
{
  public Transform player;
  private float health = 10f;
  private Animator animator;

    public UnityEngine.AI.NavMeshAgent agent;

    private float count = 0;
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (player == null) return;
        if(count > 0.2f)
        {
            agent.SetDestination(player.position);
            count = 0f;
        }
        count += Time.deltaTime;
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            health -= 3f;
            if(health <= 0)
            {
                animator.SetTrigger("Die");
                // Destroy(gameObject);

                return;
            }

            animator.SetTrigger("Hit");
        }
    }
}
