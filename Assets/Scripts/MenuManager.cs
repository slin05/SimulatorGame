using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        GameSettings.currentDay = 1;        // always start from day 1
        GameSettings.UpdateDifficulty();    // sets difficulty to Easy automatically
        SceneManager.LoadScene("GameSceneManager");
    }
}