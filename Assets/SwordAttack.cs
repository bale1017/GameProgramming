using BasePatterns;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public float damage = 3;
    BoxCollider2D swordCollider;
    Vector2 rightAttackOffset;

    // Start is called before the first frame update
    void Start()
    {
        swordCollider = GetComponent<BoxCollider2D>();
        rightAttackOffset = transform.localPosition;
    }

    public void AttackRight()
    {
        Debug.Log("Sword attack right");
        swordCollider.enabled = true;
        transform.localPosition = rightAttackOffset;
    }

    public void AttackLeft()
    {
        Debug.Log("Sword attack left");
        swordCollider.enabled = true;
        transform.localPosition = new Vector3(rightAttackOffset.x * -1, rightAttackOffset.y);
    }

    public void StopAttack()
    {
        swordCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            //Deal damage to enemy
            IController enemy = collision.GetComponent<IController>();
            if (enemy != null)
            {
                enemy.health.ReduceHealth(damage);
            }
        }
    }
}

