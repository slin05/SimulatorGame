using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class RawChickenPile : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    public GameObject foodItemPrefab; 
    public Canvas rootCanvas;      

    // ── Runtime ────────────────────────────────────────────────
    private GameObject _spawnedFood;   // the food being dragged right now
    private CanvasGroup _spawnedCG;
    private RectTransform _spawnedRT;

    // ──────────────────────────────────────────────────────────

    // When player starts dragging the bowl, spawn a new food item
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (foodItemPrefab == null)
        {
            Debug.LogError("RawChickenPile: Food Item Prefab not assigned!");
            return;
        }

        // Spawn at the bowl's position
        _spawnedFood = Instantiate(foodItemPrefab, rootCanvas.transform);
        _spawnedFood.transform.position = transform.position;

        _spawnedRT = _spawnedFood.GetComponent<RectTransform>();
        if (_spawnedRT != null) _spawnedRT.sizeDelta = new Vector2(80, 80);

        // Set drag source so stove knows it's raw
        var drag = _spawnedFood.GetComponent<DraggableFood>();
        if (drag != null)
            drag.dragSource = DraggableFood.Source.RawPile;

        // Make sure it has a CanvasGroup for raycasts
        _spawnedCG = _spawnedFood.GetComponent<CanvasGroup>();
        if (_spawnedCG == null)
            _spawnedCG = _spawnedFood.AddComponent<CanvasGroup>();

        // Forward the drag event to the spawned food's DraggableFood
        var draggable = _spawnedFood.GetComponent<DraggableFood>();
        if (draggable != null)
        {
            // Manually trigger OnBeginDrag on the spawned food
            ExecuteEvents.Execute<IBeginDragHandler>(
                _spawnedFood, eventData, (handler, data) =>
                handler.OnBeginDrag((PointerEventData)data));
        }
    }

    // While dragging, forward drag to the spawned food
    public void OnDrag(PointerEventData eventData)
    {
        if (_spawnedFood == null) return;

        ExecuteEvents.Execute<IDragHandler>(
            _spawnedFood, eventData, (handler, data) =>
            handler.OnDrag((PointerEventData)data));
    }

    // When released, forward end drag to the spawned food
    public void OnEndDrag(PointerEventData eventData)
    {
        if (_spawnedFood == null) return;

        ExecuteEvents.Execute<IEndDragHandler>(
            _spawnedFood, eventData, (handler, data) =>
            handler.OnEndDrag((PointerEventData)data));

        _spawnedFood = null;
        _spawnedCG = null;
        _spawnedRT = null;
    }
}