using System.Collections.Generic;
using UnityEngine;

public class PersistentInventory : MonoBehaviour
{
    public static PersistentInventory Instance { get; private set; }

    [Min(1)]
    public int capacity = 4;

    [Header("Debug")]
    public bool debugLogs = true;

    private readonly List<GameObject> storedItems =
        new List<GameObject>();

    public int Count => storedItems.Count;

    private void Awake()
    {
        // If another scene contains another PersistentInventory,
        // keep the original and remove the duplicate.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // This object and its children now survive scene changes.
        DontDestroyOnLoad(gameObject);

        if (debugLogs)
            Debug.Log("PersistentInventory initialized.");
    }

    public bool Store(GameObject item)
    {
        if (item == null)
            return false;

        if (storedItems.Count >= capacity)
        {
            if (debugLogs)
                Debug.Log("Persistent inventory is full.");

            return false;
        }

        // Parent the item underneath the persistent object BEFORE
        // disabling it. That makes it survive scene unloading.
        item.transform.SetParent(transform, true);

        storedItems.Add(item);

        if (debugLogs)
        {
            Debug.Log(
                $"PersistentInventory stored {item.name}. " +
                $"Count={storedItems.Count}"
            );
        }

        item.SetActive(false);

        return true;
    }

    public GameObject TakeLast()
    {
        if (storedItems.Count == 0)
            return null;

        int index = storedItems.Count - 1;

        GameObject item = storedItems[index];

        storedItems.RemoveAt(index);

        if (item != null)
        {
            // It is no longer owned by the persistent inventory.
            item.transform.SetParent(null, true);
        }

        if (debugLogs)
        {
            Debug.Log(
                $"PersistentInventory retrieved " +
                $"{(item != null ? item.name : "NULL")}. " +
                $"Count={storedItems.Count}"
            );
        }

        return item;
    }
}