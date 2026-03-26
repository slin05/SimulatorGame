using UnityEngine;
using UnityEngine.UI;

public enum FoodState { Raw, Cooking, Cooked, Overcooked }

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(CanvasGroup))]
public class FoodItem : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite rawSprite;
    public Sprite cookingSprite;
    public Sprite cookedSprite;
    public Sprite overcookedSprite;

    [Header("Cook Settings")]
    public float cookTime = 4f;

    // ── State ──────────────────────────────────────────────────
    public FoodState State { get; private set; } = FoodState.Raw;
    public float Timer { get; private set; } = 0f;

    public Sprite CurrentSprite => State switch
    {
        FoodState.Raw => rawSprite,
        FoodState.Cooking => cookingSprite,
        FoodState.Cooked => cookedSprite,
        FoodState.Overcooked => overcookedSprite,
        _ => rawSprite
    };

    private Stove _currentStove;

    // ──────────────────────────────────────────────────────────
    void Awake()
    {
        State = FoodState.Raw;
    }

    void Update()
    {
        // FIX 1: keep running until Overcooked, not just until Cooked
        if (State != FoodState.Cooking && State != FoodState.Cooked) return;

        Timer += Time.deltaTime;

        _currentStove?.SetProgress(Timer / cookTime);

        if (Timer >= cookTime * 2f)
        {
            SetState(FoodState.Overcooked);
        }
        else if (Timer >= cookTime)
        {
            SetState(FoodState.Cooked);
        }
    }

    public void StartCooking(Stove stove)
    {
        _currentStove = stove;
        Timer = 0f;
        SetState(FoodState.Cooking);
    }

    public void StopCooking()
    {
        _currentStove = null;
    }

    public bool IsServable() => State == FoodState.Cooked;

    void SetState(FoodState newState)
    {
        if (State == newState) return; // prevent repeat triggers
        State = newState;
        _currentStove?.SetProgress(Timer / cookTime);
    }
}