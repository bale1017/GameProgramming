using UnityEngine;

public class BatAttack : MonoBehaviour
{
    public float damage = 3;
    CircleCollider2D batAttackCollider;
    Vector2 rightAttackOffset;

    // Start is called before the first frame update
    void Start()
    {
        batAttackCollider = GetComponent<CircleCollider2D>();
        rightAttackOffset = transform.localPosition;
    }

    public void AttackRight()
    {
        Debug.Log("Bat attacks right");
        batAttackCollider.enabled = true;
        transform.localPosition = rightAttackOffset;
    }

    public void AttackLeft()
    {
        Debug.Log("Bat attacks left");
        batAttackCollider.enabled = true;
        transform.localPosition = new Vector3(rightAttackOffset.x * -1, rightAttackOffset.y);
    }

    public void StopAttack()
    {
        batAttackCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTriggerEnter2D was called");
        if (collision.tag == "Player")
        {
            Debug.Log("Attacks player");
            //Deal damage to player
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.health.ReduceHealth(damage);
            }
        }
    }
}
