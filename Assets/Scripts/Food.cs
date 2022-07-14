using UnityEngine;

public class Food : MonoBehaviour
{
    public float FoodSize;

    private bool _moveToPlayer;
    private float _moveProgress;
    private Player _player;

    private void Update()
    {
        if (_moveToPlayer)
            MoveToPlayer();
    }

    public bool CanEat(float playerSize)
    {
        if (playerSize >= FoodSize)
            return true;

        return false;
    }

    public void EatFood(Player player)
    {
        _player = player;
        _moveToPlayer = true;
    }

    private void MoveToPlayer()
    {
        _moveProgress += Time.deltaTime;
        var timeToReach = 1.5f;

        transform.position = Vector3.Lerp(transform.position, _player.transform.position, _moveProgress / timeToReach);

        if (Vector3.Distance(transform.position, _player.transform.position) < 0.2f)
        {
            _player.EatFood();
            Destroy(gameObject);
        }
    }
}