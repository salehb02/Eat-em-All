using UnityEngine;
using DG.Tweening;

public class Food : MonoBehaviour
{
    public float FoodSize = 1;
    public int Prize = 5;

    private bool _moveToPlayer;
    private float _moveProgress;
    private Player _player;
    private Vector3 _initScale;
    private bool _eatable = true;

    private void Start()
    {
        _initScale = transform.localScale;
    }

    private void Update()
    {
        if (_moveToPlayer)
            MoveToPlayer();
    }

    public bool CanEat(float playerSize)
    {
        if (!_eatable)
            return false;

        if (playerSize >= FoodSize)
            return true;

        return false;
    }

    public void EatFood(Player player)
    {
        _player = player;
        _moveToPlayer = true;

        transform.DOScale(Vector3.zero, 1f);
    }

    public void Spitted(Vector3 pointToGo)
    {
        transform.SetParent(null);
        gameObject.SetActive(true);

        transform.DOMove(pointToGo, 0.5f).OnComplete(() =>
        {
            Destroy(gameObject, 1f);
        });

        transform.DOScale(_initScale, 0.5f);
    }

    private void MoveToPlayer()
    {
        _moveProgress += Time.deltaTime;
        var timeToReach = 3f;

        transform.position = Vector3.Lerp(transform.position, _player.transform.position, _moveProgress / timeToReach);

        if (Vector3.Distance(transform.position, _player.transform.position) < 0.2f)
        {
            _player.EatFood(this);
            gameObject.SetActive(false);
            transform.SetParent(_player.transform);
            _moveToPlayer = false;
            _eatable = false;
        }
    }
}