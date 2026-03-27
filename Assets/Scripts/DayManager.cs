using UnityEngine;
using UnityEngine.SceneManagement;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance;

    public float dayDuration = 300f;   // 5 minutes in seconds
    private float dayTimer;
    private bool dayActive = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        StartDay();
    }

    void Update()
    {
        if (!dayActive) return;

        dayTimer -= Time.deltaTime;
        if (dayTimer <= 0)
        {
            EndDay();
        }
    }

    public void StartDay()
    {
        GameSettings.UpdateDifficulty();
        dayTimer = dayDuration;
        dayActive = true;
        Debug.Log("Day " + GameSettings.currentDay + " started! Difficulty: " + GameSettings.difficulty);
    }

    void EndDay()
{
    dayActive = false;
    PlayerPrefs.SetInt("SavedDay", GameSettings.currentDay); // save progress
    Debug.Log("Day " + GameSettings.currentDay + " ended!");

    if (GameSettings.currentDay >= 12)
    {
        Debug.Log("Game complete!");
        SceneManager.LoadScene("EndScene");
    }
    else
    {
        GameSettings.currentDay++;
        SceneManager.LoadScene("DayEndScene");
    }
}

    // returns remaining time as a formatted string like "4:32"
    public string GetTimeRemaining()
    {
        int minutes = Mathf.FloorToInt(dayTimer / 60);
        int seconds = Mathf.FloorToInt(dayTimer % 60);
        return string.Format("{0}:{1:00}", minutes, seconds);
    }
}