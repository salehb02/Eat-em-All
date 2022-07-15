using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameManagerPresentor _presentor;
    private int _currentMoney;

    private void Start()
    {
        _presentor = GetComponent<GameManagerPresentor>();
        UpdateUI();
    }

    public void AddMoney(int amount)
    {
        _currentMoney += amount;
        UpdateUI();
    }

    public void UpdateFoodCapacity(int currentAmount, int maxCapacity)
    {
        _presentor.SetFoodCapacityText(currentAmount, maxCapacity);
    }

    private void UpdateUI()
    {
        _presentor.SetMoneyText(_currentMoney);
    }
}