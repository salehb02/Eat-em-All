using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodToMoneyConvertor : MonoBehaviour
{
    public GameObject FoodEnterPoint;
    public GameObject MoneySpawnPoint;
    public List<GameObject> MoneyPoints;

    private Animator _animator;
    private GameManager gameManager;
    private List<MoneyPlaceholder> availablePoints = new List<MoneyPlaceholder>();

    [System.Serializable]
    public class MoneyPlaceholder
    {
        public Vector3 Position;
        public GameObject Money;
        public bool IsFull;
        public bool IsGoingToFull;
    }

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        gameManager = FindObjectOfType<GameManager>();

        InitMoneyPlaceholders();
    }

    private void Update()
    {
        CheckMoneyPlaceholders();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        var player = other.GetComponentInParent<Player>();
        player?.SpitFoods(this);

        var playerMovement = other.GetComponentInParent<PlayerMovement>();
        playerMovement.CustomObjectLookAt = FoodEnterPoint.transform;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        var playerMovement = other.GetComponentInParent<PlayerMovement>();
        var player = other.GetComponentInParent<Player>();
        player.StopSpitting();
        playerMovement.CustomObjectLookAt = null;
    }

    public void TriggerVacuum() => _animator.SetTrigger("Vacuum");

    public void InstantiateMoney()
    {
        StartCoroutine(InstantiateMoneyCoroutine());
    }

    private IEnumerator InstantiateMoneyCoroutine()
    {
        var money = Instantiate(gameManager.MoneyPrefab, MoneySpawnPoint.transform.position, MoneySpawnPoint.transform.rotation);
        var rigid = money.GetComponent<Rigidbody>();
        rigid.AddForce(-transform.forward * 150f);
        rigid.isKinematic = true;

        yield return new WaitForSeconds(1f);
        rigid.velocity = Vector3.zero;

        foreach (var holder in availablePoints)
        {
            if (holder.IsFull || holder.IsGoingToFull)
                continue;

            holder.IsGoingToFull = true;
            rigid.DOMove(holder.Position, 0.5f).OnComplete(()=> { holder.Money = money.gameObject; holder.IsFull = true; });
            yield break;
        }
    }

    private void InitMoneyPlaceholders()
    {
        foreach (var holder in MoneyPoints)
        {
            var newHolder = new MoneyPlaceholder();
            newHolder.Position = holder.transform.position;

            availablePoints.Add(newHolder);
        }
    }

    private void CheckMoneyPlaceholders()
    {
        foreach (var holder in availablePoints)
        {
            if (!holder.IsFull)
                continue;

            if (Vector3.Distance(holder.Money.transform.position, holder.Position) > 0.5f)
            {
                holder.Money = null;
                holder.IsFull = false;
                holder.IsGoingToFull = false;
            }
        }
    }
}