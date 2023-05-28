using System.IO;
using UnityEngine;
// Note this line, if it is left out, the script won't know that the class 'Path' exists and it will throw compiler errors
// This line should always be present at the top of scripts which use pathfinding
using Pathfinding;
using System;

public class BatController : MonoBehaviour
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
    public float attackRange = 0.1F;
    public float damage = 1;
    private float nextAttackTime;
    public float attackRate = 0.1F; 

    // values for a* algorithm
    public Transform targetPosition;
    public float speed = 0.5F;
    public float nextWaypointDistance = 0.2F;
    public float updatePathTime = 2;

    public float timeUntilSleeping = 2;
    private float sleepTime;
    public float distanceOffset = 2;

    private Movement mv;
    public Health health;

    private CircleCollider2D attackCollider;

    public void Start()
    {
        seeker = GetComponent<Seeker>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        mv = new Movement(seeker, speed, nextWaypointDistance, updatePathTime);
        health = new Health(3, ReceivedDamage, Defeated);

        attackCollider = GetComponent<CircleCollider2D>();

        state = State.Idle;
    }

    public void FixedUpdate()
    {
        switch (state)
        {
            default:
            case State.Idle:
                if (Vector3.Distance(transform.position, PlayerController.Instance.GetPosition()) < chaseRange)
                {
                    //Player within target range
                    state = State.ChaseTarget;
                } else
                {
                    if (Time.time > sleepTime)
                    {
                        animator.SetBool("isMoving", false);
                    }
                }
                break;

            case State.AttackTarget:
                //Player within attack range
                if (Time.time > nextAttackTime)
                {
                    animator.SetTrigger("isAttacking");
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
                    sleepTime = Time.time + timeUntilSleeping;
                    state = State.Idle;
                }
                break;
        }
        
    }

    // Called by 'bat_attack' animation
    public void Attack()
    {
        mv.LockMovement();
        attackCollider.enabled = true;
    }

    // Called by 'bat_attack' animation
    public void EndAttack()
    {
        mv.UnlockMovement();
        attackCollider.enabled = false;
    }

    public void Defeated(float val)
    {
        Debug.Log("Bat has been slayed");
        animator.SetTrigger("defeated");
    }

    public void ReceivedDamage(float val)
    {
        Debug.Log("Bat received " + val + " damage");
        animator.SetTrigger("receivesDamage");
    }

    public void RemoveEnemy()
    { // called from inside "death"-animation
        //Destroy(gameObject);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTriggerEnter2D was called");
        if (collision.tag == "Player")
        {
            //Deal damage to player
            PlayerController player = collision.GetComponent<PlayerController>();
            player.health.ReduceHealth(damage);
        }
    }

}
