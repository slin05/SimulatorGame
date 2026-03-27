using UnityEngine;

[CreateAssetMenu(menuName = "Cooking/Recipe")]
public class Recipe : ScriptableObject
{
    public string foodName;
    public Ingredient[] requiredIngredients;
    public Ingredient[] choiceIngredients;
    public bool requiresCooking;
    public float cookTime;   // seconds (0 = no cooking needed)
    public float burnTime;   // seconds after cookTime before burnt
    public int basePrice;    // money earned on success
    public int wasteCost;    // money lost if burnt/thrown
}
