using System.IO;
using UnityEngine;
// Note this line, if it is left out, the script won't know that the class 'Path' exists and it will throw compiler errors
// This line should always be present at the top of scripts which use pathfinding
using Pathfinding;
using System.Collections;
using BasePatterns;
using UnityEngine.SocialPlatforms.Impl;

public class BatController : MonoBehaviour, IController
{
    private Seeker seeker;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private EnemyState state;
    public float initialHealth = 3;
    public float chaseRange = 1;
    public float attackRange = 0.1F;
    public float damage = 1;
    private float nextAttackTime;
    public float attackRate = 0.1F;
    public float scorePoints = 10;

    // values for a* algorithm
    public Transform targetPosition;
    public float speed = 0.5F;
    public float nextWaypointDistance = 0.2F;
    public float updatePathTime = 2;

    public float timeUntilSleeping = 2;
    private float sleepTime;
    public float distanceOffset = 2;

    private Movement movement;
    public Health health;


    public AudioSource TakeDamage;

    private CircleCollider2D attackCollider;

    Health IController.health { get => health; }

    public void Start()
    {
        seeker = GetComponent<Seeker>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        attackCollider = GetComponent<CircleCollider2D>();

        movement = new Movement(seeker, speed, nextWaypointDistance, updatePathTime);
        health = new Health(initialHealth, ReceivedDamage, Defeated);

        movement.PreCalcPath(transform.position, targetPosition.position);
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
        Vector3 dir = movement.Move(transform.position, targetPosition.position);
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
        attackCollider.enabled = true;
        StartCoroutine(EndAttack());
    }

    // Called at end of 'bat_attack' animation
    public IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(1);
        attackCollider.enabled = false;
        movement.UnlockMovement();
    }

    public void Defeated(float val)
    {
        TakeDamage.Play();
        Debug.Log("Bat has been slayed");
        animator.SetTrigger("defeated");

        GetComponent<ReTime>().AddKeyFrame(g => ScoreManager.Instance.score += scorePoints, g => ScoreManager.Instance.score -= scorePoints);
    }
    public void ReceivedDamage(float val)
    {
        TakeDamage.Play();
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
