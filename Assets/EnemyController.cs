using System.IO;
using UnityEngine;
// Note this line, if it is left out, the script won't know that the class 'Path' exists and it will throw compiler errors
// This line should always be present at the top of scripts which use pathfinding
using Pathfinding;
using System;

public class EnemyController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private enum State
    {
        Idle,
        ChaseTarget
    }
    private State state;
    public float targetRange = 1;

    // values for a* algorithm
    public Transform targetPosition;
    private Seeker seeker;
    public Pathfinding.Path path;
    public float speed = 0.5F;
    public float nextWaypointDistance = 0.2F;
    private int currentWaypoint = 0;
    public bool reachedEndOfPath;
    public float updatePathTime = 2;
    private float nextPathUpdate;

    // values for fight system
    public float health = 1;
    public float Health
    {
        set
        {
            print("health is changed");
            health = value;
            if (health <= 0)
            {
                Defeated();
            }
        }
        get
        {
            return health;
        }
    }

    public void Start()
    {
        seeker = GetComponent<Seeker>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Start a new path to the targetPosition, call the the OnPathComplete function
        // when the path has been calculated (which may take a few frames depending on the complexity)
        //seeker.StartPath(transform.position, targetPosition.position, OnPathComplete);
        //nextPathUpdate = Time.time + updatePathTime;
        state = State.Idle;
    }

    public void OnPathComplete(Pathfinding.Path p)
    {
        Debug.Log("A path was calculated. Did it fail with an error? " + p.error);

        if (!p.error)
        {
            path = p;
            // Reset the waypoint counter so that we start to move towards the first point in the path
            currentWaypoint = 0;
            nextPathUpdate = Time.time + updatePathTime;
        }
    }

    public void FixedUpdate()
    {
        switch (state)
        {
            default:
            case State.Idle:
                findTarget();
                break;

            case State.ChaseTarget:
                if (path == null || Time.time > nextPathUpdate)
                {
                    // We have no path to follow yet, so don't do anything
                    //return;
                    seeker.StartPath(transform.position, targetPosition.position, OnPathComplete);
                }

                // Check in a loop if we are close enough to the current waypoint to switch to the next one.
                // We do this in a loop because many waypoints might be close to each other and we may reach
                // several of them in the same frame.
                reachedEndOfPath = false;
                // The distance to the next waypoint in the path
                float distanceToWaypoint;
                while (true)
                {
                    // If you want maximum performance you can check the squared distance instead to get rid of a
                    // square root calculation. But that is outside the scope of this tutorial.
                    distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
                    if (distanceToWaypoint < nextWaypointDistance)
                    {
                        // Check if there is another waypoint or if we have reached the end of the path
                        if (currentWaypoint + 1 < path.vectorPath.Count)
                        {
                            currentWaypoint++;
                        }
                        else
                        {
                            // Set a status variable to indicate that the agent has reached the end of the path.
                            // You can use this to trigger some special code if your game requires that.
                            reachedEndOfPath = true;
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                // Slow down smoothly upon approaching the end of the path
                // This value will smoothly go from 1 to 0 as the agent approaches the last waypoint in the path.
                var speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint / nextWaypointDistance) : 1f;

                // Direction to the next waypoint
                // Normalize it so that it has a length of 1 world unit
                Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
                Debug.Log(dir.x);
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
                // Multiply the direction by our desired speed to get a velocity
                Vector3 velocity = dir * speed * speedFactor;

                // Move the agent
                transform.position += velocity * Time.deltaTime;
                break;
        }
        
    }

    private void findTarget()
    {
        if (Vector3.Distance(transform.position, PlayerController.Instance.GetPosition()) < targetRange)
        {
            //Player within target range
            state = State.ChaseTarget;
        }
    }

    //private Vector3 GetRoamingPosititon()
    //{
    //    return startingPosition + GetRandomDir() * Random.Range(10f, 70f);
    //}

    //private static Vector3 GetRandomDir()
    //{
    //    return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    //}

    public void Defeated()
    {
        animator.SetTrigger("Defeated");
    }
    public void RemoveEnemy()
    {
        Destroy(gameObject);
    }
}
