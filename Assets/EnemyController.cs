using System.IO;
using UnityEngine;
// Note this line, if it is left out, the script won't know that the class 'Path' exists and it will throw compiler errors
// This line should always be present at the top of scripts which use pathfinding
using Pathfinding;
using System;

public class EnemyController : MonoBehaviour
{
    private Seeker seeker;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private enum State
    {
        Idle,
        ChaseTarget,
        AttackTarget
    }
    private State state;
    public float chaseRange = 1;
    public float attackRange = 0.15F;
    public float damage = 1;

    // values for a* algorithm
    public Transform targetPosition;
    public float speed = 0.5F;
    public float nextWaypointDistance = 0.2F;
    public float updatePathTime = 2;

    public float distanceOffset = 2;

    private Movement mv;
    public Health health;

    public void Start()
    {
        seeker = GetComponent<Seeker>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        mv = new Movement(seeker, speed, nextWaypointDistance, updatePathTime);
        health = new Health(3, Defeated);

        state = State.Idle;
    }

    public void FixedUpdate()
    {
        switch (state)
        {
            default:
            case State.Idle:
                Debug.Log("Bat in idle state");
                //animator.SetBool("isMoving", false);
                if (Vector3.Distance(transform.position, PlayerController.Instance.GetPosition()) < chaseRange)
                {
                    if (Vector3.Distance(transform.position, PlayerController.Instance.GetPosition()) < attackRange)
                    {
                        //Player within attack range
                        state = State.AttackTarget;
                    } else
                    {
                        //Player within target range
                        state = State.ChaseTarget;
                    }
                }
                break;

            case State.AttackTarget:
                Debug.Log("Bat in attack state");
                if (Vector3.Distance(transform.position, PlayerController.Instance.GetPosition()) < attackRange)
                {
                    //Player within attack range
                    animator.SetTrigger("isAttacking");
                }  else
                {
                    //Player outside of target range
                    state = State.Idle;
                }
                break;

            case State.ChaseTarget:
                Debug.Log("Bat in chase state");
                Vector3 dir = mv.Move(transform.position, targetPosition.position);
                if (dir != Vector3.zero)
                {
                    animator.SetBool("isMoving", true);
                    if (dir.x < 0)
                    {
                        spriteRenderer.flipX = true;
                    }
                    else if (dir.x > 0)
                    {
                        spriteRenderer.flipX = false;
                    }
                }
                // Move the bat
                transform.position += dir * Time.deltaTime;

                if (Vector3.Distance(transform.position, PlayerController.Instance.GetPosition()) + distanceOffset > chaseRange)
                {
                    //Player outside of target range
                    state = State.Idle;
                }
                break;
        }
        
    }

    public void Attack()
    {
        mv.LockMovement();
    }

    public void EndAttack()
    {
        mv.UnlockMovement();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            //Deal damage to player
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.health.ReduceHealth(damage);
            }
        }
    }

    public void Defeated()
    {
        animator.SetTrigger("defeated");
    }
    public void RemoveEnemy()
    { // called from inside "death"-animation
        Destroy(gameObject);
    }
}
