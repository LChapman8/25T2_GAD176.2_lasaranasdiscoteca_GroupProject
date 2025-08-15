using System.Collections;
using System.Collections.Generic;
using Peekaboopro.EnemyAI;
using UnityEngine;
using UnityEngine.UIElements;

public class RotatingEnemyAI : BaseEnemyAI
{
    public bool rotateX = false;
    public bool rotateY = true;
    public bool rotateZ = false;

    public float speed = 50f;

    private void Update()
    {

        // Handle reacting to noise first
        if (reactingToNoise)
        {
            Vector3 lookDir = (noiseLookPosition - transform.position).normalized;
            if (lookDir != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }

            noiseReactTimer -= Time.deltaTime;
            if (noiseReactTimer <= 0f)
            {
                reactingToNoise = false;
                finishedReacting = true;
            }

            return; // skip rotation while reacting
        }

        // If just finished reacting, reset flag
        if (finishedReacting)
        {
            finishedReacting = false;
        }

        //Rotates the enemy while its standing still
        Vector3 rotation = Vector3.zero;
        if (rotateX)
        {
            rotation += Vector3.right;
        }
        if (rotateY)
        {
            rotation += Vector3.up;
        }
        if (rotateZ)
        {
            rotation += Vector3.forward;
        }

        transform.Rotate(rotation * speed * Time.deltaTime);
    }
}

