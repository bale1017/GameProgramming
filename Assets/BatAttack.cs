using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatAttack : MonoBehaviour
{
    public float damage = 1;
    CircleCollider2D attackCollider;
    Vector2 rightAttackOffset;

    // Start is called before the first frame update
    void Start()
    {
        attackCollider = GetComponent<CircleCollider2D>();
        rightAttackOffset = transform.localPosition;
    }

    public void AttackRight()
    {
        transform.localPosition = rightAttackOffset;
        attackCollider.enabled = true;
        SoundPlayer.current.PlaySound(Sound.BAT_ATTACK, transform);
    }

    public void AttackLeft()
    {
        transform.localPosition = new Vector3(rightAttackOffset.x * -1, rightAttackOffset.y);
        attackCollider.enabled = true;
        SoundPlayer.current.PlaySound(Sound.BAT_ATTACK, transform);
    }

    public void StopAttack()
    {
        attackCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTriggerEnter2D of Bat called");
        if (collision.tag == "Player")
        {
            //Deal damage to player
            Health player = collision.GetComponent<Health>();
            player.AffectHealth(-damage);
        }
    }
}
