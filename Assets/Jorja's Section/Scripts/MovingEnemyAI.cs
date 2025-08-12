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
        Patrol();

        if (transform.position == patrolPoints[targetPoint].position)
        {
            increaseTargetInt();
        }
    }

    void increaseTargetInt()
    {
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

        //Look at direction enemy is walking in
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        // Wait and patrol at each patrolPoint
        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            waiting = true;
            waitTimer = waitTimeAtWaypoint;
        }
    }
}
