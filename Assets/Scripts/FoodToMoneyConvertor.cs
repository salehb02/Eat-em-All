using UnityEngine;

public class FoodToMoneyConvertor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        var player = other.GetComponentInParent<Player>();

        player?.SpitFoods();
    }
}