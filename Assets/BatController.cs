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
    public float scorePoints = 100;

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

        if (Game.current.IsRunning() && !Game.IsRewinding)
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
        if (Vector3.Distance(transform.position, targetPosition.position) < chaseRange)
        {
            state = EnemyState.ChaseTarget;
            return;
        }
        if (Time.time > sleepTime)
        {
            animator.SetBool("isMoving", false);
        }
    }

    private void ChaseTarget()
    {
        if (Game.IsRewinding) return;
        Vector3 dir = movement.Move(transform.position, targetPosition.position);
        if (dir != Vector3.zero)
        {
            animator.SetBool("isMoving", true);
            if (dir.x < 0 && !spriteRenderer.flipX)
            {
                GetComponent<ReTime>().AddKeyFrame(
                    g => g.GetComponent<SpriteRenderer>().flipX = true,
                    g => g.GetComponent<SpriteRenderer>().flipX = false
                );
            }
            else if (dir.x > 0 && spriteRenderer.flipX)
            {
                GetComponent<ReTime>().AddKeyFrame(
                    g => g.GetComponent<SpriteRenderer>().flipX = false,
                    g => g.GetComponent<SpriteRenderer>().flipX = true
                );
            }
        }

        // Move the bat
        transform.position += dir * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPosition.position) < attackRange)
        {
            //Player inside attack range
            state = EnemyState.AttackTarget;
        }
        else if (Vector3.Distance(transform.position, targetPosition.position) + distanceOffset > chaseRange)
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
        if (Game.IsRewinding) return;
        SoundPlayer.current.PlaySound(Sound.BAT_ATTACK);
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
        SoundPlayer.current.PlaySound(Sound.BAT_DAMAGE, transform);

        if (val <= 0) return;
        animator.SetTrigger("receivesDamage"); 
    }

    public void Defeated()
    {
        SoundPlayer.current.PlaySound(Sound.BAT_DEATH, transform);

        GetComponent<ReTime>().AddKeyFrame(
            g => g.GetComponent<BatController>().isDead = true,
            g => g.GetComponent<BatController>().isDead = false
        );
        Debug.Log("Bat has been slain");
        animator.SetBool("defeated", true);

        GetComponent<ReTime>().AddKeyFrame(g => ScoreManager.Instance.score += scorePoints, g => ScoreManager.Instance.score -= scorePoints);

    }

    public void BatDefeated()
    {
        if (Game.IsRewinding) return;
        animator.SetBool("defeated", false);
        GetComponent<ReTime>().AddKeyFrame(
            g => g.GetComponent<BatController>().batIsDead(),
            g => g.GetComponent<BatController>().batIsAlive()
        );
    }

    public void batIsDead()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<CircleCollider2D>().enabled = false;
    }

    public void batIsAlive()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        gameObject.GetComponent<CircleCollider2D>().enabled = true;
    }

}
