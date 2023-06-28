using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasePatterns;

public class SkeletonSwordWarriorController : MonoBehaviour
{
    private Seeker seeker;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // generic values and values for state machine
    private EnemyState state;
    public float chaseRange = 1;
    public float attackRange = 0.1F;
    public float damage = 5;
    public float attackRate = 0.1F;

    private float nextAttackTime;
    private bool chooseAttackA = true;

    // values for a* algorithm
    private Transform targetPosition;
    public float speed = 0.2F;
    public float nextWaypointDistance = 0.2F;
    public float updatePathTime = 2;

    // values for roaming
    private Vector3 startPosition;
    private Vector3 randNextDestination;
    private float roamingTime;
    public float roamingOffset = 1F;
    public float timeUntilNextDestination = 3;
    private float returnTime;
    public float timeUntilReturning = 2;
    public float distanceOffset = 2;

    private Movement movement;
    public SkeletonSword skeletonSword;

    public void Start()
    {
        seeker = GetComponent<Seeker>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        movement = new Movement(seeker, speed, nextWaypointDistance, updatePathTime);
        Health health = GetComponent<Health>();
        if (health != null)
        {
            health.OnDeath.AddListener(Defeated);
            health.OnHealthDecreaseBy.AddListener(ReceivedDamage);
        }

        startPosition = transform.position;
        randNextDestination = Movement.GetRandNextDestination(startPosition, roamingOffset);
        movement.PreCalcPath(transform.position, randNextDestination);
        roamingTime = Time.time + timeUntilNextDestination;
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

                    case EnemyState.Roaming:
                        Roaming();
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
            //TODO implement enemy specific behaviour
            animator.SetBool("isMoving", false);
            if (Time.time <= returnTime)
            {
                state = EnemyState.Idle;
            } else
            {
                state = EnemyState.Roaming;
            }
        }
    }

    public void Roaming()
    {
        if (Vector3.Distance(transform.position, PlayerController.Instance.GetPosition()) < chaseRange)
        {
            //Player within target range
            state = EnemyState.ChaseTarget;
        } else
        {
            //Debug.Log(transform.position.ToString("F8"));
            //Debug.Log(randNextDestination.ToString("F8"));
            //Debug.Log(transform.position == randNextDestination);
            //Debug.Log(Movement.Equal(transform.position, randNextDestination));

            movement.speed = 0.2F;
            animator.SetFloat("movementSpeed", 0.4F);
            if (Movement.Equal(transform.position, randNextDestination) || Time.time > roamingTime)
            {
                roamingTime = Time.time + timeUntilNextDestination;
                randNextDestination = Movement.GetRandNextDestination(startPosition, roamingOffset);
            }
            Vector3 dir = movement.Move(transform.position, randNextDestination);
            Move(dir);
        }
    }

    private void ChaseTarget()
    {
        movement.speed = 0.5F;
        animator.SetFloat("movementSpeed", 0.6F);
        Vector3 dir = movement.Move(transform.position, PlayerController.Instance.GetPosition());
        Move(dir);

        if (Vector3.Distance(transform.position, PlayerController.Instance.GetPosition()) < attackRange)
        {
            //Player inside attack range
            state = EnemyState.AttackTarget;
        }
        else if (Vector3.Distance(transform.position, PlayerController.Instance.GetPosition()) + distanceOffset > chaseRange)
        {
            //Player outside of target range
            returnTime = Time.time + timeUntilReturning;
            state = EnemyState.Idle;
        }
    }

    private void AttackTarget()
    {
        //Player within attack range
        if (Time.time > nextAttackTime)
        {
            if (chooseAttackA)
            {
                //use attack A
                SoundPlayer.current.PlaySound(Sound.SKELETON_SWORD_SLASH_FAST);
                animator.SetTrigger("isAttackingA");
                chooseAttackA = false;
            }
            else
            {
                //use attack B
                SoundPlayer.current.PlaySound(Sound.SKELETON_SWORD_SLASH_FAST);
                animator.SetTrigger("isAttackingB");
                chooseAttackA = true;
            }

            nextAttackTime = Time.time + attackRate;

            if (Vector3.Distance(transform.position, PlayerController.Instance.GetPosition()) + distanceOffset > chaseRange)
            {
                //Player outside of target range
                returnTime = Time.time + timeUntilReturning;
                state = EnemyState.Idle;
            }
        }
    }

    private void Move(Vector3 dir)
    {
        if (GetComponent<ReTime>().isRewinding) return;
        if (!Movement.Equal(dir, Vector3.zero))
        {
            //Debug.Log("Try to move");
            animator.SetBool("isMoving", true);

            //switch direction depending of the position of the next waypoint
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

            //Move the Skeleton Sword Warrior
            transform.position += dir * Time.deltaTime;
        }
    }

    // Called at begin of attack animation
    public void Attack()
    {
        movement.LockMovement();
        if (spriteRenderer.flipX == true)
        {
            skeletonSword.AttackLeft();
        } else
        {
            skeletonSword.AttackRight();
        }
        StartCoroutine(EndAttack());
    }

    // Called at end of attack animation
    public IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(1);
        skeletonSword.StopAttack();
        movement.UnlockMovement();
    }

    public void Defeated()
    {
        Debug.Log("Skeleton Sword Warrior has been slayed!");
        SoundPlayer.current.PlaySound(Sound.SKELETON_DEATH);
        animator.SetBool("defeated", true);
    }

    public void ReceivedDamage(float val)
    {
        Debug.Log("Skeleton Sword Warrior received " + val + " damage!");
        SoundPlayer.current.PlaySound(Sound.SKELETON_DAMAGE);
        animator.SetTrigger("receivesDamage");
    }

    public void RemoveEnemy()
    { // called from inside "death"-animation
        //Destroy(gameObject);
        gameObject.SetActive(false);
    }
}
