using UnityEngine;

public class NextLevelTrigger : MonoBehaviour
{
    private GameManager _gameManager;

    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        _gameManager.NextLevel();
    }
}
