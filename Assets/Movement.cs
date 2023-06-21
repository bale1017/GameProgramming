using Pathfinding;
using UnityEngine;
using UnityEngine.Animations;

public class Movement
{
    private Seeker seeker;
    private Path path;
    private int currentWaypoint = 0;
    private float nextWaypointDistance;
    private float nextPathUpdate;
    private float updatePathTime;
    public float speed;
    private bool canMove = true;

    public Movement(Seeker _seeker, float _speed, float _nextWaypointDistance, float _updatePathTIme)
    {
        seeker = _seeker;
        speed = _speed;
        nextWaypointDistance = _nextWaypointDistance;
        nextPathUpdate = _updatePathTIme;
    }

    public void OnPathComplete(Path p)
    {
        //Debug.Log("A path was calculated. Did it fail with an error? " + p.error);

        if (!p.error)
        {
            path = p;
            // Reset the waypoint counter so that we start to move towards the first point in the path
            currentWaypoint = 0;
            nextPathUpdate = Time.time + updatePathTime;
        }
    }

    public void PreCalcPath(Vector3 currentPosition, Vector3 targetPosition)
    {
        seeker.StartPath(currentPosition, targetPosition, OnPathComplete);
    }

    public Vector3 Move(Vector3 currentPosition, Vector3 targetPosition)
    {
        if (canMove)
        {
            if (Time.time > nextPathUpdate)
            {
                seeker.StartPath(currentPosition, targetPosition, OnPathComplete);
            }

            // Check in a loop if we are close enough to the current waypoint to switch to the next one.
            // We do this in a loop because many waypoints might be close to each other and we may reach
            // several of them in the same frame.
            var reachedEndOfPath = false;
            // The distance to the next waypoint in the path
            float distanceToWaypoint;
            while (true)
            {
                // If you want maximum performance you can check the squared distance instead to get rid of a
                // square root calculation. But that is outside the scope of this tutorial.
                distanceToWaypoint = Vector3.Distance(currentPosition, path.vectorPath[currentWaypoint]);
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
            Vector3 dir = (path.vectorPath[currentWaypoint] - currentPosition).normalized;

            // Multiply the direction by our desired speed to get a velocity
            return dir * speed * speedFactor;
        } else
        {
            return Vector3.zero;
        }
    }

    public void LockMovement()
    {
        canMove = false;
    }

    public void UnlockMovement()
    {
        canMove = true;
    }

    public static Vector3 GetRandNextDestination(Vector3 originalPosition, float offset)
    {
        float randX = Random.Range(originalPosition.x - offset, originalPosition.x + offset);
        float randY = Random.Range(originalPosition.y - offset, originalPosition.y + offset);
        return new Vector3(randX, randY);
    }

    public static bool Equal(Vector3 a, Vector3 b, int precision = 3)
    {
        float aX = (float) System.Math.Round(a.x, precision);
        float aY = (float) System.Math.Round(a.y, precision);
        float bX = (float) System.Math.Round(b.x, precision);
        float bY = (float) System.Math.Round(b.y, precision);

        if (aX == bX && aY == bY) {
            return true;
        } else
        {
            return false;
        }
    }
}
