using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManagerPresentor : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI FoodCapacityText;
    [SerializeField] private TextMeshProUGUI MoneyText;

    [Header("Upgrade Panel")]
    public GameObject UpgradePanel;
    public Button UpgradeSpeed;
    public Button UpgradeCapacity;
    public Button UpgradeSize;
    public TextMeshProUGUI UpgradeSpeedPrice;
    public TextMeshProUGUI UpgradeCapacityPrice;
    public TextMeshProUGUI UpgradeSizePrice;

    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = GetComponent<GameManager>();

        UpgradeSize.onClick.AddListener(_gameManager.UpgradeSize);
        UpgradeCapacity.onClick.AddListener(_gameManager.UpgradeCapacity);
        UpgradeSpeed.onClick.AddListener(_gameManager.UpgradeSpeed);

        SetUpgradePanelActivation(false);
    }

    public void SetMoneyText(int value) => MoneyText.text = $"${(value > 0 ? value.ToString("#,#") : value.ToString())}";
    public void SetFoodCapacityText(int currentAmount, int maxCapacity) => FoodCapacityText.text = $"{currentAmount}/{maxCapacity}";

    public void SetUpgradePanelActivation(bool active) => UpgradePanel.SetActive(active);
    public void SetSpeedUpgradePrice(string value) => UpgradeSpeedPrice.text = value;
    public void SetCapacityUpgradePrice(string value) => UpgradeCapacityPrice.text = value;
    public void SetSizeUpgradePrice(string value) => UpgradeSizePrice.text = value;
}