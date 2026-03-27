using UnityEngine;

[CreateAssetMenu(fileName = "Customer", menuName = "Game/Customer")]
public class CustomerData : ScriptableObject
{
    public string customerName;
    public Sprite customerSprite;
    public float patienceDuration = 15f;
}