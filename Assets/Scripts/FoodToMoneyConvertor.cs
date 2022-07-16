using UnityEngine;

public class FoodToMoneyConvertor : MonoBehaviour
{
    public GameObject FoodEnterPoint;
    public GameObject MoneySpawnPoint;

    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        var player = other.GetComponentInParent<Player>();

        player?.SpitFoods(this);

    }
}