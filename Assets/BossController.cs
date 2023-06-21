using BasePatterns;
using Pathfinding;
using UnityEngine;
using System;
using System.Collections;

public class BossController : MonoBehaviour, IController
{
    private Seeker seeker;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // generic values and values for state machine
    private EnemyState state;
    public float initialHealth = 30;
    public float chaseRange = 1000;
    public float attackRangeX = 0.45F;
    public float attackRangeY = 0.1F;
    public float damage = 6;
    public float timeToNextAttack = 2;
    public float attackAnimationSpeed = 0.9F;

    private bool isFirstPhase = true;
    private float nextAttackTime;
    private int randAttack;

    // values for a* algorithm
    public Transform targetPosition;
    public float speed = 0.7F;
    public float nextWaypointDistance = 0.2F;
    public float updatePathTime = 2;

    private Movement movement;
    public Health health;
    public RevanSwordAttackA attackA;
    public RevanSwordAttackB attackB;
    public RevanSwordAttackC attackC;

    Health IController.health { get => health; }

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        movement = new Movement(seeker, speed, nextWaypointDistance, updatePathTime);
        health = new Health(initialHealth, ReceivedDamage, Defeated);

        animator.SetFloat("attackAnimationSpeed", attackAnimationSpeed);
        movement.PreCalcPath(transform.position, targetPosition.position);
        state = EnemyState.Idle;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Game.current.IsRunning()) { 
            if (!Game.IsRewinding)
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
        }
    }

    private void Idle()
    {
        //Debug.Log("Revan in Idle State");
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
        //Debug.Log("Revan in Chase State");
        Vector3 dir = movement.Move(transform.position, PlayerController.Instance.GetPosition());
        //Debug.Log(dir);
        Move(dir);
        if (Math.Abs(transform.position.x - PlayerController.Instance.GetPosition().x) < attackRangeX &&
            Math.Abs(transform.position.y - PlayerController.Instance.GetPosition().y) < attackRangeY)
        {
            //Player inside attack range
            state = EnemyState.AttackTarget;
        }
    }

    private void AttackTarget()
    {
        //Debug.Log("Revan in Attack State");
        animator.SetBool("isMoving", false);

        //Player within atack range
        if (Time.time > nextAttackTime)
        {
            randAttack = UnityEngine.Random.Range(0, 3); //get random int [0, 1, 2] -> 1/3 chance for each
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

            nextAttackTime = Time.time + timeToNextAttack;
        }
        if (Math.Abs(transform.position.x - PlayerController.Instance.GetPosition().x) >= attackRangeX ||
            Math.Abs(transform.position.y - PlayerController.Instance.GetPosition().y) >= attackRangeY)
        {
            state = EnemyState.Idle;
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

    public void Attack()
    {
        movement.LockMovement();
        if (randAttack == 0)
        {
            if (spriteRenderer.flipX == true)
            {
                attackA.AttackLeft();
            } else
            {
                attackA.AttackRight();
            }
        } else if (randAttack == 1)
        {
            if (spriteRenderer.flipX == true)
            {
                attackB.AttackLeft();
            }
            else
            {
                attackB.AttackRight();
            }
        } else
        {
            if (spriteRenderer.flipX == true)
            {
                attackC.AttackLeft();
            }
            else
            {
                attackC.AttackRight();
            }
        }
        StartCoroutine(EndAttack());
    }

    public IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("Revan ends his attack");
        movement.UnlockMovement();
        attackA.StopAttack();
        attackB.StopAttack();
        attackC.StopAttack();
    }

    private void Rewind()
    {
        animator.SetTrigger("startRewind");

        animator.SetBool("rewind", true);

        //TODO add rewinding logic
        isFirstPhase = false;

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
