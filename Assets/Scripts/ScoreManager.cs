using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public int money = 100;
    public int highScore = 0;

    void Awake()
    {
        Instance = this;
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    public void AddMoney(int amount)
    {
        money += amount;
        if (money > highScore)
        {
            highScore = money;
            PlayerPrefs.SetInt("HighScore", highScore);
        }
    }

    public void DeductMoney(int amount)
    {
        money -= amount;
        if (money < 0) money = 0;
        // trigger game over if money == 0
    }
}