using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public float damage = 3;
    public BoxCollider2D swordCollider;
    Vector2 rightAttackOffset;

    // Start is called before the first frame update
    void Start()
    {
        //swordCollider = GetComponent<BoxCollider2D>();
        rightAttackOffset = transform.position;
    }

    public void AttackRight()
    {
        print("Attack right");
        swordCollider.enabled = true;
        transform.localPosition = rightAttackOffset;
    }

    public void AttackLeft()
    {
        print("Attack left");
        swordCollider.enabled = true;
        transform.localPosition = new Vector3(rightAttackOffset.x * -1, rightAttackOffset.y);
    }

    public void StopAttack()
    {
        swordCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "EnemyController")
        {
            //Deal damage to enemy
            EnemyController enemy = collision.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.Health -= damage;
            }
        }
    }
}

