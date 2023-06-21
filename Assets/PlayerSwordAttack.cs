using BasePatterns;
using UnityEngine;

public class PlayerSwordAttack : MonoBehaviour
{
    public AudioSource Slash;
    public float damage = 3;
    BoxCollider2D swordCollider;
    Vector2 rightAttackOffset;

    //Start is called before the first frame update
    void Start()
    {
        swordCollider = GetComponent<BoxCollider2D>();
        rightAttackOffset = transform.localPosition;
    }

    public void AttackRight()
    {
        Slash.Play();
        Debug.Log("Sword attack right");
        transform.localPosition = rightAttackOffset;
        swordCollider.enabled = true;
    }

    public void AttackLeft()
    {
        Slash.Play();
        Debug.Log("Sword attack left");
        transform.localPosition = new Vector3(rightAttackOffset.x * -1, rightAttackOffset.y);
        swordCollider.enabled = true;
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

