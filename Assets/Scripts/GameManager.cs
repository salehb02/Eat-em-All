using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Money MoneyPrefab;
    public GameObject NextLevelPanel;

    private GameManagerPresentor _presentor;
    private CameraFollow _camera;
    private int _currentMoney;
    private int _initFoodsCount;
    private Player _player;

    public int SpeedUpgrade { get; private set; }
    public int CapacityUpgrade { get; private set; }
    public int SizeUpgrade { get; private set; }

    public const string MONEY_PREFS = "PLAYER_MONEY";
    public const string LEVEL_PREFS = "CURRENT_LEVEL";
    public const string SPEED_PREFS = "PLAYER_SPEED";
    public const string CAPACITY_PREFS = "PLAYER_CAPACITY";
    public const string SIZE_PREFS = "PLAYER_SIZE";

    private void Awake()
    {
        if (PlayerPrefs.HasKey(LEVEL_PREFS) && PlayerPrefs.GetInt(LEVEL_PREFS) != SceneManager.GetActiveScene().buildIndex)
        {
            if (ControlPanel.Instance.LoadLastLevel)
                SceneManager.LoadScene(PlayerPrefs.GetInt(LEVEL_PREFS));
        }
    }

    private void Start()
    {
        _camera = FindObjectOfType<CameraFollow>();
        _presentor = GetComponent<GameManagerPresentor>();
        _player = FindObjectOfType<Player>();

        _currentMoney = PlayerPrefs.GetInt(MONEY_PREFS);
        SpeedUpgrade = PlayerPrefs.GetInt(SPEED_PREFS);
        CapacityUpgrade = PlayerPrefs.GetInt(CAPACITY_PREFS);
        SizeUpgrade = PlayerPrefs.GetInt(SIZE_PREFS);

        UpdateUI();
        NextLevelPanel.gameObject.SetActive(false);
        _initFoodsCount = FindObjectsOfType<Food>().Length;

        _presentor.SetCurrentLevelText(SceneManager.GetActiveScene().buildIndex + 1);
        _presentor.SetLevelProgressSlider(0);
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
        PlayerPrefs.SetInt(MONEY_PREFS, _currentMoney);
        UpdateUI();
    }

    public void UseMoney(int amount)
    {
        _currentMoney -= amount;
        PlayerPrefs.SetInt(MONEY_PREFS, _currentMoney);
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
        {
            _presentor.SetSpeedUpgradeIntractable(false);
            _presentor.SetSizeUpgradePrice("Maxed");
        }
        else
        {
            var price = GetFinalPrice(ControlPanel.Instance.SpeedUpgrades[SpeedUpgrade + 1].Price);

            if (GetMoney() >= price)
            {
                _presentor.SetSpeedUpgradeIntractable(true);
                _presentor.SetSizeUpgradePrice($"{price.ToString("#,#")}");
            }
            else
            {
                _presentor.SetSpeedUpgradeIntractable(false);
                _presentor.SetSizeUpgradePrice("Not enough");
            }
        }

        // Capacity upgrade price text
        if (CapacityUpgrade == ControlPanel.Instance.CapacityUpgrades.Length - 1)
        {
            _presentor.SetCapacityUpgradeIntractable(false);
            _presentor.SetCapacityUpgradePrice("Maxed");
        }
        else
        {
            var price = GetFinalPrice(ControlPanel.Instance.CapacityUpgrades[CapacityUpgrade + 1].Price);

            if (GetMoney() >= price)
            {
            _presentor.SetCapacityUpgradeIntractable(true);
                _presentor.SetCapacityUpgradePrice($"{price.ToString("#,#")}");
            }
            else
            {
                _presentor.SetCapacityUpgradePrice("Not enough");
            _presentor.SetCapacityUpgradeIntractable(false);
            }
        }

        // Size upgrade price text
        if (SizeUpgrade == ControlPanel.Instance.SizeUpgrades.Length - 1)
        {
            _presentor.SetSizeUpgradeIntractable(false);
        _presentor.SetSizeUpgradePrice("Maxed");
        }
        else
        {
            var price = GetFinalPrice(ControlPanel.Instance.SizeUpgrades[SizeUpgrade + 1].Price);

            if (GetMoney() >= price)
            {
                _presentor.SetSizeUpgradeIntractable(true);
                _presentor.SetSizeUpgradePrice($"{price.ToString("#,#")}");
            }
            else
            {
                _presentor.SetSizeUpgradePrice("Not enough");
                _presentor.SetSizeUpgradeIntractable(false);
            }
        }

        _presentor.SetSpeedCurrentLevel(SpeedUpgrade + 1);
        _presentor.SetCapacityCurrentLevel(CapacityUpgrade + 1);
        _presentor.SetSizeCurrentLevel(SizeUpgrade + 1);
    }

    public void UpgradeSpeed()
    {
        if (SpeedUpgrade >= ControlPanel.Instance.SpeedUpgrades.Length - 1)
            return;

        var price = GetFinalPrice(ControlPanel.Instance.SpeedUpgrades[SpeedUpgrade + 1].Price);

        if (GetMoney() < price)
            return;

        UseMoney(price);
        SpeedUpgrade++;
        UpdateUI();
        PlayerPrefs.SetInt(SPEED_PREFS, SpeedUpgrade);
    }

    public void UpgradeCapacity()
    {
        if (CapacityUpgrade >= ControlPanel.Instance.CapacityUpgrades.Length - 1)
            return;

        var price = GetFinalPrice(ControlPanel.Instance.CapacityUpgrades[CapacityUpgrade + 1].Price);

        if (GetMoney() < price)
            return;

        UseMoney(price);
        CapacityUpgrade++;
        UpdateUI();
        PlayerPrefs.SetInt(CAPACITY_PREFS, CapacityUpgrade);
    }

    public void UpgradeSize()
    {
        if (SizeUpgrade >= ControlPanel.Instance.SizeUpgrades.Length - 1)
            return;

        var price = GetFinalPrice(ControlPanel.Instance.SizeUpgrades[SizeUpgrade + 1].Price);

        if (GetMoney() < price)
            return;

        UseMoney(price);
        SizeUpgrade++;
        UpdateUI();
        PlayerPrefs.SetInt(SIZE_PREFS, SizeUpgrade);
    }

    private int GetFinalPrice(int price)
    {
        return System.Convert.ToInt32(price * Mathf.Clamp(ControlPanel.Instance.PriceMultiplierPerLevel * SceneManager.GetActiveScene().buildIndex, 1, Mathf.Infinity));
    }

    public void CheckEndLevel(int eatenFoodsCount)
    {
        _presentor.SetLevelProgressSlider(Mathf.Lerp(1, 0, (_initFoodsCount * 0.9f - eatenFoodsCount) / (_initFoodsCount * 0.9f)));

        if (_initFoodsCount - eatenFoodsCount >= _initFoodsCount * 0.1f)
            return;

        if (PlayerPrefs.GetInt(LEVEL_PREFS) < SceneManager.sceneCountInBuildSettings - 1)
            PlayerPrefs.SetInt(LEVEL_PREFS, SceneManager.GetActiveScene().buildIndex + 1);
        else
            PlayerPrefs.SetInt(LEVEL_PREFS, Random.Range(0, SceneManager.sceneCountInBuildSettings));

        NextLevelPanel.SetActive(true);
        _player.ActivateNextLevelArrow(NextLevelPanel.transform.position);
    }

    public void NextLevel()
    {
        PlayerPrefs.DeleteKey(SIZE_PREFS);
        PlayerPrefs.DeleteKey(SPEED_PREFS);
        PlayerPrefs.DeleteKey(CAPACITY_PREFS);
        SceneManager.LoadScene(PlayerPrefs.GetInt(LEVEL_PREFS));
    }
}