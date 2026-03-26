// Stove.cs — Unity 6000.4.0f1
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(Image))]
public class Stove : MonoBehaviour, IDropHandler
{
    [Header("Child UI References")]
    public Image foodDisplay;
    public Slider progressBar;
    public TMP_Text statusLabel;

    [Header("Prefabs")]
    public GameObject foodItemPrefab;

    [Header("Colours")]
    public Color overcookedTint = new Color(1f, 0.4f, 0.4f);

    // ── State ──────────────────────────────────────────────────
    public FoodItem CurrentFood { get; private set; }
    public bool IsEmpty => CurrentFood == null;

    private GameObject _foodGO;
    private CanvasGroup _foodCG;
    private bool _readyToPickUp = false;
    private Canvas _rootCanvas;

    // ──────────────────────────────────────────────────────────
    void Start()
    {
        _rootCanvas = GetComponentInParent<Canvas>();
        ShowEmpty();
    }

    // ── Drop Handler ───────────────────────────────────────────
    public void OnDrop(PointerEventData eventData)
    {
        if (!IsEmpty) return;

        var food = eventData.pointerDrag?.GetComponent<FoodItem>();
        if (food == null) return;
        if (food.State != FoodState.Raw) return;

        _foodGO = eventData.pointerDrag;
        _foodCG = _foodGO.GetComponent<CanvasGroup>();
        if (_foodCG == null) _foodCG = _foodGO.AddComponent<CanvasGroup>();

        // Hide food but keep active so Update() runs
        _foodCG.alpha = 0f;
        _foodCG.blocksRaycasts = false;
        _foodCG.interactable = false;

        _foodGO.transform.SetParent(transform);
        _foodGO.transform.localPosition = Vector3.zero;

        CurrentFood = food;
        _readyToPickUp = false;
        CurrentFood.StartCooking(this);

        foodDisplay.gameObject.SetActive(true);
        foodDisplay.transform.SetAsLastSibling();
        foodDisplay.sprite = CurrentFood.CurrentSprite;
        foodDisplay.color = Color.white;

        progressBar.gameObject.SetActive(true);
        progressBar.value = 0f;

        SetLabel("Cooking...", Color.white);
    }

    // ── Called by FoodItem every frame ─────────────────────────
    public void SetProgress(float normalised)
    {
        if (CurrentFood == null) return;

        progressBar.value = Mathf.Clamp01(normalised);
        foodDisplay.sprite = CurrentFood.CurrentSprite;

        switch (CurrentFood.State)
        {
            case FoodState.Cooking:
                foodDisplay.color = Color.white;
                SetLabel("Cooking...", Color.white);
                _readyToPickUp = false;
                break;

            case FoodState.Cooked:
                foodDisplay.color = Color.white;
                SetLabel("Done! Click to pick up", Color.green);
                if (!_readyToPickUp) MakeClickable();
                break;

            case FoodState.Overcooked:
                foodDisplay.color = overcookedTint;
                SetLabel("Burned! Click to trash", Color.red);
                if (!_readyToPickUp) MakeClickable();
                break;
        }
    }

    // FIX 2: make the foodDisplay itself clickable — much more reliable
    // than trying to re-enable the hidden food GO
    void MakeClickable()
    {
        _readyToPickUp = true;

        var btn = foodDisplay.gameObject.GetComponent<Button>();
        if (btn == null) btn = foodDisplay.gameObject.AddComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnStoveClicked);
    }

    void OnStoveClicked()
    {
        if (CurrentFood == null) return;

        if (CurrentFood.State == FoodState.Overcooked)
        {
            // Just trash it
            Destroy(_foodGO);
            CurrentFood = null;
            _foodGO = null;
            _foodCG = null;
            _readyToPickUp = false;
            ShowEmpty();
            return;
        }

        if (!CurrentFood.IsServable()) return;

        // FIX 2: store spawn position and cooked sprite BEFORE any destroy
        Vector3 spawnPos = foodDisplay.transform.position;
        Sprite cookedSprite = CurrentFood.cookedSprite;

        // Clean up tracker
        Destroy(_foodGO);
        CurrentFood = null;
        _foodGO = null;
        _foodCG = null;
        _readyToPickUp = false;

        // Hide stove display
        ShowEmpty();

        // Spawn cooked food at stove position
        if (foodItemPrefab == null)
        {
            Debug.LogError("Stove: Food Item Prefab is not assigned!");
            return;
        }

        var go = Instantiate(foodItemPrefab, _rootCanvas.transform);
        go.transform.position = spawnPos;

        // Set size
        var rt = go.GetComponent<RectTransform>();
        if (rt != null) rt.sizeDelta = new Vector2(80, 80);

        // Set cooked sprite
        var img = go.GetComponent<Image>();
        if (img != null) img.sprite = cookedSprite;

        // Set drag source
        var drag = go.GetComponent<DraggableFood>();
        if (drag != null)
        {
            drag.dragSource = DraggableFood.Source.Stove;
            drag.fromStove = this;
        }
    }

    public void TrashFood()
    {
        if (_foodGO != null) Destroy(_foodGO);
        CurrentFood = null;
        _foodGO = null;
        _foodCG = null;
        _readyToPickUp = false;
        ShowEmpty();
    }

    // ── Helpers ────────────────────────────────────────────────
    void SetLabel(string text, Color color)
    {
        if (statusLabel == null) return;
        statusLabel.gameObject.SetActive(true);
        statusLabel.text = text;
        statusLabel.color = color;
    }

    void ShowEmpty()
    {
        // Remove click listener when empty
        var btn = foodDisplay.gameObject.GetComponent<Button>();
        if (btn != null) btn.onClick.RemoveAllListeners();

        foodDisplay.gameObject.SetActive(false);
        progressBar.gameObject.SetActive(false);
        SetLabel("+", new Color(1f, 1f, 1f, 0.4f));
    }
}