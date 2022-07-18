using UnityEngine;

public class FoodToMoneyConvertor : MonoBehaviour
{
    public GameObject FoodEnterPoint;
    public GameObject MoneySpawnPoint;

    private Animator _animator;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        var player = other.GetComponentInParent<Player>();
        player?.SpitFoods(this);
    }

    public void TriggerVacuum()
    {
        _animator.SetTrigger("Vacuum");
    }
}