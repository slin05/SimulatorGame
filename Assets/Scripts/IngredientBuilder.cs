using System.Collections.Generic;
using UnityEngine;

public class IngredientBuilder : MonoBehaviour
{
    private List<Ingredient> added = new List<Ingredient>();

    public void AddIngredient(Ingredient ing)
    {
        added.Add(ing);
    }

    public bool CheckRecipe(Recipe recipe)
    {
        foreach (var ing in recipe.requiredIngredients)
        {
            if (!added.Contains(ing)) return false;
        }
        return true;
    }

    public void Clear() => added.Clear();
}
