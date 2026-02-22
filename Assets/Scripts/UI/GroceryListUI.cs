using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GroceryListUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FruitSpawner spawner;
    [SerializeField] private Transform contentParent;
    [SerializeField] private GroceryRowUI rowPrefab;

    [Header("Sort")]
    [SerializeField] private bool sortByName = true;

    void OnEnable()
    {
        if (spawner != null)
            spawner.OnSpawnedSummary += Rebuild;
    }

    void OnDisable()
    {
        if (spawner != null)
            spawner.OnSpawnedSummary -= Rebuild;
    }

    void Rebuild(IReadOnlyDictionary<FruitType, int> counts)
    {
        // Clear old rows
        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);

        // Copy & sort
        var list = new List<KeyValuePair<FruitType, int>>(counts);

        if (sortByName)
            list.Sort((a, b) => string.Compare(a.Key.displayName, b.Key.displayName, System.StringComparison.Ordinal));
        else
            list.Sort((a, b) => b.Value.CompareTo(a.Value)); // sort by count desc

        // Spawn rows
        foreach (var kv in list)
        {
            var type = kv.Key;
            int count = kv.Value;

            var row = Instantiate(rowPrefab, contentParent);
            row.Set(type.icon, type.displayName, count);
        }
    }
}