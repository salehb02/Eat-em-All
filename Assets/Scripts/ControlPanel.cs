using UnityEngine;

[CreateAssetMenu(menuName = "Current Project/Control Panel")]
public class ControlPanel : ScriptableObject
{
    public SpeedUpgrade[] SpeedUpgrades;
    public UpgradeExtended[] CapacityUpgrades;
    public SizeUpgrade[] SizeUpgrades;

    public float PriceMultiplierPerLevel = 1.2f;
    public bool BeRedWhileVacuuming;
    public bool LoadLastLevel;
    public float MouthSpeed = 1f;
    public float FoodVacuumSpeedMultiplier = 2f;
    public float MinFoodVacuumSpeed;

    [System.Serializable]
    public class Upgrade
    {
        public string Title;
        public int Price;
    }

    [System.Serializable]
    public class UpgradeExtended : Upgrade
    {
        public float Value;
    }

    [System.Serializable]
    public class SpeedUpgrade : Upgrade
    {
        public float NormalSpeed;
        public float FatSpeed;
    }

    [System.Serializable]
    public class SizeUpgrade : Upgrade
    {
        public Vector3 PlayerScale;
        public float FoodSizeSupport;
        public float CameraOffset;
    }

    #region Singleton
    private static ControlPanel instance;
    public static ControlPanel Instance
    {
        get => instance == null ? instance = Resources.Load("ControlPanel") as ControlPanel : instance;
    }
    #endregion
}