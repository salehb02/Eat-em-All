using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject Mouth;
    public Material FaceMaterial;

    public Upgrade[] Upgrades;
    private int _currentUpgrade;

    public float Fatness { get; private set; }

    private List<Food> _eatenFoods = new List<Food>();

    private GameManager _gameManager;
    private Animator _animator;
    private PlayerMovement _movement;

    [System.Serializable]
    public class Upgrade
    {
        public string Title;
        public int FoodCapacity;
        public float PlayerFoodSize;
        public Vector3 PlayerScale = Vector3.one;
        public int UpgradeNeededMoney;
    }

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _animator = GetComponentInChildren<Animator>();
        _movement = GetComponent<PlayerMovement>();

        transform.localScale = Upgrades[_currentUpgrade].PlayerScale;
    }

    private void Update()
    {
        Fatness = _eatenFoods.Count / (float)Upgrades[_currentUpgrade].FoodCapacity;
        _animator.SetFloat("Fat", Fatness);
        FaceMaterial.SetFloat("_Fatness", Fatness);

        transform.localScale = Vector3.Lerp(transform.localScale, Upgrades[_currentUpgrade].PlayerScale, Time.deltaTime * 3f);
    }

    public void EatFood(Food food)
    {
        _eatenFoods.Add(food);
        _animator.SetTrigger("Eat");
        _gameManager.UpdateFoodCapacity(_eatenFoods.Count, Upgrades[_currentUpgrade].FoodCapacity);
    }

    public bool CheckUpgrade()
    {
        if (_currentUpgrade == Upgrades.Length)
            return false;

        if (_gameManager.GetMoney() >= Upgrades[_currentUpgrade + 1].UpgradeNeededMoney)
        {
            _gameManager.UseMoney(Upgrades[_currentUpgrade + 1].UpgradeNeededMoney);
            UpgradePlayer();
            return true;
        }
        else
        {
            return false;
        }
    }

    private void UpgradePlayer()
    {
        if (_currentUpgrade < Upgrades.Length - 1)
            _currentUpgrade++;
    }

    public void SpitFoods(FoodToMoneyConvertor convertor)
    {
        StartCoroutine(SpitFoodsCoroutine(convertor));
    }

    private IEnumerator SpitFoodsCoroutine(FoodToMoneyConvertor convertor)
    {
        _movement.Controllable = false;
        _animator.SetTrigger("Spit");

        foreach (var food in _eatenFoods.ToArray())
        {
            food.Spitted(convertor.FoodEnterPoint.transform.position);
            _eatenFoods.Remove(food);
            _gameManager.UpdateFoodCapacity(_eatenFoods.Count, Upgrades[_currentUpgrade].FoodCapacity);
            _gameManager.SpawnMoney(food.Prize, convertor.MoneySpawnPoint.transform);
            yield return new WaitForSeconds(0.2f);
        }

        _movement.Controllable = true;
    }

    public void ThrowMoney(Vector3 moneyEntrance)
    {

    }

    private void OnTriggerStay(Collider other)
    {
        if (_eatenFoods.Count >= Upgrades[_currentUpgrade].FoodCapacity)
            return;

        if (other.CompareTag("Money"))
        {
            var money = other.GetComponentInParent<Money>();
            money?.MoveToPlayer(this);
            return;
        }

        if (!other.CompareTag("Food"))
            return;

        var food = other.GetComponent<Food>();

        if (!food)
            return;

        var canEat = food.CanEat(Upgrades[_currentUpgrade].PlayerFoodSize);

        if (canEat)
        {
            food.EatFood(this);
            EatFood(food);
        }
    }
}