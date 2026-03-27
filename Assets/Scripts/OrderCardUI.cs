using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderCardUI : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public TextMeshProUGUI foodNameText;
    public Slider patienceSlider;
    public TextMeshProUGUI tipText;

    private float maxTime;
    private float currentTime;
    private bool isActive = false;
    private int tipAmount = 5;

    public void SetupOrder(string foodName, float patienceTime)
    {
        foodNameText.text = foodName;
        maxTime = patienceTime;
        currentTime = patienceTime;
        patienceSlider.maxValue = patienceTime;
        patienceSlider.value = patienceTime;
        tipText.text = "";
        isActive = true;
        tipAmount = 5;
    }

    void Update()
    {
        if (!isActive) return;

        currentTime -= Time.deltaTime;
        patienceSlider.value = currentTime;

        // change slider color based on time left
        Image fill = patienceSlider.fillRect.GetComponent<Image>();
        if (currentTime > maxTime * 0.6f)
            fill.color = Color.green;
        else if (currentTime > maxTime * 0.3f)
            fill.color = Color.yellow;
        else
            fill.color = Color.red;

        // reduce tip as time passes
        tipAmount = Mathf.Max(0, (int)(5 * (currentTime / maxTime)));

        if (currentTime <= 0)
            OrderFailed();
    }

    public void OrderCompleted()
    {
        isActive = false;
        tipText.text = "+" + tipAmount + " tip!";
        ScoreManager.Instance.AddMoney(tipAmount);
        Debug.Log("Tip earned: " + tipAmount);
        Invoke("DestroyCard", 1.5f); // show tip for 1.5s then destroy
    }

    void OrderFailed()
    {
        isActive = false;
        tipText.text = "Too slow!";
        Invoke("DestroyCard", 1.5f);
    }

    void DestroyCard()
    {
        Destroy(gameObject);
    }
}