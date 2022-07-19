using UnityEngine;

public class Money : MonoBehaviour, IVacuumable
{
    private int _value;
    private float _moveProgress;
    private bool _moveToPlayer;
    private Player _player;
    private GameManager _gameManager;
    private Rigidbody _rigid;

    private void Start()
    {
        // Load needed components
        _rigid = GetComponent<Rigidbody>();
        _gameManager = FindObjectOfType<GameManager>();
        _player = FindObjectOfType<Player>();

        // Initial force to throw money
        _rigid.AddForce(-transform.forward * 150f);
    }

    private void Update()
    {
        MovingToPlayer();
    }

    public void SetValue(int value) => _value = value;

    // Trigger to start moving to player
    public void StartMoveToPlayer()
    {
        _moveToPlayer = true;

        foreach (var collider in GetComponentsInChildren<Collider>())
            collider.enabled = false;
    }

    private void MovingToPlayer()
    {
        if (!_moveToPlayer)
            return;

        _moveProgress += Time.deltaTime;
        var timeToReach = 1f;

        _rigid.MovePosition(Vector3.Lerp(transform.position, _player.Mouth.transform.position, _moveProgress / timeToReach));

        if (Vector3.Distance(transform.position, _player.Mouth.transform.position) < 0.3f)
        {
            _gameManager.AddMoney(_value);
            OnEndVaccum();
            Destroy(gameObject);
        }
    }

    public void OnEndVaccum()
    {
        _player.EndVacuuming(this);
    }
}