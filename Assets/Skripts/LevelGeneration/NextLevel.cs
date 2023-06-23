using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            loadNextLevel();
        }
    }

    private void loadNextLevel()
    {
        GameObject camera = GameObject.Find("Main Camera");
        camera.GetComponentInChildren<SpriteRenderer>().enabled = true;
        GenerateLevel generateLevel = GameObject.Find("Game").GetComponent<GenerateLevel>();
        foreach (var obj in generateLevel.createdObjects)
        {
            Destroy(obj);
        }
        // move player and Camera
        GameObject player = GameObject.Find("Player");
        player.transform.position = new Vector2(-5, 0);
        camera.transform.position = new Vector2(-5, 0);
        generateLevel.level++;

        generateLevel.GenerateLayout();
        camera.GetComponentInChildren<SpriteRenderer>().enabled = false;
    }
}
