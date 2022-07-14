using UnityEngine;

public class Player : MonoBehaviour
{
    public int FoodCapacity = 10;
    public float PlayerSize = 2;

    private int _currentFoodsCount;

    public void EatFood()
    {
        _currentFoodsCount++;
    }

    public void AddFoodCapacity(int amount = 1)
    {
        FoodCapacity += amount;
    }

    public void AddPlayerSize(float sizeAmount, float addToTransformSize)
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (_currentFoodsCount >= FoodCapacity)
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