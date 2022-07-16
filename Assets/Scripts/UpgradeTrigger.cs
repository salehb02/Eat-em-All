using UnityEngine;

public class UpgradeTrigger : MonoBehaviour
{
    public GameObject MoneyEntrance;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        var player = other.GetComponentInParent<Player>();

        if (!player.CheckUpgrade())
            player.ThrowMoney(MoneyEntrance.transform.position);
    }
}