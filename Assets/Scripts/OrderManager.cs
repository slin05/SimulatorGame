using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class OrderManager : MonoBehaviour
{
    // ── Recipes ────────────────────────────────────────────────
    public Recipe[] easyRecipes;
    public Recipe[] mediumRecipes;
    public Recipe[] hardRecipes;

    // ── Order System ───────────────────────────────────────────
    public float patienceTime = 30f;
    public List<Recipe> currentOrder = new List<Recipe>();
    private float patienceTimer;

    // ── Order Card UI ──────────────────────────────────────────
    public Transform ordersPanel;         // drag OrdersPanel here
    public GameObject orderCardPrefab;    // drag OrderCard prefab here
    private List<OrderCardUI> activeCards = new List<OrderCardUI>();

    // ──────────────────────────────────────────────────────────
    void Start()
    {
        if (GameSettings.difficulty == 0) patienceTime = 30f;
        else if (GameSettings.difficulty == 1) patienceTime = 20f;
        else if (GameSettings.difficulty == 2) patienceTime = 12f;
        else patienceTime = 8f;

        SpawnOrder();
    }

    void Update()
    {
        patienceTimer -= Time.deltaTime;
        if (patienceTimer <= 0) CustomerLeft();
    }

    public void SpawnOrder()
    {
        currentOrder.Clear();

        // pick how many dishes
        int dishCount = Random.Range(1, GameSettings.GetMaxDishesPerCustomer() + 1);
        for (int i = 0; i < dishCount; i++)
            currentOrder.Add(PickRandomRecipe());

        patienceTimer = patienceTime;

        // spawn order card UI
        if (orderCardPrefab != null && ordersPanel != null)
        {
            GameObject card = Instantiate(orderCardPrefab, ordersPanel);
            OrderCardUI cardUI = card.GetComponent<OrderCardUI>();

            // build order text showing all dishes
            string orderNames = "";
            foreach (var r in currentOrder)
                orderNames += r.foodName + "\n";

            cardUI.SetupOrder(orderNames, patienceTime);
            activeCards.Add(cardUI);
        }

        // log order to console
        string orderLog = "New order (" + dishCount + " dishes): ";
        foreach (var r in currentOrder) orderLog += r.foodName + " ";
        Debug.Log(orderLog);
    }

    public void ServeOrder(List<Recipe> served)
    {
        int totalEarned = 0;
        foreach (var r in served)
            totalEarned += r.basePrice;

        ScoreManager.Instance.AddMoney(totalEarned);

        // complete and remove first active card
        if (activeCards.Count > 0)
        {
            activeCards[0].OrderCompleted();
            activeCards.RemoveAt(0);
        }

        Debug.Log("Order served! Earned: " + totalEarned);
        SpawnOrder();
    }

    void CustomerLeft()
    {
        ScoreManager.Instance.DeductMoney(5);
        Debug.Log("Customer left!");

        // remove first active card
        if (activeCards.Count > 0)
        {
            activeCards.RemoveAt(0);
        }

        SpawnOrder();
    }

    Recipe PickRandomRecipe()
    {
        if (GameSettings.difficulty == 3)
        {
            Recipe[] all = CombineArrays(easyRecipes, mediumRecipes, hardRecipes);
            return all[Random.Range(0, all.Length)];
        }
        else if (GameSettings.difficulty == 2)
        {
            Recipe[] combined = CombineArrays(easyRecipes, mediumRecipes, hardRecipes);
            return combined[Random.Range(0, combined.Length)];
        }
        else if (GameSettings.difficulty == 1)
        {
            Recipe[] combined = CombineArrays(easyRecipes, mediumRecipes);
            return combined[Random.Range(0, combined.Length)];
        }
        else
        {
            return easyRecipes[Random.Range(0, easyRecipes.Length)];
        }
    }

    Recipe[] CombineArrays(Recipe[] a, Recipe[] b)
    {
        Recipe[] result = new Recipe[a.Length + b.Length];
        a.CopyTo(result, 0);
        b.CopyTo(result, a.Length);
        return result;
    }

    Recipe[] CombineArrays(Recipe[] a, Recipe[] b, Recipe[] c)
    {
        Recipe[] result = new Recipe[a.Length + b.Length + c.Length];
        a.CopyTo(result, 0);
        b.CopyTo(result, a.Length);
        c.CopyTo(result, a.Length + b.Length);
        return result;
    }
}