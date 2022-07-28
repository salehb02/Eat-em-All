using UnityEngine;

public class EmotionController : MonoBehaviour
{
    private Player player;

    private void Start()
    {
        player = GetComponentInParent<Player>();
    }

    public void EnterEmotions()
    {
        player.PlayingEmotion = true;
    }

    public void ExitEmotions()
    {
        player.PlayingEmotion = false;
    }
}