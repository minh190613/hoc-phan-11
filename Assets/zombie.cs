using UnityEngine;

public class zombie : MonoBehaviour
{
  public Transform player;

    public UnityEngine.AI.NavMeshAgent agent;

    private float count = 0;
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
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
}
