using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int FoodCapacity = 10;
    public float PlayerSize = 2;
    public GameObject Mouth;

    public float Fatness { get; private set; }

    private List<Food> _eatenFoods = new List<Food>();

    private GameManager _gameManager;
    private Animator _animator;
    private PlayerMovement _movement;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _animator = GetComponentInChildren<Animator>();
        _movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        Fatness = _eatenFoods.Count / (float)FoodCapacity;
        _animator.SetFloat("Fat", Fatness);
    }

    public void EatFood(Food food)
    {
        _eatenFoods.Add(food);
        _animator.SetTrigger("Eat");
        _gameManager.UpdateFoodCapacity(_eatenFoods.Count,FoodCapacity);
    }

    public void AddFoodCapacity(int amount = 1)
    {
        FoodCapacity += amount;
    }

    public void AddPlayerSize(float sizeAmount, float addToTransformSize)
    {

    }

    public void SpitFoods(Vector3 foodEnterPoint)
    {
        StartCoroutine(SpitFoodsCoroutine(foodEnterPoint));
    }

    private IEnumerator SpitFoodsCoroutine(Vector3 foodEnterPoint)
    {
        _movement.Controllable = false;
        _animator.SetTrigger("Spit");

        foreach (var food in _eatenFoods.ToArray())
        {
            food.Spitted(foodEnterPoint);
            _gameManager.AddMoney(food.Prize);
            _eatenFoods.Remove(food);
            _gameManager.UpdateFoodCapacity(_eatenFoods.Count, FoodCapacity);
            yield return new WaitForSeconds(0.2f);
        }

        _movement.Controllable = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_eatenFoods.Count >= FoodCapacity)
            return;

        if (!other.CompareTag("Food"))
            return;

        var food = other.GetComponent<Food>();

        if (!food)
            return;

        var canEat = food.CanEat(PlayerSize);

        if (canEat)
        {
            food.EatFood(this);
            EatFood(food);
        }
    }
}