using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpikeTrap : MonoBehaviour
{

    public Sprite activeSpike;
    public Sprite inactiveSpike;
    public float triggerDelay = 1;
    public float activeTime = 2;

    private bool triggered = false;


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            if (triggered)
                print("damage player");
            else
                // wait
                StartCoroutine(activateSpike());
    }

    private IEnumerator activateSpike()
    {
        yield return new WaitForSeconds(triggerDelay);
        // extend spike
        this.GetComponent<SpriteRenderer>().sprite = activeSpike;
        triggered = true;
        yield return new WaitForSeconds(activeTime);
        triggered = false;
        this.GetComponent<SpriteRenderer>().sprite = inactiveSpike;
    }
}
