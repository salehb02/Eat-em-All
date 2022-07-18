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
    private List<Food> _vacumingFoods = new List<Food>();
    private float _mouthOpen;
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

    }

    private void Update()
    {
        LoadUpgrades();

        Fatness = _eatenFoods.Count / (float)FoodCapacity;
        _animator.SetFloat("Fat", Fatness);

        if (_vacumingFoods.Count == 0 || !ControlPanel.Instance.BeRedWhileVacuuming)
            FaceMaterial.SetFloat("_Fatness", Mathf.Lerp(FaceMaterial.GetFloat("_Fatness"), Fatness, Time.deltaTime * 2f));

        var emission = FatnessParticle.emission;

        if (Fatness >= 0.5f)
        {
            emission.rateOverTimeMultiplier = Mathf.Lerp(0, MaxParticlesCount, Fatness);
        }
        else
        {
            emission.rateOverTimeMultiplier = Mathf.Lerp(emission.rateOverTimeMultiplier, 0, Time.deltaTime * 5f);
        }

        transform.localScale = Vector3.Lerp(transform.localScale, PlayerScale, Time.deltaTime * 3f);
        _gameManager.UpdateFoodCapacity(_eatenFoods.Count, FoodCapacity);
     
        ToNextLevelTriggerArrow();
        FoodVacumMode();
    }

    private void ToNextLevelTriggerArrow()
    {
        if (!_showNextLevelArrow || Vector3.Distance(transform.position, _nextLevelTriggerPos) <= 8f)
        {
            ToNextLevelArrow.gameObject.SetActive(false);
            return;
        }

        ToNextLevelArrow.SetActive(true);
        ToNextLevelArrow.transform.rotation = Quaternion.LookRotation((_nextLevelTriggerPos - transform.position).normalized);
    }

    private void FoodVacumMode()
    {
        _mouthOpen = Mathf.Lerp(_mouthOpen, _vacumingFoods.Count > 0 ? 1 : 0, Time.deltaTime * 5f);
        _animator.SetFloat("MouthOpen", _mouthOpen);

        if (ControlPanel.Instance.BeRedWhileVacuuming)
            FaceMaterial.SetFloat("_Fatness", Mathf.Lerp(FaceMaterial.GetFloat("_Fatness"), 1, Time.deltaTime));
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

    public int SpitFoods(FoodToMoneyConvertor convertor)
    {
        StartCoroutine(SpitFoodsCoroutine(convertor));
        return _eatenFoods.Count;
    }

    private IEnumerator SpitFoodsCoroutine(FoodToMoneyConvertor convertor)
    {
        if (_eatenFoods.Count > 0)
        {
            _animator.SetTrigger("Spit");
        }

        foreach (var food in _eatenFoods.ToArray())
        {
            food.Spitted(convertor.FoodEnterPoint.transform.position);
            _eatenFoods.Remove(food);
            _gameManager.UpdateFoodCapacity(_eatenFoods.Count, FoodCapacity);
            _gameManager.SpawnMoney(food.Prize, convertor.MoneySpawnPoint.transform);
            convertor.TriggerVacuum();
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Money"))
        {
            var headingMoney = other.transform.position - transform.position;
            var dotMoney = Vector3.Dot(headingMoney, transform.forward);

            if (dotMoney < 0.1f)
                return;

            var money = other.GetComponentInParent<Money>();
            money?.MoveToPlayer(this);
            return;
        }

        if (_eatenFoods.Count >= FoodCapacity)
            return;

        if (!other.CompareTag("Food"))
            return;

        var food = other.GetComponent<Food>();

        if (!food)
            return;

        // Check if food is in front
        var heading = food.transform.position - transform.position;

        var dot = Vector3.Dot(heading, transform.forward);

        if (dot < 0.1f)
        {
            if (_vacumingFoods.Contains(food))
                _vacumingFoods.Remove(food);

            return;
        }

        var canEat = food.CanEat(FoodSize);

        if (canEat)
        {
            food.EatFood();
            EatFood(food);
        }
        else
        {
            food.SetJustVacum(true);

            if (!_vacumingFoods.Contains(food) && food.Eaten == false)
                _vacumingFoods.Add(food);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Food"))
            return;

        var food = other.GetComponent<Food>();

        if (!food)
            return;

        food.SetJustVacum(false);
        _vacumingFoods.Remove(food);
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