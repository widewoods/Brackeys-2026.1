using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatermelonAttacker : MonoBehaviour, IAttacker
{

    private float attackTimer = 3f;
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private float force = 5f;
    private FruitController brain;

    void Start()
    {
        brain = GetComponent<FruitController>();
    }

    public void Attack(Transform target)
    {
        brain.enabled = false;
        Vector3 direction = target.position - transform.position;
        GetComponent<Rigidbody>().velocity = direction * force;
    }

    public bool CanAttack()
    {
        return true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) return;
        brain.enabled = true;
    }
}
