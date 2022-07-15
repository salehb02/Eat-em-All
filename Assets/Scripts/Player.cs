using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int FoodCapacity = 10;
    public float PlayerSize = 2;

    private List<Food> _eatenFoods = new List<Food>();

    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    public void EatFood(Food food)
    {
        _eatenFoods.Add(food);
        _gameManager.UpdateFoodCapacity(_eatenFoods.Count,FoodCapacity);
    }

    public void AddFoodCapacity(int amount = 1)
    {
        FoodCapacity += amount;
    }

    public void AddPlayerSize(float sizeAmount, float addToTransformSize)
    {

    }

    private Vector3 RandomPointOnXZCircle(Vector3 center, float radius)
    {
        var angle = Random.Range(0, 2f * Mathf.PI);
        return center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
    }

    public void SpitFoods()
    {
        foreach (var food in _eatenFoods.ToArray())
        {
            food.Spitted(RandomPointOnXZCircle(transform.position, 2f));
            _gameManager.AddMoney(food.Prize);
            _eatenFoods.Remove(food);
        }
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
        }
    }
}