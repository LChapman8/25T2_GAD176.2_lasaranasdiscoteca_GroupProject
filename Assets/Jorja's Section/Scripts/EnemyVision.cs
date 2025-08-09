using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    //The distance/range and angle the enemy can see the player
    public float viewRadius;
    public float viewAngle;

    //The player and obstacle layers
    //If player behind an object/obstacle, enemy will not be able to see the player
    public LayerMask targetPlayer;
    public LayerMask obstacleMask;

    public GameObject player;

    private void Update()
    {
        Vector3 playerTarget = (player.transform.position - transform.position).normalized;

        if (Vector3.Angle(transform.forward, playerTarget) < viewAngle / 2)
        {
            float distanceToTarget = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToTarget <= viewRadius)
            {
                if(Physics.Raycast(transform.position, playerTarget, distanceToTarget, obstacleMask) == false)
                {
                    Debug.Log("Oh no! The enemy has seen you!");
                }
            }
        }
    }
}
