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
    private bool damaged = true;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!Game.current.IsRunning()) return;
        if (Game.IsRewinding) return;
        if (collision.tag != "Player") return;

        if (triggered && !damaged)
        {
            Health target = collision.GetComponent<Health>();
            target.AffectHealth(-damage);
            GetComponent<ReTime>().AddKeyFrame(g => damaged = true, g => damaged = false);
        }
        else if (!triggered)
        {
            StartCoroutine(activateSpike());
        }
    }

    private IEnumerator activateSpike()
    {
        if (!TryGetComponent<ReTime>(out var retime))
        {
            retime = gameObject.AddComponent<ReTime>();
        }
        retime.AddKeyFrame(g => triggered = true, g => triggered = false);
        retime.AddKeyFrame(g => damaged = true, g => damaged = false);

        yield return new WaitForSeconds(triggerDelay);

        retime.AddKeyFrame(g => damaged = false, g => damaged = true);
        retime.AddKeyFrame(
            g => g.GetComponent<SpriteRenderer>().sprite = activeSpike,
            g => g.GetComponent<SpriteRenderer>().sprite = inactiveSpike
        );
        
        yield return new WaitForSeconds(activeTime);
        
        retime.AddKeyFrame(g => triggered = false, g => triggered = true);
        retime.AddKeyFrame(
            g => g.GetComponent<SpriteRenderer>().sprite = inactiveSpike,
            g => g.GetComponent<SpriteRenderer>().sprite = activeSpike
        );
    }
}
