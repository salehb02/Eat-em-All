using UnityEngine;

public class Money : MonoBehaviour
{
    private int _value;
    private Player _player;
    private float _moveProgress;
    private GameManager _gameManager;
    private Rigidbody _rigid;
    private bool _readyToAchive = false;

    private void Start()
    {
        _rigid = GetComponent<Rigidbody>();
        _gameManager = FindObjectOfType<GameManager>();

        _rigid.AddForce(-transform.forward * 150f);
        Invoke(nameof(MakeMoneyReady), 2f);
    }

    public void SetValue(int value) => _value = value;

    public void MoveToPlayer(Player player)
    {
        if (!_readyToAchive)
            return;

        _player = player;
        _rigid.isKinematic = true;

        foreach (var collider in GetComponentsInChildren<Collider>())
            collider.enabled = false;
    }

    private void Update()
    {
        if (!_player)
            return;

        _moveProgress += Time.deltaTime;
        var timeToReach = 3f;
        transform.position = Vector3.Lerp(transform.position, _player.Mouth.transform.position, _moveProgress / timeToReach);

        if (Vector3.Distance(transform.position, _player.Mouth.transform.position) < 0.2f)
        {
            _gameManager.AddMoney(_value);
            _player.TriggerEatAnimation();
            Destroy(gameObject);
        }
    }

    private void MakeMoneyReady()
    {
        _readyToAchive = true;
    }
}