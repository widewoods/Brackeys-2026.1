using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class MoveToTest : MonoBehaviour
{

    public Transform goal;
    private NavMeshAgent agent;

    [SerializeField] private FruitLineOfSight fruitLineOfSight;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (fruitLineOfSight.hasLineOfSight)
        {
            agent.destination = goal.position;
        }
        else
        {
            agent.destination = transform.position;
        }
    }
}