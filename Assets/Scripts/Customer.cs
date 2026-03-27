using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class Customer : MonoBehaviour, IDropHandler
{
    [Header("References")]
    public Image spriteImage;
    public Slider patienceBar;
    public TMP_Text orderLabel;
    public TMP_Text nameLabel;

    [Header("Reward")]
    public int serveReward = 10;

    [Header("Walk Settings")]
    public float walkSpeed = 400f;

    // ── Runtime ───────────────────────────────────────────────
    private float _patience;
    private float _timer;
    private bool _waiting;
    private CustomerData _data;
    private string _orderedFood;

    public string OrderedFood => _orderedFood;
    public bool IsWaiting => _waiting;

    // ─────────────────────────────────────────────────────────
    public void Init(CustomerData data, string foodName, Vector2 waitPos)
    {
        _data = data;
        _orderedFood = foodName;
        _patience = data.patienceDuration;

        if (spriteImage != null)
        {
            spriteImage.sprite = data.customerSprite;
            spriteImage.color = Color.white;
            spriteImage.raycastTarget = true;
            Debug.Log($"[Customer] Sprite set: {data.customerSprite?.name}, size: {spriteImage.rectTransform.sizeDelta}");
        }
        else Debug.LogError("[Customer] spriteImage is NULL");

        if (nameLabel != null) nameLabel.text = data.customerName;
        if (orderLabel != null) orderLabel.text = foodName;
        if (patienceBar != null) patienceBar.value = 1f;

        var rt = (RectTransform)transform;
        Debug.Log($"[Customer] Root size: {rt.sizeDelta}, anchoredPos: {rt.anchoredPosition}");

        StartCoroutine(WalkTo(waitPos, StartWaiting));
    }

    void Update()
    {
        if (!_waiting) return;

        _timer += Time.deltaTime;
        float normalised = 1f - (_timer / _patience);

        if (patienceBar != null)
        {
            patienceBar.value = Mathf.Clamp01(normalised);

            // Fill colour: green → yellow → red
            var fill = patienceBar.fillRect?.GetComponent<Image>();
            if (fill != null)
                fill.color = Color.Lerp(Color.red, Color.green, normalised);
        }

        if (_timer >= _patience)
            LeaveAngry();
    }

    // Called when cooked food is dropped onto this customer
    public void OnDrop(PointerEventData eventData)
    {
        if (!_waiting) return;

        var drag = eventData.pointerDrag?.GetComponent<DraggableFood>();
        if (drag == null) return;

        // Only accept food coming from the stove
        if (drag.dragSource != DraggableFood.Source.Stove) return;

        // Check the food name matches the order
        var img = eventData.pointerDrag.GetComponent<UnityEngine.UI.Image>();
        // Accept the serve — destroy the food and send customer away happy
        Destroy(eventData.pointerDrag);
        Serve();
    }

    // Called by serving logic when correct food is delivered
    public void Serve()
    {
        _waiting = false;
        GameManager.Instance?.AddMoney(serveReward);
        GameManager.Instance?.OnFoodServed();
        StartCoroutine(WalkTo(new Vector2(-1200f, ((RectTransform)transform).anchoredPosition.y),
            () => Destroy(gameObject)));
    }

    void StartWaiting() => _waiting = true;

    void LeaveAngry()
    {
        _waiting = false;
        StartCoroutine(WalkTo(new Vector2(-1200f, ((RectTransform)transform).anchoredPosition.y),
            () => Destroy(gameObject)));
    }

    IEnumerator WalkTo(Vector2 target, System.Action onArrival = null)
    {
        var rt = (RectTransform)transform;
        while (Vector2.Distance(rt.anchoredPosition, target) > 5f)
        {
            rt.anchoredPosition = Vector2.MoveTowards(
                rt.anchoredPosition, target, walkSpeed * Time.deltaTime);
            yield return null;
        }
        rt.anchoredPosition = target;
        onArrival?.Invoke();
    }
}