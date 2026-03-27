using UnityEngine;
using UnityEngine.UI;

public class DrinkButton : MonoBehaviour
{
    public int drinkIndex;
    public Sprite glassFullSprite;
    public BeverageMachineDropZone machine;

    public void OnDrinkButtonPressed()
    {
        // get glass directly from machine
        GameObject glassObj = machine.GetCurrentGlass();
        if (glassObj == null)
        {
            Debug.Log("No glass at machine!");
            return;
        }
        Debug.Log("Found glass object: " + glassObj.name);

        GlassData glassData = glassObj.GetComponent<GlassData>();
        if (glassData == null)
        {
            Debug.Log("No GlassData found! " + glassObj.name);
            return;
        }

        // set drink type
        switch (drinkIndex)
        {
            case 0: glassData.drinkType = Ingredient.Water; break;
            case 1: glassData.drinkType = Ingredient.CocaCola; break;
            case 2: glassData.drinkType = Ingredient.DietCoke; break;
            case 3: glassData.drinkType = Ingredient.Fanta; break;
            case 4: glassData.drinkType = Ingredient.Milkshake; break;
        }

        glassData.isFilled = true;

        // change glass image to full
        Image glassImage = glassObj.GetComponent<Image>();
        if (glassImage != null && glassFullSprite != null)
        {
            glassImage.sprite = glassFullSprite;
            Debug.Log("Glass is now full with: " + glassData.drinkType);
        }
        else
        {
            Debug.Log("Missing image or sprite!");
        }

        machine.FillGlass(drinkIndex);
    }
}