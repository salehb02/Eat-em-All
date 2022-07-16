using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Money MoneyPrefab;

    private GameManagerPresentor _presentor;
    private int _currentMoney;

    private void Start()
    {
        _presentor = GetComponent<GameManagerPresentor>();
        UpdateUI();
    }

    public void SpawnMoney(int moneyValue,Transform point)
    {
        var money = Instantiate(MoneyPrefab, point.position, point.rotation, null);
        money.SetValue(moneyValue);
    }

    public void AddMoney(int amount)
    {
        _currentMoney += amount;
        UpdateUI();
    }

    public void UseMoney(int amount)
    {
        _currentMoney -= amount;
        UpdateUI();
    }

    public int GetMoney() => _currentMoney;

    public void UpdateFoodCapacity(int currentAmount, int maxCapacity)
    {
        _presentor.SetFoodCapacityText(currentAmount, maxCapacity);
    }

    private void UpdateUI()
    {
        _presentor.SetMoneyText(_currentMoney);
    }
}