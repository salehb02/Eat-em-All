using UnityEngine;
using TMPro;

public class GameManagerPresentor : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI FoodCapacityText;
    [SerializeField] private TextMeshProUGUI MoneyText;

    public void SetMoneyText(int value) => MoneyText.text = $"${(value > 0 ? value.ToString("#,#") : value.ToString())}";
    public void SetFoodCapacityText(int currentAmount, int maxCapacity) => FoodCapacityText.text = $"{currentAmount}/{maxCapacity}";
}