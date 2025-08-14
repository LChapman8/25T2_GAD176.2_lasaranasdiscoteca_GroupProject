using System.Collections;
using System.Collections.Generic;
using Peekaboopro.EnemyAI;
using UnityEngine;
using UnityEngine.UIElements;

public class StationaryEnemyAI : BaseEnemyAI
{
    public bool rotateX = false;
    public bool rotateY = true;
    public bool rotateZ = false;

    public float speed = 50f;

    private void Update()
    {
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

