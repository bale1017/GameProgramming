using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    private bool isTriggered = false;
    public Sprite closeTrapDoor;
    public Sprite openTrapDoor;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTriggered && !Game.IsRewinding)
        {
            Game.current.WinGame();
            return;
        }
        if (!TryGetComponent<ReTime>(out var retime))
        {
            retime = gameObject.AddComponent<ReTime>();
        }
        SoundPlayer.current.PlaySound(Sound.TRAPDOOR_OPEN);
        retime.AddKeyFrame(
            g =>
            {
                g.GetComponent<SpriteRenderer>().sprite = openTrapDoor;
                isTriggered = true;
            },
            g =>
            {
                g.GetComponent<SpriteRenderer>().sprite = closeTrapDoor;
                isTriggered = false;
            }
        );
    }
}
