using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public void BuyFryer()
    {
        if (ScoreManager.Instance.money >= 50)
        {
            ScoreManager.Instance.DeductMoney(50);
            PlayerPrefs.SetInt("hasFryer", 1);
        }
    }
    // Add similar methods for oven, grill, tip jar
}
