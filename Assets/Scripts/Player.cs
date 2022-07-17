using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject Mouth;
    public Material FaceMaterial;
    public GameObject ToNextLevelArrow;
    public ParticleSystem FatnessParticle;
    public int MaxParticlesCount = 20;

    private List<Food> _eatenFoods = new List<Food>();
    private int _foodsEatenTilNow;

    private GameManager _gameManager;
    private Animator _animator;
    private PlayerMovement _movement;
    private Vector3 _nextLevelTriggerPos;
    private bool _showNextLevelArrow;

    public int FoodCapacity { get; private set; }
    public float FoodSize { get; private set; }
    public Vector3 PlayerScale { get; private set; }
    public float PlayerSpeed { get; private set; }
    public float Fatness { get; private set; }

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _animator = GetComponentInChildren<Animator>();
        _movement = GetComponent<PlayerMovement>();

        ToNextLevelArrow.gameObject.SetActive(false);
    }

    private void Update()
    {
        LoadUpgrades();

        Fatness = _eatenFoods.Count / (float)FoodCapacity;
        _animator.SetFloat("Fat", Fatness);
        FaceMaterial.SetFloat("_Fatness", Fatness);

        var emission = FatnessParticle.emission;
        emission.rateOverTimeMultiplier = Mathf.Lerp(0,MaxParticlesCount,Fatness);

        transform.localScale = Vector3.Lerp(transform.localScale, PlayerScale, Time.deltaTime * 3f);
        _gameManager.UpdateFoodCapacity(_eatenFoods.Count, FoodCapacity);

        ToNextLevelArrow.transform.rotation = Quaternion.LookRotation((_nextLevelTriggerPos - transform.position).normalized);

        if (_showNextLevelArrow)
        {
            if (Vector3.Distance(transform.position, _nextLevelTriggerPos) > 8f)
            {
                ToNextLevelArrow.SetActive(true);
            }
            else
            {
                ToNextLevelArrow.SetActive(false);
            }
        }
    }

    private void LoadUpgrades()
    {
        FoodCapacity = System.Convert.ToInt32(ControlPanel.Instance.CapacityUpgrades[_gameManager.CapacityUpgrade].Value);
        FoodSize = ControlPanel.Instance.SizeUpgrades[_gameManager.SizeUpgrade].FoodSizeSupport;
        PlayerScale = ControlPanel.Instance.SizeUpgrades[_gameManager.SizeUpgrade].PlayerScale;
        PlayerSpeed = ControlPanel.Instance.SpeedUpgrades[_gameManager.SpeedUpgrade].Value;
    }

    public void EatFood(Food food)
    {
        _eatenFoods.Add(food);
        _foodsEatenTilNow++;
        _animator.SetTrigger("Eat");
        _gameManager.CheckEndLevel(_foodsEatenTilNow);
    }

    public void SpitFoods(FoodToMoneyConvertor convertor)
    {
        StartCoroutine(SpitFoodsCoroutine(convertor));
    }

    private IEnumerator SpitFoodsCoroutine(FoodToMoneyConvertor convertor)
    {
        if (_eatenFoods.Count > 0)
        {
            _movement.Controllable = false;
            _animator.SetTrigger("Spit");
        }

        foreach (var food in _eatenFoods.ToArray())
        {
            food.Spitted(convertor.FoodEnterPoint.transform.position);
            _eatenFoods.Remove(food);
            _gameManager.UpdateFoodCapacity(_eatenFoods.Count, FoodCapacity);
            _gameManager.SpawnMoney(food.Prize, convertor.MoneySpawnPoint.transform);
            yield return new WaitForSeconds(0.2f);
        }

        _movement.Controllable = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (_eatenFoods.Count >= FoodCapacity)
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

        var canEat = food.CanEat(FoodSize);

        if (canEat)
        {
            food.EatFood(this);
            EatFood(food);
        }
    }

    public void TriggerEatAnimation()
    {
        _animator.SetTrigger("Eat");
    }

    public void ActivateNextLevelArrow(Vector3 nextLevelPos)
    {
        _showNextLevelArrow = true;
        _nextLevelTriggerPos = nextLevelPos;
    }
}