using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FruitController))]
public class CatchPlayer : MonoBehaviour
{
    [SerializeField] LayerMask playerMask;
    private Camera playerCamera;
    private Camera fruitCamera;
    FruitController fruitController;

    BoxCollider playerCollider;

    void Start()
    {
        playerCamera = Camera.main;
        fruitCamera = GetComponentInChildren<Camera>();
        fruitController = GetComponent<FruitController>();
    }

    void Update()
    {
        if (fruitController.currentState == FruitController.State.Catched)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                FruitController.escapeCount += 1;
                Debug.Log($"Need Count: {FruitController.goalEscapeCount-FruitController.escapeCount}");
                if (FruitController.escapeCount >= FruitController.goalEscapeCount)
                {
                    HandleEscape();
                    FruitController.escapeCount = 0; // reset counter
                    FruitController.goalEscapeCount *= 2; // increase goal for next escape
                }
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (((1 << other.gameObject.layer) & playerMask) == 0) return;
        playerCollider = other.gameObject.GetComponent<BoxCollider>();
        
        HandleCollision();
    }

    void HandleCollision()
    {
        if (fruitController.currentState == FruitController.State.Catched)
        {
            return;
        }
        
        Debug.Log("Contact with player");
        fruitController.currentState = FruitController.State.Catched;
        playerCollider.enabled = false;
        playerCamera.enabled = false;
        fruitCamera.enabled = true;
    }

    void HandleEscape()
    {
        Debug.Log("Player escaped");
        fruitController.currentState = FruitController.State.ReturnHome;
        playerCollider.enabled = true;
        playerCamera.enabled = true;
        fruitCamera.enabled = false;
    }
}
