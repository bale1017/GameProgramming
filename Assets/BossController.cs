using BasePatterns;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private float nextAttackTime;
    private int chooseAttackABC = 0;

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

    }

    private void AttackTarget()
    {

    }

    private void ChaseTarget()
    {

    }

    private void ReceivedDamage(float val)
    {

    }

    private void Defeated(float val)
    {

    }
}
