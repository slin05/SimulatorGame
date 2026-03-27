using UnityEngine;

public class FoodPickedUpNotifier : MonoBehaviour
{
    public Stove stove;

    void OnDestroy()
    {
        stove?.OnCookedFoodDestroyed();
    }
}