using System.IO;
using UnityEngine;
// Note this line, if it is left out, the script won't know that the class 'Path' exists and it will throw compiler errors
// This line should always be present at the top of scripts which use pathfinding
using Pathfinding;
using System.Collections;
using BasePatterns;

public class BatController : MonoBehaviour
{
    private Animator animator;
    private Seeker seeker;
    private SpriteRenderer spriteRenderer;

    private EnemyState state;
    public float chaseRange = 1;
    public float attackRange = 0.1F;
    private float nextAttackTime;
    public float attackRate = 0.1F;
    private bool isDead = false;

    // values for a* algorithm
    private Transform targetPosition;
    public float speed = 0.5F;
    public float nextWaypointDistance = 0.2F;
    public float updatePathTime = 2;

    public float timeUntilSleeping = 2;
    private float sleepTime;
    public float distanceOffset = 2;

    public AudioSource TakeDamage;
    public BatAttack batAttack;    
    private Movement movement;

    public void Start()
    {
        seeker = GetComponent<Seeker>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        TakeDamage = GetComponentInChildren<AudioSource>();

        movement = new Movement(seeker, speed, nextWaypointDistance, updatePathTime);
        Health health = GetComponent<Health>();
        if (health != null)
        {
            health.OnDeath.AddListener(Defeated);
            health.OnHealthDecreaseBy.AddListener(ReceivedDamage);
        }

        movement.PreCalcPath(transform.position, transform.position);
        state = EnemyState.Idle;
    }

    public void FixedUpdate()
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

        if (Game.current.IsRunning())
        {
            if (!isDead && !Game.IsRewinding)
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
        if (Vector3.Distance(transform.position, PlayerController.Instance.GetPosition()) < chaseRange)
        {
            //Player within target range
            state = EnemyState.ChaseTarget;
        }
        else
        {
            if (Time.time > sleepTime)
            {
                animator.SetBool("isMoving", false);
            }
        }
    }

    private void ChaseTarget()
    {
        if (GetComponent<ReTime>().isRewinding) return;
        Vector3 dir = movement.Move(transform.position, targetPosition.position);
        if (dir != Vector3.zero)
        {
            animator.SetBool("isMoving", true);
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
        }

        // Move the bat
        transform.position += dir * Time.deltaTime;

        if (Vector3.Distance(transform.position, PlayerController.Instance.GetPosition()) < attackRange)
        {
            //Player inside attack range
            state = EnemyState.AttackTarget;
        }
        else if (Vector3.Distance(transform.position, PlayerController.Instance.GetPosition()) + distanceOffset > chaseRange)
        {
            //Player outside of target range
            sleepTime = Time.time + timeUntilSleeping;
            state = EnemyState.Idle;
        }
    }

    private void AttackTarget()
    {
        //Player within attack range
        if (Time.time > nextAttackTime)
        {
            animator.SetTrigger("isAttacking");
            nextAttackTime = Time.time + attackRate;
            state = EnemyState.ChaseTarget;
        }
    }

    // Called at begin of 'bat_attack' animation
    public void Attack()
    {
        movement.LockMovement();
        if (spriteRenderer.flipX == true)
        {
            batAttack.AttackLeft();
        } else
        {
            batAttack.AttackRight();
        }
        StartCoroutine(EndAttack());
    }

    public IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(1);
        batAttack.StopAttack();
        movement.UnlockMovement();
    }

    public void ReceivedDamage(float val)
    {
        if (val <= 0) return;
        TakeDamage.Play();
        Debug.Log("Bat received damage, its new health is: " + val);
        animator.SetTrigger("receivesDamage"); 
    }

    public void Defeated()
    {
        isDead = true;
        Debug.Log("Bat has been slayed");
        TakeDamage.Play();
        animator.SetBool("defeated", true);
    }

    public void BatDefeated()
    {
        GetComponent<ReTime>().AddKeyFrame(
            g => g.GetComponent<BatController>().batIsDead(),
            g => g.GetComponent<BatController>().batIsAlive()
        );
    }

    public void batIsDead()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<CircleCollider2D>().enabled = false;
        animator.SetBool("defeated", false);
    }

    public void batIsAlive()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        gameObject.GetComponent<CircleCollider2D>().enabled = true;
        isDead = false;
    }

}
