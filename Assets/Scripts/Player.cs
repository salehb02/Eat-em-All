using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Public variables
    public GameObject Mouth;
    public Material FaceMaterial;
    public GameObject ToNextLevelArrow;
    public ParticleSystem FatnessParticle;
    public int MaxParticlesCount = 20;

    // Private variables
    private List<Food> _eatenFoods = new List<Food>();
    private List<IVacuumable> _vacuuming = new List<IVacuumable>();
    private List<Food> _justVacumingFoods = new List<Food>();
    private float _mouthOpen;
    private int _foodsEatenTilNow;
    private bool _showNextLevelArrow;
    private Vector3 _nextLevelTriggerPos;
    private GameManager _gameManager;
    private Animator _animator;

    // Properties
    public int FoodCapacity { get; private set; }
    public float FoodSize { get; private set; }
    public Vector3 PlayerScale { get; private set; }
    public float PlayerSpeed { get; private set; }
    public float Fatness { get; private set; }

    private void Start()
    {
        // Load needed components
        _gameManager = FindObjectOfType<GameManager>();
        _animator = GetComponentInChildren<Animator>();

        // Reset particles
        var emission = FatnessParticle.emission;
        emission.rateOverTimeMultiplier = 0;
    }

    private void Update()
    {
        LoadUpgrades();
        ProcessFatness();
        ProcessUpgrades();
        ProcessMaterials();
        ProcessParticles();
        ToNextLevelTriggerArrow();
        FoodVacumMode();
        MouthControl();
    }

    private void ProcessFatness()
    {
        Fatness = _eatenFoods.Count / (float)FoodCapacity;
        _animator.SetFloat("Fat", Fatness);
    }

    private void MouthControl()
    {
        _mouthOpen = Mathf.Lerp(_mouthOpen, _vacuuming.Count > 0 ? 1 : 0, Time.deltaTime * 5f);
        _animator.SetFloat("MouthOpen", _mouthOpen);
    }

    private void ProcessUpgrades()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, PlayerScale, Time.deltaTime * 3f);
        _gameManager.UpdateFoodCapacity(_eatenFoods.Count, FoodCapacity);
    }

    private void ProcessMaterials()
    {
        if (_justVacumingFoods.Count == 0 || !ControlPanel.Instance.BeRedWhileVacuuming)
            FaceMaterial.SetFloat("_Fatness", Mathf.Lerp(FaceMaterial.GetFloat("_Fatness"), Fatness, Time.deltaTime * 2f));
    }

    private void ProcessParticles()
    {
        var emission = FatnessParticle.emission;

        if (Fatness >= 0.5f)
        {
            emission.rateOverTimeMultiplier = Mathf.Lerp(0, MaxParticlesCount, Fatness);
        }
        else
        {
            emission.rateOverTimeMultiplier = Mathf.Lerp(emission.rateOverTimeMultiplier, 0, Time.deltaTime * 5f);
        }
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
        if (_justVacumingFoods.Count == 0 || _vacuuming.Count > 0)
            return;

        _mouthOpen = Mathf.Lerp(_mouthOpen, 1, Time.deltaTime * 5f);

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
        _gameManager.CheckEndLevel(_foodsEatenTilNow);
    }

    public void SpitFoods(FoodToMoneyConvertor convertor)
    {
        StartCoroutine(SpitFoodsCoroutine(convertor));
    }

    private IEnumerator SpitFoodsCoroutine(FoodToMoneyConvertor convertor)
    {
        if (_eatenFoods.Count == 0)
            yield break;

        _animator.SetTrigger("Spit");

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
        if (!other.CompareTag("Vacuumable"))
            return;

        // Check if object is in front
        var heading = other.transform.position - transform.position;
        var dot = Vector3.Dot(heading, transform.forward);

        // Get any vacuumable objects
        if (other.TryGetComponent<IVacuumable>(out var vacuumable))
        {
            // Return if vacuumable is not in front
            if (dot < 0.1f)
                return;

            vacuumable.StartVacuum();
            AddVacuumbale(vacuumable);
        }

        // Get foods
        if (other.TryGetComponent<Food>(out var food))
        {
            // Return if food is not in front
            if (dot < 0.1f)
            {
                if (_justVacumingFoods.Contains(food))
                    _justVacumingFoods.Remove(food);

                return;
            }

            // Return if player is full
            if (_eatenFoods.Count >= FoodCapacity)
                return;

            // Check if food is eatable
            if (food.IsEatable(FoodSize))
            {
                food.StartVacuum();
                AddVacuumbale(food);
            }
            else
            {
                if (!_justVacumingFoods.Contains(food) && food.Eaten == false)
                    _justVacumingFoods.Add(food);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Vacuumable"))
            return;

        if (other.TryGetComponent<Food>(out var food))
        {
            _justVacumingFoods.Remove(food);
        }
    }

    public void ActivateNextLevelArrow(Vector3 nextLevelPos)
    {
        _showNextLevelArrow = true;
        _nextLevelTriggerPos = nextLevelPos;
    }

    public void AddVacuumbale(IVacuumable vacuumable)
    {
        if (!_vacuuming.Contains(vacuumable))
            _vacuuming.Add(vacuumable);
    }

    public void EndVacuuming(IVacuumable vaccumable) => _vacuuming.Remove(vaccumable);
}