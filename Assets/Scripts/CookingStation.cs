using UnityEngine;

public class CookingStation : MonoBehaviour
{
    public Recipe currentRecipe;
    private float timer = 0f;
    private bool isCooking = false;

    void Update()
    {
        if (!isCooking) return;
        timer += Time.deltaTime;
        if (timer >= currentRecipe.cookTime + currentRecipe.burnTime)
        {
            BurnFood();
        }
    }

    public void StartCooking(Recipe recipe)
    {
        if (!recipe.requiresCooking) return;
        currentRecipe = recipe;
        timer = 0f;
        isCooking = true;
    }

    public bool IsCooked()
    {
        return isCooking && timer >= currentRecipe.cookTime;
    }

    void BurnFood()
    {
        isCooking = false;
        ScoreManager.Instance.DeductMoney(currentRecipe.wasteCost);
        Debug.Log("Food burnt! Cost: " + currentRecipe.wasteCost);
        // ScoreManager will go here later
    }

    void OnMouseDown()
    {
        if (!isCooking && currentRecipe != null)
        {
            StartCooking(currentRecipe);
        }
    }
}