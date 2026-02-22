using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    [SerializeField] private FruitSpawns[] fruitSpawns;
    [SerializeField] private int spawnCount;

    // Counts by type
    private readonly Dictionary<FruitType, int> counts = new();

    // UI can subscribe to this
    public event Action<IReadOnlyDictionary<FruitType, int>> OnSpawnedSummary;

    void Start()
    {
        SpawnRandom();
        OnSpawnedSummary?.Invoke(counts);
        CheckEndCondition.Instance.totalCount = spawnCount;
    }

    void SpawnRandom()
    {
        counts.Clear();

        for (int i = 0; i < spawnCount; i++)
        {
            int index = UnityEngine.Random.Range(0, fruitSpawns.Length);
            FruitSpawns chosen = fruitSpawns[index];

            GameObject fruit = Instantiate(chosen.prefab, chosen.home.position, Quaternion.identity);

            var fc = fruit.GetComponent<FruitController>();
            fc.home = chosen.home;

            if (fc.type != null)
            {
                counts.TryGetValue(fc.type, out int c);
                counts[fc.type] = c + 1;
                fruit.name = fc.type.displayName;
            }
            else
            {
                Debug.LogWarning($"Fruit prefab '{chosen.prefab.name}' has no FruitType assigned.");
            }
        }
    }
}

[Serializable]
public class FruitSpawns
{
    public Transform home;
    public GameObject prefab;
}