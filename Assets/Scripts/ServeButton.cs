using UnityEngine;

public class ServeButton : MonoBehaviour
{
    public IngredientBuilder ingredientBuilder;
    public OrderManager orderManager;

    public void OnServePressed()
    {
        // check every dish in the order
        bool allCorrect = true;
        foreach (Recipe dish in orderManager.currentOrder)
        {
            if (!ingredientBuilder.CheckRecipe(dish))
            {
                allCorrect = false;
                break;
            }
        }

        if (allCorrect)
        {
            Debug.Log("Order correct! Serving...");
            orderManager.ServeOrder(orderManager.currentOrder);
            ingredientBuilder.Clear();
        }
        else
        {
            Debug.Log("Wrong ingredients! Try again.");
        }
    }
}