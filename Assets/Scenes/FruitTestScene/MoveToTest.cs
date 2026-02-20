using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class MoveToTest : MonoBehaviour
{

    public Transform goal;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.destination = goal.position;
    }

    void Update()
    {
        agent.destination = goal.position;
    }
}