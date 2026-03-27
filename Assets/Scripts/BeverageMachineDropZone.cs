using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BeverageMachineDropZone : MonoBehaviour, IDropHandler
{
    [Header("Assign in Inspector")]
    public GameObject drinkButtonsPanel;
    public GameObject currentGlass; // changed to public

    public void OnDrop(PointerEventData eventData)
    {
        DraggableFood glass = eventData.pointerDrag?.GetComponent<DraggableFood>();
        if (glass == null) return;

        // snap glass to machine
        glass.transform.SetParent(transform);
        glass.transform.position = transform.position + new Vector3(0, -100, 0);
        currentGlass = glass.gameObject;

        if (drinkButtonsPanel != null)
            drinkButtonsPanel.SetActive(true);

        Debug.Log("Glass placed at machine!");
    }

    public void FillGlass(int drinkIndex)
    {
        if (currentGlass == null) return;

        if (drinkButtonsPanel != null)
            drinkButtonsPanel.SetActive(false);

        // make glass draggable again
        DraggableFood draggable = currentGlass.GetComponent<DraggableFood>();
        if (draggable != null)
            draggable.enabled = true;

        Debug.Log("Glass filled with drink index: " + drinkIndex);
    }

    public GameObject GetCurrentGlass()
    {
        return currentGlass;
    }
}