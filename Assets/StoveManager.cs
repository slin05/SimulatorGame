// Controls how many stoves are active and applies cook time upgrades.

using UnityEngine;

public class StoveManager : MonoBehaviour
{
    // ── Singleton ──────────────────────────────────────────────
    public static StoveManager instance;

    // ── Inspector ──────────────────────────────────────────────
    [Header("Assign all stoves in order (left to right)")]
    public Stove[] stoves;

    [Header("How many stoves start active on Day 1")]
    [Range(1, 4)]
    public int startingStoves = 2;

    // ── Runtime ────────────────────────────────────────────────
    private int _activeCount;
    private float _cookMultiplier = 1f;   // < 1 = faster cooking

    // ──────────────────────────────────────────────────────────
    void Awake()
    {
        // Unity 6 singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        _activeCount = startingStoves;
        RefreshStoves();
    }

    // ── Public API ─────────────────────────────────────────────

    /// Called by the Extra Stove upgrade — activates the next stove in the array.
    /// </summary>
    public void ActivateNextStove()
    {
        if (_activeCount >= stoves.Length)
        {
            Debug.Log("StoveManager: all stoves already active.");
            return;
        }
        _activeCount++;
        RefreshStoves();
    }

    /// Called by the Turbo Burner upgrade.
    /// Pass 0.7f to make cooking 30% faster.
    /// Stacks multiplicatively — calling twice gives 0.7 × 0.7 = 0.49 (51% faster).
    /// </summary>
    public void ApplyCookSpeedUpgrade(float multiplier)
    {
        _cookMultiplier *= multiplier;

        // Apply to all existing FoodItems currently on stoves
        foreach (var stove in stoves)
        {
            if (stove.CurrentFood != null)
                stove.CurrentFood.cookTime *= multiplier;
        }

        Debug.Log($"StoveManager: cook multiplier is now {_cookMultiplier:F2}");
    }

    /// Returns the effective cook time after upgrades.
    /// FoodItem.cs uses its own cookTime field — call this when spawning new food
    /// to pre-apply the upgrade.
    /// </summary>
    public float GetEffectiveCookTime(float baseCookTime)
    {
        return baseCookTime * _cookMultiplier;
    }

    // ── Private ────────────────────────────────────────────────
    void RefreshStoves()
    {
        for (int i = 0; i < stoves.Length; i++)
        {
            bool shouldBeActive = i < _activeCount;
            stoves[i].gameObject.SetActive(shouldBeActive);
        }
    }
}