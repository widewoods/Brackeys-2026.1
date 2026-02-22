using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(FruitController))]
public class CatchPlayer : MonoBehaviour
{
    [SerializeField] LayerMask playerMask;
    private Camera playerCamera;
    private Camera fruitCamera;
    FruitController fruitController;

    BoxCollider[] playerCollider;
    PlayerController playerController;

    Transform playerTransform;

    private TextMeshProUGUI Grabbed;
    private TextMeshProUGUI PressSpace;
    private GameObject shoppingLsit;
    private GameObject plus;

    void Start()
    {
        playerCamera = Camera.main;
        fruitCamera = GetComponentInChildren<Camera>();
        fruitController = GetComponent<FruitController>();

        Grabbed = GameObject.Find("Grabbed").GetComponent<TextMeshProUGUI>();
        PressSpace = GameObject.Find("SpacePress").GetComponent<TextMeshProUGUI>();

        shoppingLsit = GameObject.Find("GroceryListPanel");
        plus = GameObject.Find("+");

        Grabbed.enabled = false;
        PressSpace.enabled = false;
    }

    void Update()
    {
        if (fruitController.currentState == FruitController.State.Catched)
        {
            Grabbed.enabled = true;
            PressSpace.enabled = true;

            shoppingLsit.SetActive(false);
            plus.SetActive(false);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                // PressSpace scale 커졌다 작아지기
                PressSpace.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

                FruitController.escapeCount += 1;
                Debug.Log($"Need Count: {FruitController.goalEscapeCount - FruitController.escapeCount}");

                PressSpace.text = $"Press Space!!\nX{FruitController.goalEscapeCount - FruitController.escapeCount}";
                if (FruitController.escapeCount >= FruitController.goalEscapeCount)
                {
                    HandleEscape();
                    FruitController.escapeCount = 0; // reset counter
                    FruitController.goalEscapeCount *= 2; // increase goal for next escape
                }
            } else
            {
                PressSpace.transform.localScale = Vector3.Lerp(PressSpace.transform.localScale, new Vector3(1f, 1f, 1f), Time.deltaTime * 5f);
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (((1 << other.gameObject.layer) & playerMask) == 0) return;
        if (fruitController.currentState != FruitController.State.Chase) return;
        playerCollider = other.gameObject.GetComponentsInChildren<BoxCollider>();
        playerController = other.gameObject.GetComponent<PlayerController>();
        playerTransform = other.gameObject.transform;

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
        foreach (var collider in playerCollider)
        {
            collider.enabled = false;
        }
        playerCamera.enabled = false;
        playerController.IsCatched = true;
        fruitCamera.enabled = true;

        playerTransform.position = PlayerSpawn.SpawnTransform.position;
        // playerTransform.rotation = PlayerSpawn.SpawnTransform.rotation;
    }

    void HandleEscape()
    {
        Debug.Log("Player escaped");

        Grabbed.enabled = false;
        PressSpace.enabled = false;

        shoppingLsit.SetActive(true);
        plus.SetActive(true);

        fruitController.currentState = FruitController.State.ReturnHome;
        foreach (var collider in playerCollider)
        {
            collider.enabled = true;
        }
        playerCamera.enabled = true;
        playerController.IsCatched = false;
        fruitCamera.enabled = false;
    }
}
