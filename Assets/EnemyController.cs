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
    private float nextAttackTime;
    public float attackRate = 0.05F; 

    // values for a* algorithm
    public Transform targetPosition;
    public float speed = 0.5F;
    public float nextWaypointDistance = 0.2F;
    public float updatePathTime = 2;

    public float distanceOffset = 2;

    public BatAttack attack;
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
                //animator.SetBool("isMoving", false);
                if (Vector3.Distance(transform.position, PlayerController.Instance.GetPosition()) < chaseRange)
                {
                    //Player within target range
                    state = State.ChaseTarget;
                }
                break;

            case State.AttackTarget:
                //Player within attack range
                if (Time.time > nextAttackTime)
                {
                    animator.SetTrigger("isAttacking");
                    //animator.SetBool("test", true);
                    nextAttackTime = Time.time + attackRate;
                    state = State.ChaseTarget;
                }
                break;

            case State.ChaseTarget:
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

                if (Vector3.Distance(transform.position, PlayerController.Instance.GetPosition()) < attackRange)
                {
                    //Player inside attack range
                    state = State.AttackTarget;
                } else if (Vector3.Distance(transform.position, PlayerController.Instance.GetPosition()) + distanceOffset > chaseRange)
                {
                    //Player outside of target range
                    state = State.Idle;
                }
                break;
        }
        
    }

    public void Attack()
    {
        Debug.Log("Bat attacks");
        mv.LockMovement();
        if (spriteRenderer.flipX == true)
        {
            attack.AttackLeft();
        }
        else
        {
            attack.AttackRight();
        }
    }

    public void EndAttack()
    {
        mv.UnlockMovement();
        attack.StopAttack();
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
