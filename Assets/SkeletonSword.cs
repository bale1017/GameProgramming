using UnityEngine;

public class SkeletonSword : MonoBehaviour
{
    public float damage = 2;
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
        transform.localPosition = rightAttackOffset;
        swordCollider.enabled = true;
    }

    public void AttackLeft()
    {
        transform.localPosition = new Vector3(rightAttackOffset.x * -1, rightAttackOffset.y);
        swordCollider.enabled = true;
    }

    public void StopAttack()
    {
        swordCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTriggerEnter2D of Skeleton called");
        if (collision.tag == "Player")
        {
            //Deal damage to player
            PlayerController player = collision.GetComponent<PlayerController>();
            player.health.ReduceHealth(damage);
        }
    }
}
