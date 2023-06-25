using BasePatterns;
using Pathfinding;
using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    private Seeker seeker;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // generic values and values for state machine
    private EnemyState state;
    public float chaseRange = 1000;
    public float attackRangeX = 0.45F;
    public float attackRangeY = 0.1F;
    public float damage = 6;
    public float timeToNextAttack = 2;
    public float attackAnimationSpeed = 0.8F;
    public float rewindTimeInSec = 3;

    private bool isFirstPhase = true;
    private float nextAttackTime;
    private int randAttack;
    private bool activatedUI = false;
    private bool revanIsRewinding = false;

    // values for a* algorithm
    private Transform targetPosition;
    public float speed = 0.6F;
    public float nextWaypointDistance = 0.2F;
    public float updatePathTime = 2;

    private Health health;
    private Movement movement;
    public RevanSwordAttack attackA;
    public RevanSwordAttack attackB;
    public RevanSwordAttack attackC;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        movement = new Movement(seeker, speed, nextWaypointDistance, updatePathTime);
        health = GetComponent<Health>();
        if (health == null)
        {
            health = gameObject.AddComponent<Health>();
        }
        health.OnDeath.AddListener(Defeated);
        health.OnHealthDecreaseBy.AddListener(ReceivedDamage);

        animator.SetFloat("attackAnimationSpeed", attackAnimationSpeed);
        movement.PreCalcPath(transform.position, transform.position);
        state = EnemyState.Idle;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GameObject[] possibleTargets = GameObject.FindGameObjectsWithTag("Player");
        GameObject closestTarget = null;
        float distance = float.MaxValue;
        foreach (var possibleTarget in possibleTargets)
        {
            if (closestTarget == null || distance > Vector3.Distance(transform.position, possibleTarget.transform.position))
            {
                distance = Vector3.Distance(transform.position, possibleTarget.transform.position);
                closestTarget = possibleTarget;
            }
        }
        targetPosition = closestTarget.transform;

        if (!activatedUI)
        {
            activatedUI = true;
            Game.current.activateBossUI();
        }

        if (Game.current.IsRunning()) { 
            if (!Game.IsRewinding && !revanIsRewinding)
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
        if (GetComponent<ReTime>().isRewinding) return;
        if (!Movement.Equal(dir, Vector3.zero))
        {
            animator.SetBool("isMoving", true);

            //switch direction depending of the position of the next waypoint
            if (dir.x < 0)
            {
                GetComponent<ReTime>().AddKeyFrame(
                    g => spriteRenderer.flipX = true,
                    g => spriteRenderer.flipX = false
                );
            }
            else if (dir.x > 0)
            {
                GetComponent<ReTime>().AddKeyFrame(
                    g => spriteRenderer.flipX = false,
                    g => spriteRenderer.flipX = true
                );
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
        movement.UnlockMovement();
        attackA.StopAttack();
        attackB.StopAttack();
        attackC.StopAttack();
    }

    private IEnumerator Rewind()
    {
        revanIsRewinding = true;
        health.MakeInvulnerable();
        animator.SetTrigger("startRewind");
        animator.SetBool("isRewinding", true);
        yield return new WaitForSeconds(1);

        Game.current.StartRewind();
        isFirstPhase = false;
        yield return new WaitForSeconds(rewindTimeInSec);

        Game.current.StopRewind();
        animator.SetBool("isRewinding", false);
        health.MakeVulnerable();
        revanIsRewinding = false;
    }

    private void ReceivedDamage(float val)
    {
        Debug.Log("Revan received " + val + " damage!");
        animator.SetTrigger("receivesDamage");
    }

    private void Defeated()
    {
        if (!isFirstPhase)
        {
            Debug.Log("Revan has been slayed!");
            animator.SetBool("defeated", true);
        } else
        {
            //It's rewind time!
            StartCoroutine(Rewind());
        }
    }
}
