using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    [Header("Assign in Inspector")]
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI timerText;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Update()
    {
        // update coins
        if (ScoreManager.Instance != null)
            coinsText.text = "Coins: " + ScoreManager.Instance.money;

        // update day
        dayText.text = "Day " + GameSettings.currentDay;

        // update timer
        if (DayManager.Instance != null)
            timerText.text = DayManager.Instance.GetTimeRemaining();
    }

    public void ShowTip(int amount)
    {
        // flash tip text -- handled by OrderCardUI
        Debug.Log("Tip earned: " + amount);
    }
}