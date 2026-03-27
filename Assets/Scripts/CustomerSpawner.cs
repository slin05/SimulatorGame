using UnityEngine;
using System.Collections.Generic;

public class CustomerSpawner : MonoBehaviour
{
    public static CustomerSpawner instance;

    [Header("Setup")]
    public Canvas rootCanvas;
    public GameObject customerPrefab;
    public CustomerData[] customerPool;
    public string[] availableFoods = { "Chicken" };
    public RectTransform spawnPoint;       // off-screen left
    public RectTransform[] waitPositions;    // spots in front of counter

    [Header("Difficulty (Easy=1 Medium=2 Hard=3 Extreme=4)")]
    [Range(1, 4)]
    public int difficulty = 1;

    [Header("Spawn Timing")]
    public float firstSpawnDelay = 2f;
    public float spawnInterval = 8f;

    // ── Runtime ───────────────────────────────────────────────
    private List<Customer> _activeCustomers = new List<Customer>();

    void Awake() { instance = this; }

    void Start()
    {
        InvokeRepeating(nameof(TrySpawn), firstSpawnDelay, spawnInterval);
    }

    void TrySpawn()
    {
        // Clean up destroyed customers
        _activeCustomers.RemoveAll(c => c == null);

        if (_activeCustomers.Count >= difficulty) return;
        if (customerPool == null || customerPool.Length == 0) return;

        RectTransform waitPos = GetFreeWaitPosition();
        if (waitPos == null) return;

        var data = customerPool[Random.Range(0, customerPool.Length)];
        var food = availableFoods[Random.Range(0, availableFoods.Length)];

        // Spawn as child of the Canvas
        var go = Instantiate(customerPrefab, rootCanvas.transform);
        go.transform.SetAsLastSibling();
        var rt = (RectTransform)go.transform;
        rt.anchoredPosition = spawnPoint.anchoredPosition;

        var customer = go.GetComponent<Customer>();
        customer.Init(data, food, waitPos.anchoredPosition);
        _activeCustomers.Add(customer);
    }

    RectTransform GetFreeWaitPosition()
    {
        foreach (var pos in waitPositions)
        {
            bool occupied = false;
            foreach (var c in _activeCustomers)
            {
                if (c == null) continue;
                var rt = (RectTransform)c.transform;
                if (Vector2.Distance(rt.anchoredPosition, pos.anchoredPosition) < 50f)
                {
                    occupied = true;
                    break;
                }
            }
            if (!occupied) return pos;
        }
        return null;
    }

    // Called by GameManager when day advances
    public void StopSpawning()
    {
        CancelInvoke(nameof(TrySpawn));
    }

    public void StartSpawning()
    {
        CancelInvoke(nameof(TrySpawn));
        InvokeRepeating(nameof(TrySpawn), firstSpawnDelay, spawnInterval);
    }

    public void SetDifficulty(int d)
    {
        difficulty = Mathf.Clamp(d, 1, 4);
    }
}