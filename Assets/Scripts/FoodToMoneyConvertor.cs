using UnityEngine;

public class FoodToMoneyConvertor : MonoBehaviour
{
    public GameObject FoodEnterPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        var player = other.GetComponentInParent<Player>();

        player?.SpitFoods(FoodEnterPoint.transform.position);
    }
}