using UnityEngine;
using DG.Tweening;

public class Food : MonoBehaviour, IVacuumable
{
    public float FoodSize = 1;
    public int Prize = 5;

    private bool _moveToPlayer;
    private float _moveProgress;
    private Player _player;
    private Vector3 _initScale;
    private bool _eatable = true;
    private Vector3 _startLerpPos;
    public bool Eaten { get; private set; }

    private void Start()
    {
        _initScale = transform.localScale;
        _player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        if (_moveToPlayer)
            MoveToPlayer();
    }

    public bool IsEatable(float playerSize)
    {
        if (!_eatable)
            return false;

        if (playerSize >= FoodSize)
            return true;

        return false;
    }

    public void Spitted(Vector3 pointToGo)
    {
        transform.SetParent(null);
        gameObject.SetActive(true);

        transform.DOMove(pointToGo, 0.3f).OnComplete(() =>
        {
            Destroy(gameObject, 0.5f);
        });

        transform.DOScale(_initScale, 0.15f).OnComplete(() =>
        {
            transform.DOScale(Vector3.zero, 0.15f);
        });
    }

    private void MoveToPlayer()
    {
        _moveProgress += Time.deltaTime;
        var timeToReach = (FoodSize / _player.FoodSize) / ControlPanel.Instance.FoodVacuumSpeedMultiplier;
        timeToReach = Mathf.Clamp(timeToReach, ControlPanel.Instance.MinFoodVacuumSpeed, Mathf.Infinity);

        transform.position = Vector3.Lerp(_startLerpPos, _player.Mouth.transform.position, _moveProgress / timeToReach);
        transform.localScale = Vector3.Lerp(_initScale, Vector3.zero, _moveProgress / timeToReach);

        if (Vector3.Distance(transform.position, _player.Mouth.transform.position) < 0.2f)
            EndVaccum();
    }

    public void EndVaccum()
    {
        gameObject.SetActive(false);
        transform.SetParent(_player.transform);
        _moveToPlayer = false;
        _eatable = false;
        _player.EatFood(this);
        _player.EndVacuuming(this);
    }

    public void StartVacuum()
    {
        _moveToPlayer = true;

        _eatable = false;
        Eaten = true;

        _startLerpPos = transform.position;
    }
}