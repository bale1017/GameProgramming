using BasePatterns;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasePatterns;

public class BossController : MonoBehaviour, IController
{
    private Seeker seeker;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // generic values and values for state machine
    private EnemyState state;
    public float initialHealth = 10;
    public float chaseRange = 1;
    public float attackRange = 0.1F;
    public float damage = 7;
    public float attackRate = 0.1F;

    private bool isFirstPhase = true;
    private float nextAttackTime;

    // values for a* algorithm
    public Transform targetPosition;
    public float speed = 0.2F;
    public float nextWaypointDistance = 0.2F;
    public float updatePathTime = 2;

    private Movement movement;
    public Health health;

    Health IController.health { get => health; }

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        movement = new Movement(seeker, speed, nextWaypointDistance, updatePathTime);
        health = new Health(initialHealth, ReceivedDamage, Defeated);

        movement.PreCalcPath(transform.position, targetPosition.position);
        state = EnemyState.Idle;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if (!Game.current.IsRunning()) { 
            if (!Game.IsRewinding && !PauseMenu.Paused)
            {
                switch (state)
                {
                    default:
                    case EnemyState.Idle:
                        Idle();
                        break;

                    case EnemyState.AttackTarget:
                        AttackTarget();
                        break;

                    case EnemyState.ChaseTarget:
                        ChaseTarget();
                        break;
                }
            }
        //}
    }

    private void Idle()
    {
        if (Vector3.Distance(transform.position, PlayerController.Instance.GetPosition()) < chaseRange)
        {
            //Player within target range
            state = EnemyState.ChaseTarget;
        }
        else
        {
            animator.SetBool("isMoving", false);
            state = EnemyState.Idle;
        }
    }

    private void ChaseTarget()
    {
        Vector3 dir = movement.Move(transform.position, PlayerController.Instance.GetPosition());
        Move(dir);
        if (Vector3.Distance(transform.position, PlayerController.Instance.GetPosition()) < attackRange)
        {
            //Player inside attack range
            state = EnemyState.AttackTarget;
        }
    }

    private void AttackTarget()
    {
        //Player within atack range
        if (Time.time > nextAttackTime)
        {
            var randAttack = Random.Range(0, 3); //get random int [0, 1, 2] -> 1/3 chance for each
            if (randAttack == 0)
            {
                //use attack A
                animator.SetTrigger("isAttackingA");
            } else if (randAttack == 1)
            {
                //use attack B
                animator.SetTrigger("isAttackingB");
            } else
            {
                //use attack C
                animator.SetTrigger("isAttackingC");
            }

            nextAttackTime = Time.time + attackRate;
            state = EnemyState.ChaseTarget;
        }
    }

    private void Move(Vector3 dir)
    {
        if(!Movement.Equal(dir, Vector3.zero))
        {
            animator.SetBool("isMoving", true);

            //switch direction depending of the position of the next waypoint
            if (dir.x <= 0)
            {
                spriteRenderer.flipX = true;
            } else if (dir.x > 0)
            {
                spriteRenderer.flipX = false;
            }

            //Move Revan
            transform.position += dir * Time.deltaTime;
        }
    }

    private void Rewind()
    {
        animator.SetTrigger("startRewind");

        animator.SetBool("rewind", true);



        animator.SetBool("rewind", false);

        animator.SetTrigger("endRewind");
    }

    private void ReceivedDamage(float val)
    {
        Debug.Log("Revan received " + val + " damage!");
        animator.SetTrigger("receivesDamage");

        if (val <= 2 && isFirstPhase)
        {
            //It's rewind time!
            Rewind();
        }
    }

    private void Defeated(float val)
    {
        if (!isFirstPhase)
        {
            Debug.Log("Revan has been slayed!");
            animator.SetBool("defeated", true);
        }
    }
}
