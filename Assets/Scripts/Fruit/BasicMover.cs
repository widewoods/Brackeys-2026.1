using UnityEngine;
using UnityEngine.AI;

public class BasicMover : MonoBehaviour, IMover
{

    public Transform goal;
    private NavMeshAgent agent;

    [SerializeField] private FruitLineOfSight fruitLineOfSight;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void Idle()
    {
        agent.isStopped = true;
    }

    public void MoveTo(Vector3 worldPos)
    {
        agent.isStopped = false;
        agent.destination = worldPos;
    }

    public void StopForAttack()
    {
        throw new System.NotImplementedException();
    }
}