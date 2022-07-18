using UnityEngine;

public class Money : MonoBehaviour
{
    private int _value;
    private Player _player;
    private float _moveProgress;
    private GameManager _gameManager;
    private Rigidbody _rigid;

    private void Start()
    {
        _rigid = GetComponent<Rigidbody>();
        _gameManager = FindObjectOfType<GameManager>();

        _rigid.AddForce(-transform.forward * 150f);
    }

    public void SetValue(int value) => _value = value;

    public void MoveToPlayer(Player player)
    {
        _player = player;

        foreach (var collider in GetComponentsInChildren<Collider>())
            collider.enabled = false;
    }

    private void Update()
    {
        if (!_player)
            return;

        _moveProgress += Time.deltaTime;
        var timeToReach = 1f;

        _rigid.MovePosition(Vector3.Lerp(transform.position, _player.Mouth.transform.position, _moveProgress / timeToReach));

        if (Vector3.Distance(transform.position, _player.Mouth.transform.position) < 0.3f)
        {
            _gameManager.AddMoney(_value);
            _player.TriggerEatAnimation();
            Destroy(gameObject);
        }
    }
}