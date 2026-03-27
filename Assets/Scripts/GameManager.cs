using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{

    // ── Singleton ──────────────────────────────────────────────
    public static GameManager Instance { get; private set; }

    // ── Inspector ──────────────────────────────────────────────
    [Header("Day Settings")]
    public float dayDuration = 90f;

    [Header("Economy")]
    public int startingMoney = 0;
    public int burnPenalty = 5;

    [Header("Header UI")]
    public TMP_Text dayText;
    public TMP_Text timerText;
    public TMP_Text moneyText;
    public Image coinIcon;

    [Header("Screens")]
    public GameObject gameplayScreen;
    public GameObject endOfDayScreen;
    public GameObject loseScreen;

    [Header("End-of-Day Summary")]
    public TMP_Text summaryDayText;
    public TMP_Text summaryMoneyText;
    public TMP_Text summaryServedText;
    public TMP_Text summaryBurnedText;
    public TMP_Text summaryBonusText;
    public Button nextDayButton;

    [Header("Lose Screen")]
    public TMP_Text loseDayText;
    public TMP_Text loseReasonText;
    public Button restartButton;

    [Header("Warnings")]
    public GameObject wrongOrderWarning;
    public GameObject burntFoodWarning;

    // ── Runtime ────────────────────────────────────────────────
    public int CurrentDay { get; private set; } = 1;
    public int Money { get; private set; }
    public int ServedCount { get; private set; }
    public int BurnedCount { get; private set; }

    private float _timer;
    private bool _dayActive;

    // ──────────────────────────────────────────────────────────
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        Money = startingMoney;

        if (nextDayButton != null) nextDayButton.onClick.AddListener(OnNextDayClicked);
        if (restartButton != null) restartButton.onClick.AddListener(OnRestartClicked);

        StartDay();
    }

    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.lKey.wasPressedThisFrame)
            ShowLoseScreen("Test lose");

        if (!_dayActive) return;

        _timer -= Time.deltaTime;
        UpdateTimerUI();

        if (_timer <= 0f)
        {
            _timer = 0f;
            EndDay();
        }
    }

    // ── Day Flow ───────────────────────────────────────────────
    void StartDay()
    {
        ServedCount = 0;
        BurnedCount = 0;
        _timer = dayDuration;
        _dayActive = true;

        if (endOfDayScreen != null) endOfDayScreen.SetActive(false);
        if (loseScreen != null) loseScreen.SetActive(false);

        UpdateDayUI();
        UpdateMoneyUI();
        UpdateTimerUI();
    }

    void EndDay()
    {
        _dayActive = false;

        // Stop customers spawning
        CustomerSpawner.instance?.StopSpawning();

        // Trash anything still on stoves
        if (StoveManager.instance != null)
        {
            foreach (var stove in StoveManager.instance.stoves)
            {
                if (stove != null && stove.CurrentFood != null)
                    stove.TrashFood();
            }
        }

        // Destroy any cooked food still floating in the scene
        foreach (var drag in FindObjectsByType<DraggableFood>(FindObjectsSortMode.None))
        {
            if (drag.dragSource == DraggableFood.Source.Stove)
                Destroy(drag.gameObject);
        }

        // Lose condition: negative money at end of day
        if (Money < 0)
        {
            ShowLoseScreen("You went into debt!");
            return;
        }

        ShowEndOfDayScreen();
    }

    void ShowEndOfDayScreen()
    {
        if (endOfDayScreen != null) endOfDayScreen.SetActive(true);

        int bonus = ServedCount >= 5 ? 20 : 0;   // simple streak bonus

        if (summaryDayText != null) summaryDayText.text = $"Day {CurrentDay} Complete!";
        if (summaryServedText != null) summaryServedText.text = $"Served: {ServedCount}";
        if (summaryBurnedText != null) summaryBurnedText.text = $"Burned: {BurnedCount}";
        if (summaryBonusText != null) summaryBonusText.text = bonus > 0 ? $"Bonus: +${bonus}" : "";
        if (summaryMoneyText != null) summaryMoneyText.text = $"Total: ${Money + bonus}";

        AddMoney(bonus);
    }

    void ShowLoseScreen(string reason)
    {
        if (loseScreen != null) loseScreen.SetActive(true);

        if (loseDayText != null) loseDayText.text = $"Survived {CurrentDay - 1} days";
        if (loseReasonText != null) loseReasonText.text = reason;
    }

    // ── Button Callbacks ───────────────────────────────────────
    void OnNextDayClicked()
    {
        CurrentDay++;
        if (StoveManager.instance != null)
            StoveManager.instance.OnDayStart();
        if (CustomerSpawner.instance != null)
        {
            CustomerSpawner.instance.SetDifficulty(CurrentDay);
            CustomerSpawner.instance.StartSpawning();
        }
        StartDay();
    }

    void OnRestartClicked()
    {
        UpgradeCardUI.ResetPurchases();
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    // ── Public API ─────────────────────────────────────────────
    public void OnFoodServed()
    {
        ServedCount++;
        UpdateMoneyUI();
    }

    public void OnFoodBurned()
    {
        BurnedCount++;
        AddMoney(-burnPenalty);
        StartCoroutine(ShowWarning(burntFoodWarning));
    }

    public void TriggerWrongOrder() => StartCoroutine(ShowWarning(wrongOrderWarning));

    System.Collections.IEnumerator ShowWarning(GameObject warning)
    {
        if (warning == null) yield break;
        warning.SetActive(true);
        yield return new WaitForSeconds(2f);
        warning.SetActive(false);
    }

    public void AddMoney(int amount)
    {
        Money += amount;
        UpdateMoneyUI();
    }

    // ── UI Helpers ─────────────────────────────────────────────
    void UpdateDayUI()
    {
        if (dayText != null) dayText.text = $"Day {CurrentDay}";
    }

    void UpdateTimerUI()
    {
        if (timerText == null) return;
        int secs = Mathf.CeilToInt(_timer);
        timerText.text = $"{secs / 60:D2}:{secs % 60:D2}";
    }

    void UpdateMoneyUI()
    {
        if (moneyText != null) moneyText.text = $"${Money}";
    }
}