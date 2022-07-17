using UnityEngine;

public class UpgradeTrigger : MonoBehaviour
{
    private GameManagerPresentor _presentor;

    private void Start()
    {
        _presentor = FindObjectOfType<GameManagerPresentor>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        _presentor.SetUpgradePanelActivation(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        _presentor.SetUpgradePanelActivation(false);
    }
}