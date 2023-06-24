using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    private bool isTriggered = false;
    public Sprite openTrapDoor;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isTriggered)
        {
            this.GetComponent<SpriteRenderer>().sprite = openTrapDoor;
            isTriggered = true;
        }
        else
        {
            Game.current.WinGame();
        }
    }
}
