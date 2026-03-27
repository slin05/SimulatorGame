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

    private GameObject _foodGO;          // hidden FoodItem tracker
    private CanvasGroup _foodCG;
    private GameObject _spawnedCookedGO; // visible draggable food
    private Canvas _rootCanvas;
    private bool _cookedFoodSpawned;

    // ──────────────────────────────────────────────────────────
    void Start()
    {
        _rootCanvas = GetComponentInParent<Canvas>();
        ShowEmpty();
    }

    // ── Accept raw food drop ───────────────────────────────────
    public void OnDrop(PointerEventData eventData)
    {
        if (!IsEmpty) return;

        var food = eventData.pointerDrag?.GetComponent<FoodItem>();
        if (food == null || food.State != FoodState.Raw) return;

        _foodGO = eventData.pointerDrag;
        _foodCG = _foodGO.GetComponent<CanvasGroup>() ?? _foodGO.AddComponent<CanvasGroup>();
        _foodCG.alpha = 0f;
        _foodCG.blocksRaycasts = false;
        _foodCG.interactable = false;

        _foodGO.transform.SetParent(transform);
        _foodGO.transform.localPosition = Vector3.zero;

        CurrentFood = food;
        _cookedFoodSpawned = false;
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

        float display = CurrentFood.State == FoodState.Cooking
            ? Mathf.Clamp01(normalised)
            : Mathf.Clamp01(normalised - 1f);

        progressBar.value = display;
        foodDisplay.sprite = CurrentFood.CurrentSprite;

        switch (CurrentFood.State)
        {
            case FoodState.Cooking:
                foodDisplay.color = Color.white;
                SetLabel("Cooking...", Color.white);
                break;

            case FoodState.Cooked:
                foodDisplay.color = Color.white;
                SetLabel("Drag to serve!", Color.green);
                if (!_cookedFoodSpawned) SpawnCookedFood();
                break;

            case FoodState.Overcooked:
                foodDisplay.color = overcookedTint;
                SetLabel("Burned! Click to trash", Color.red);
                // Update sprite to overcooked
                foodDisplay.sprite = CurrentFood.CurrentSprite;
                // Destroy floating cooked food if player hadn't picked it up
                if (_spawnedCookedGO != null)
                {
                    var notifier = _spawnedCookedGO.GetComponent<FoodPickedUpNotifier>();
                    if (notifier != null) notifier.stove = null;
                    Destroy(_spawnedCookedGO);
                    _spawnedCookedGO = null;
                }
                MakeClickable();
                break;
        }
    }

    // ── Spawn draggable cooked food ────────────────────────────
    void SpawnCookedFood()
    {
        _cookedFoodSpawned = true;
        if (foodItemPrefab == null) return;

        Sprite cookedSprite = CurrentFood.cookedSprite;

        var go = Instantiate(foodItemPrefab, _rootCanvas.transform);
        go.transform.position = foodDisplay.transform.position;
        go.transform.SetAsLastSibling();

        var rt = go.GetComponent<RectTransform>();
        if (rt != null) rt.sizeDelta = new Vector2(80, 80);

        var img = go.GetComponent<Image>();
        if (img != null) img.sprite = cookedSprite;

        var drag = go.GetComponent<DraggableFood>();
        if (drag != null)
        {
            drag.dragSource = DraggableFood.Source.Stove;
            drag.fromStove = this;
        }

        // Remove FoodItem so it can't be re-dropped on a stove as raw
        var fi = go.GetComponent<FoodItem>();
        if (fi != null) Destroy(fi);

        // Notify stove when this food is destroyed (served or end of day)
        var notifier = go.AddComponent<FoodPickedUpNotifier>();
        notifier.stove = this;

        _spawnedCookedGO = go;
    }

    void MakeClickable()
    {
        var btn = foodDisplay.gameObject.GetComponent<Button>();
        if (btn == null) btn = foodDisplay.gameObject.AddComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnStoveClicked);
    }

    void OnStoveClicked()
    {
        GameManager.Instance?.OnFoodBurned();
        TrashFood();
    }

    // Called by FoodPickedUpNotifier.OnDestroy — food was served or cleaned up
    public void OnCookedFoodDestroyed()
    {
        _spawnedCookedGO = null;
        ClearStove();
    }

    // ── Clear everything ───────────────────────────────────────
    public void TrashFood()
    {
        if (_spawnedCookedGO != null)
        {
            // Disconnect notifier so OnDestroy doesn't trigger ClearStove twice
            var notifier = _spawnedCookedGO.GetComponent<FoodPickedUpNotifier>();
            if (notifier != null) notifier.stove = null;
            Destroy(_spawnedCookedGO);
        }
        ClearStove();
    }

    void ClearStove()
    {
        if (_foodGO != null) Destroy(_foodGO);
        CurrentFood = null;
        _foodGO = null;
        _foodCG = null;
        _spawnedCookedGO = null;
        _cookedFoodSpawned = false;
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
        var btn = foodDisplay.gameObject.GetComponent<Button>();
        if (btn != null) btn.onClick.RemoveAllListeners();
        foodDisplay.gameObject.SetActive(false);
        progressBar.gameObject.SetActive(false);
        SetLabel("+", new Color(1f, 1f, 1f, 0.4f));
    }
}