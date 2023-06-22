using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpikeTrap : MonoBehaviour
{

    public float damage = 1;
    public Sprite activeSpike;
    public Sprite inactiveSpike;
    public float triggerDelay = 1;
    public float activeTime = 2;

    private bool triggered = false;
    bool recentlyActivated = false;


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !recentlyActivated)
            if (triggered)
            {
                PlayerController player = collision.GetComponent<PlayerController>();
                player.health.ReduceHealth(damage);
                recentlyActivated = true;
            }
            else
            // wait
            {
                StartCoroutine(activateSpike());
            }
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
        recentlyActivated = false;
    }
}
