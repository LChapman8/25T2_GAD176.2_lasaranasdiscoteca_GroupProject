using System.Collections;
using System.Collections.Generic;
using Peekaboopro.EnemyAI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MovingEnemyAI : BaseEnemyAI
{
    //Movement/patrol settings
    public Transform[] patrolPoints;
    public int targetPoint;
    public float speed;
    private int currentWaypointIndex = 0;
    private float waitTimer = 0f;
    private bool waiting = false;
    public float waitTimeAtWaypoint = 1f;

    private void Start()
    {
        targetPoint = 0;
    }

    private void Update()
    {
        //Telling the enemy to 'patrol' to each point
        Patrol();

        if (transform.position == patrolPoints[targetPoint].position)
        {
            increaseTargetInt();
        }
    }

    void increaseTargetInt()
    {
        //Lets the enemy know that it needs to go to mulitple points
        //This goes through the list of waypoints
        targetPoint++;
        if(targetPoint >= patrolPoints.Length)
        {
            targetPoint = 0;
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0)
            return;

        if (waiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                waiting = false;
                currentWaypointIndex = (currentWaypointIndex + 1) % patrolPoints.Length;
            }
            return;
        }

        Transform target = patrolPoints[currentWaypointIndex];
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        //Look at direction the enemy is moving in
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        //Make the enemy wait at each point
        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            waiting = true;
            waitTimer = waitTimeAtWaypoint;
        }
    }
}
