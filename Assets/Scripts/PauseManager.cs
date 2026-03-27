using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseManager : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject pausePanel;
    public TextMeshProUGUI pauseButtonText;

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        pausePanel.SetActive(isPaused);
        pauseButtonText.text = isPaused ? "Resume" : "Pause";
        Debug.Log(isPaused ? "Game Paused" : "Game Resumed");
    }
}