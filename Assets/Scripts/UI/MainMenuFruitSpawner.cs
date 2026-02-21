using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuFruitSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> fruitList;
    [SerializeField] private List<Transform> spawnLocations;
    float timer = 0f;
    // Update is called once per frame
    void Update()
    {
        if (fruitList.Count == 0) return;
        timer += Time.deltaTime;

        if (timer >= 0.8f)
        {
            timer = 0f;
            int randomIndex = Random.Range(0, fruitList.Count);
            int randomSpawn = Random.Range(0, spawnLocations.Count);
            Instantiate(fruitList[randomIndex], spawnLocations[randomSpawn].position, Quaternion.Euler(0f, 180f, 0f));
        }
    }
}
