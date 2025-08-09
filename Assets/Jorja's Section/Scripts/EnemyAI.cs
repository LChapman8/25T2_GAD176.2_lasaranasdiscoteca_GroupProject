using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float hearingRange = 10f; // Distance within which the enemy can hear noise

    void OnEnable()
    {
        SoundManager.SoundEvent += OnHearNoise;
    }

    void OnDisable()
    {
        SoundManager.SoundEvent -= OnHearNoise;
    }

    void OnHearNoise(Vector3 noisePosition, float noiseRadius)
    {
        float distanceToNoise = Vector3.Distance(transform.position, noisePosition);

        // Consider noise radius in hearing range calculation
        if (distanceToNoise - noiseRadius <= hearingRange)
        {
            // Make the enemy look at the noise source
            transform.LookAt(noisePosition);
        }
    }
}
