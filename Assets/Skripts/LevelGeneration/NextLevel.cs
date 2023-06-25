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
        if (isTriggered)
        {
            Game.current.WinGame();
            return;
        }
        ReTime time = GetComponent<ReTime>() ?? gameObject.AddComponent<ReTime>();
        time.AddKeyFrame(
            g =>
            {
                this.GetComponent<SpriteRenderer>().sprite = openTrapDoor;
                isTriggered = true;
            },
            g =>
            {
                this.GetComponent<SpriteRenderer>().sprite = closeTrapDoor;
                isTriggered = false;
            }
        );
    }
}
