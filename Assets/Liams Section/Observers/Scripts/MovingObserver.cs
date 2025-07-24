using PeekabooPro.Observers;
using UnityEngine;

public class MovingObserver : BaseObserver
{
    // variables for the waypoint movement system
    [Header("Movement Settings")]
    public Transform[] waypoints;
    public float moveSpeed = 2f;
    public float waitTimeAtWaypoint = 1f;

    private int currentWaypointIndex = 0;
    private float waitTimer = 0f;
    private bool waiting = false;

    // on update move between waypoints based on time
    void Update()
    {
        base.Update();

        if (waypoints.Length == 0)
            return;

        if (waiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                waiting = false;
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            }
            return;
        }

        MoveToWaypoint();
    }
    // a function for moving between waypoints 
    void MoveToWaypoint()
    {
        Transform target = waypoints[currentWaypointIndex];
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        //  rotate to face direction of movement
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < 0.2f)
        {
            waiting = true;
            waitTimer = waitTimeAtWaypoint;
        }
    }
}
