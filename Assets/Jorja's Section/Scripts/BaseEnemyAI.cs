using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Peekaboopro.EnemyAI
{

    public class BaseEnemyAI : MonoBehaviour
    {
        //The distance/range and angle the enemy can see the player
        public float viewRadius;
        public float viewAngle;

        //The player and obstacle layers
        //If player behind an object/obstacle, enemy will not be able to see the player
        public LayerMask targetPlayer;
        public LayerMask obstacleMask;

        public GameObject player;

        public float hearingRange = 10f; // Distance within which the enemy can hear noise

        protected bool reactingToNoise = false; // If the enemy is reacting to a sound
        protected float noiseReactTimer = 0f;   // How long the enemy should react
        protected Vector3 noiseLookPosition;    // Where the noise came from

        // Event so child classes know when reaction ends
        protected bool finishedReacting = false;

        private void Update()
        {
            // If the enemy is reacting to noise, smoothly rotate to look at the noise position
            if (reactingToNoise)
            {
                Vector3 dir = (noiseLookPosition - transform.position).normalized;
                dir.y = 0; // keep only horizontal rotation
                if (dir != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3f);
                }

                noiseReactTimer -= Time.deltaTime;
                if (noiseReactTimer <= 0f)
                {
                    reactingToNoise = false;
                    finishedReacting = true; // tell children we just finished reacting
                }
                return; // Skip normal vision checks while reacting to noise
            }

            Vector3 playerTarget = (player.transform.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, playerTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, player.transform.position);
                if (distanceToTarget <= viewRadius)
                {
                    if (Physics.Raycast(transform.position, playerTarget, distanceToTarget, obstacleMask) == false)
                    {
                        Debug.Log("Oh no! The enemy has seen you!");

                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    }
                }
            }
        }

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
                // Make the enemy look at the noise source for 3 seconds
                noiseLookPosition = noisePosition;
                reactingToNoise = true;
                noiseReactTimer = 3f;
                finishedReacting = false;
            }
        }
    }
}

