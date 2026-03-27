using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// ── One card ──────────────────────────────────────────────────────────────
public class UpgradeCardUI : MonoBehaviour
{
    // Track purchased upgrades across days
    private static HashSet<string> _purchased = new HashSet<string>();
    private static int _stovesPurchased = 0;
    private static int MaxExtraStoves => StoveManager.instance != null
        ? StoveManager.instance.stoves.Length - StoveManager.instance.startingStoves
        : 3;

    public static void ResetPurchases()
    {
        _purchased.Clear();
        _stovesPurchased = 0;
    }

    [Header("Card Data")]
    public string upgradeName;
    [TextArea] public string description;
    public int cost;
    public int requiredDay = 1;

    [Header("Card UI (child objects)")]
    public TMP_Text nameLabel;
    public TMP_Text descLabel;
    public TMP_Text costLabel;
    public Button buyButton;

    void Start()
    {
        if (nameLabel != null) nameLabel.text = upgradeName;
        if (descLabel != null) descLabel.text = description;
        if (costLabel != null) costLabel.text = $"${cost}";
        if (buyButton != null) buyButton.onClick.AddListener(OnBuyClicked);

        RefreshState();
    }

    void OnEnable()
    {
        // Extra Stove can be bought up to 3 times
        if (upgradeName == "Extra Stove")
        {
            if (_stovesPurchased >= MaxExtraStoves)
            {
                gameObject.SetActive(false);
                return;
            }
            // Update label to show how many left
            if (nameLabel != null)
                nameLabel.text = $"Extra Stove ({MaxExtraStoves - _stovesPurchased} left)";
            RefreshState();
            return;
        }

        // All other upgrades are one time only
        if (_purchased.Contains(upgradeName))
        {
            gameObject.SetActive(false);
            return;
        }
        RefreshState();
    }

    void RefreshState()
    {
        if (buyButton == null) return;

        bool dayOk = GameManager.Instance == null || GameManager.Instance.CurrentDay >= requiredDay;
        bool moneyOk = GameManager.Instance == null || GameManager.Instance.Money >= cost;
        buyButton.interactable = dayOk && moneyOk;
    }

    void OnBuyClicked()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.Money < cost) return;
        if (GameManager.Instance.CurrentDay < requiredDay) return;

        GameManager.Instance.AddMoney(-cost);

        if (upgradeName == "Extra Stove")
        {
            _stovesPurchased++;
            ApplyUpgrade();
            // If max reached, hide — otherwise stay visible for next purchase
            if (_stovesPurchased >= MaxExtraStoves)
                gameObject.SetActive(false);
            else
            {
                if (nameLabel != null)
                    nameLabel.text = $"Extra Stove ({MaxExtraStoves - _stovesPurchased} left)";
                RefreshState();
            }
            return;
        }

        _purchased.Add(upgradeName);
        ApplyUpgrade();

        // Disable card so it can't be bought twice
        if (buyButton != null) buyButton.interactable = false;
        gameObject.SetActive(false);
    }

    void ApplyUpgrade()
    {
        switch (upgradeName)
        {
            case "No Burn Penalty":
                if (GameManager.Instance != null)
                    GameManager.Instance.burnPenalty = 0;
                break;

            case "Turbo Burner":
                StoveManager.instance?.ApplyCookSpeedUpgrade(0.7f);
                break;

            case "Extra Stove":
                StoveManager.instance?.ActivateNextStove();
                break;

            default:
                Debug.LogWarning($"UpgradeCardUI: unknown upgrade '{upgradeName}'");
                break;
        }

        Debug.Log($"[Upgrade] Purchased: {upgradeName}");
    }
}