using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrabbedFruit : MonoBehaviour
{
    private Rigidbody fruitRigidbody;
    private Collider fruitCollider;

    void Start()
    {
        fruitRigidbody = GetComponent<Rigidbody>();     
        fruitCollider = GetComponent<Collider>();
    }

    void Update()
    {
        // 과일 계속 한 방향으로 회전
        transform.Rotate(Vector3.up, 100f * Time.deltaTime);   
    }

    void OnCollisionEnter(Collision collision)
    {
        // 0.3초 기다리기
        StartCoroutine(FreezeFruitAfterDelay(0.3f));
    }

    private IEnumerator FreezeFruitAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // 0.3초 후에 freezeAll로 고정
        fruitRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        // fruitCollider.enabled = false;
        gameObject.layer = 8;
    }
}
