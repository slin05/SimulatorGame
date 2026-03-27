using UnityEngine;
using UnityEngine.EventSystems;

public class ServingWindowDropZone : MonoBehaviour, IDropHandler
{
    public OrderManager orderManager;

    public void OnDrop(PointerEventData eventData)
    {
        DraggableFood glass = eventData.pointerDrag?.GetComponent<DraggableFood>();
        if (glass == null) return;

        GlassData glassData = glass.GetComponent<GlassData>();
        if (glassData == null || !glassData.isFilled)
        {
            Debug.Log("Glass is empty!");
            return;
        }

        bool orderMatches = false;
        foreach (Recipe dish in orderManager.currentOrder)
        {
            if (dish.requiredIngredients.Length == 1 &&
                dish.requiredIngredients[0] == glassData.drinkType)
            {
                orderMatches = true;
                break;
            }
        }

        if (orderMatches)
        {
            Debug.Log("Drink served correctly!");
            ScoreManager.Instance.AddMoney(3);
            Destroy(glass.gameObject);
            orderManager.SpawnOrder();
        }
        else
        {
            Debug.Log("Wrong drink!");
        }
    }
}