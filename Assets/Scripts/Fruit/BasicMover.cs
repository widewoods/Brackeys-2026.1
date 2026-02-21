using UnityEngine;
using UnityEngine.AI;

public class BasicMover : MonoBehaviour, IMover
{
    private NavMeshAgent agent;

    [Header("Idle Wander")]
    [SerializeField] private float idleRadius = 3f;
    [SerializeField] private float idleRepathInterval = 2f;
    [SerializeField] private float idleArriveDistance = 0.4f;

    private Vector3 homePos;
    private float nextIdlePickTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        homePos = transform.position;
    }

    public void Idle()
    {
        agent.isStopped = false;

        if (agent.hasPath && agent.remainingDistance > idleArriveDistance)
            return;

        if (Time.time < nextIdlePickTime)
            return;

        if (TryGetRandomNavMeshPoint(homePos, idleRadius, out var p))
        {
            agent.SetDestination(p);
            nextIdlePickTime = Time.time + idleRepathInterval;
        }
        else
        {
            nextIdlePickTime = Time.time + 0.5f;
        }
    }

    public void MoveTo(Vector3 worldPos)
    {
        agent.isStopped = false;
        agent.SetDestination(worldPos);
    }

    public void StopForAttack()
    {
        agent.isStopped = true;
    }

    private static bool TryGetRandomNavMeshPoint(Vector3 center, float radius, out Vector3 result)
    {
        for (int i = 0; i < 20; i++)
        {
            Vector2 r = Random.insideUnitCircle * radius;
            Vector3 candidate = center + new Vector3(r.x, 0f, r.y);

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = center;
        return false;
    }
}