using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckEndCondition : MonoBehaviour
{
    public static CheckEndCondition Instance;
    public int grabbedCount;
    public int totalCount;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        grabbedCount = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.CompareTag("Player"))
        {
            if (grabbedCount >= totalCount)
            {
                SceneManager.LoadScene(3); // Clear Scene
            }
        }
        else if (other.CompareTag("Fruit"))
        {
            if (other.GetComponent<FruitController>().currentState == FruitController.State.Catched)
            {
                SceneManager.LoadScene(0); // Game over
            }
        }
    }
}
