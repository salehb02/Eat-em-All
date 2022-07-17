using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Money MoneyPrefab;

    private GameManagerPresentor _presentor;
    private CameraFollow _camera;
    private int _currentMoney;

    public int SpeedUpgrade { get; private set; }
    public int CapacityUpgrade { get; private set; }
    public int SizeUpgrade { get; private set; }

    private void Start()
    {
        _camera = FindObjectOfType<CameraFollow>();
        _presentor = GetComponent<GameManagerPresentor>();

        UpdateUI();
    }

    private void Update()
    {
        var camOffset = ControlPanel.Instance.SizeUpgrades[SizeUpgrade].CameraOffset;
        _camera.offset = Vector3.Lerp(_camera.offset, new Vector3(0, camOffset, -camOffset), Time.deltaTime * 5f);
    }

    public void SpawnMoney(int moneyValue, Transform point)
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

        // Speed upgrade price text
        if (SpeedUpgrade == ControlPanel.Instance.SpeedUpgrades.Length - 1)
            _presentor.SetSizeUpgradePrice("Maxed");
        else
        {
            if (GetMoney() >= ControlPanel.Instance.SpeedUpgrades[SpeedUpgrade + 1].Price)
            {
                _presentor.SetSizeUpgradePrice($"${ControlPanel.Instance.SpeedUpgrades[SpeedUpgrade + 1].Price.ToString("#,#")}");
            }
            else
            {
                _presentor.SetSizeUpgradePrice("Not enough $");
            }
        }

        // Capacity upgrade price text
        if (CapacityUpgrade == ControlPanel.Instance.CapacityUpgrades.Length - 1)
            _presentor.SetCapacityUpgradePrice("Maxed");
        else
        {
            if (GetMoney() >= ControlPanel.Instance.CapacityUpgrades[CapacityUpgrade + 1].Price)
            {
                _presentor.SetCapacityUpgradePrice($"${ControlPanel.Instance.CapacityUpgrades[CapacityUpgrade + 1].Price.ToString("#,#")}");
            }
            else
            {
                _presentor.SetCapacityUpgradePrice("Not enough $");
            }
        }

        // Size upgrade price text
        if (SizeUpgrade == ControlPanel.Instance.SizeUpgrades.Length - 1)
            _presentor.SetSizeUpgradePrice("Maxed");
        else
        {
            if (GetMoney() >= ControlPanel.Instance.SizeUpgrades[SizeUpgrade + 1].Price)
            {
                _presentor.SetSizeUpgradePrice($"${ControlPanel.Instance.SizeUpgrades[SizeUpgrade + 1].Price.ToString("#,#")}");
            }
            else
            {
                _presentor.SetSizeUpgradePrice("Not enough $");
            }
        }
    }

    public void UpgradeSpeed()
    {
        if (SpeedUpgrade >= ControlPanel.Instance.SpeedUpgrades.Length - 1)
            return;

        if (GetMoney() < ControlPanel.Instance.SpeedUpgrades[SpeedUpgrade + 1].Price)
            return;

        UseMoney(ControlPanel.Instance.SpeedUpgrades[SpeedUpgrade + 1].Price);
        SpeedUpgrade++;
        UpdateUI();
    }

    public void UpgradeCapacity()
    {
        if (CapacityUpgrade >= ControlPanel.Instance.CapacityUpgrades.Length - 1)
            return;

        if (GetMoney() < ControlPanel.Instance.CapacityUpgrades[CapacityUpgrade + 1].Price)
            return;

        UseMoney(ControlPanel.Instance.CapacityUpgrades[CapacityUpgrade + 1].Price);
        CapacityUpgrade++;
        UpdateUI();
    }

    public void UpgradeSize()
    {
        if (SizeUpgrade >= ControlPanel.Instance.SizeUpgrades.Length - 1)
            return;

        if (GetMoney() < ControlPanel.Instance.SizeUpgrades[SizeUpgrade + 1].Price)
            return;

        UseMoney(ControlPanel.Instance.SizeUpgrades[SizeUpgrade + 1].Price);
        SizeUpgrade++;
        UpdateUI();
    }
}