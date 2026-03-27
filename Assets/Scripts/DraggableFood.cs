using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(CanvasGroup))]
public class DraggableFood : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    // ── Drag Source ────────────────────────────────────────────
    // Set this before dragging so drop targets know where it came from
    public enum Source { RawPile, Stove, Plate }

    [HideInInspector] public Source dragSource;
    [HideInInspector] public Stove fromStove;    // set if dragSource == Stove
    [HideInInspector] public int plateIndex;   // set if dragSource == Plate

    // ── Private ────────────────────────────────────────────────
    private Transform _originalParent;
    private Vector3 _originalPosition;
    private CanvasGroup _canvasGroup;
    private Canvas _rootCanvas;

    // ──────────────────────────────────────────────────────────
    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        // Walk up to find the root Canvas
        _rootCanvas = GetComponentInParent<Canvas>()?.rootCanvas;
    }

    // ── IBeginDragHandler ──────────────────────────────────────
    public void OnBeginDrag(PointerEventData eventData)
    {
        _originalParent = transform.parent;
        _originalPosition = transform.position;

        // Re-parent to root canvas so it renders on top of everything
        transform.SetParent(_rootCanvas.transform, worldPositionStays: true);
        transform.SetAsLastSibling();

        // Disable raycasts so the drop target underneath can receive OnDrop
        _canvasGroup.blocksRaycasts = false;
    }

    // ── IDragHandler ───────────────────────────────────────────
    public void OnDrag(PointerEventData eventData)
    {
        // Unity 6: RectTransformUtility handles the canvas scale correctly
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                _rootCanvas.GetComponent<RectTransform>(),
                eventData.position,
                eventData.pressEventCamera,
                out Vector3 worldPoint))
        {
            transform.position = worldPoint;
        }
    }

    // ── IEndDragHandler ────────────────────────────────────────
    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;

        // If we're still parented to root canvas, no valid drop happened → snap back
        if (transform.parent == _rootCanvas.transform)
        {
            transform.SetParent(_originalParent, worldPositionStays: true);
            transform.position = _originalPosition;
        }
    }
}