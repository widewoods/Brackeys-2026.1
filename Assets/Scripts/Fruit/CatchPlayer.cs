using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchPlayer : MonoBehaviour
{
    [SerializeField] LayerMask playerMask;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera fruitCamera;
    void OnCollisionEnter(Collision other)
    {
        if (((1 << other.gameObject.layer) & playerMask) == 0) return;

        Debug.Log("Contact with player");
        HandleCollision();
    }

    void HandleCollision()
    {
        playerCamera.enabled = false;
        fruitCamera.enabled = true;
    }
}
