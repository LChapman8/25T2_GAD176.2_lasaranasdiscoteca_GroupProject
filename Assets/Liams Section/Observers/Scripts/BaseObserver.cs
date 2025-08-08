using System;
using UnityEngine;

namespace PeekabooPro.Observers
{
    /// <summary>
    /// This script is the base class for the observer, it contains all the functional knowledge for he observers to act the way that they need to 
    /// It contains all the logic for player detection / SFX / detection events 
    /// </summary>
    public abstract class BaseObserver : MonoBehaviour
    {
        // variables foe field of view / detection 
        [Header("field of view detection")]
        public float viewRadius = 10f;              
        [Range(0f, 360f)] public float viewAngle = 90f;  
        public float innerRadius = 3f;              
        // variable for layermask for obstacles 
        [Header("detection settings")]
        public LayerMask obstacleMask;              
        // variables for my audio sources
        [Header("audio")]
        public AudioSource detectionAudioSource;    
        public AudioSource alarmAudioSource;       
        // refernece to players transform and stealth state 
        protected Transform playerTransform;      
        protected PlayerStealthState playerStealthState;  
        // variables for detections and alarms 
        protected bool playerDetected = false;      
        protected bool innerAlarmTriggered = false;
        // my event systems for player detections and player lost
        public event Action OnPlayerDetectedEvent;  
        public event Action OnPlayerLostEvent;     

        // get references to player and stealth state on awake
        protected virtual void Awake()
        {
            playerTransform = FindObjectOfType<PlayerStealthState>()?.transform;
            playerStealthState = playerTransform?.GetComponent<PlayerStealthState>();

            if (detectionAudioSource == null)
                Debug.LogWarning($"{name}: no detection audio source assigned.");
            if (alarmAudioSource == null)
                Debug.LogWarning($"{name}: no alarm audio source assigned.");
        }

        // check detection state each frame and trigger events/audio
        protected virtual void Update()
        {
            if (playerTransform == null || playerStealthState == null)
                return;

            bool detected = CheckPlayerDetection();

            if (detected && !playerDetected)
            {
                playerDetected = true;
                innerAlarmTriggered = false; 
                OnPlayerDetected();
            }
            else if (!detected && playerDetected)
            {
                playerDetected = false;
                innerAlarmTriggered = false;
                OnPlayerLost();
            }

            // if player detected but inner alarm not triggered yet, check inner radius
            if (playerDetected && !innerAlarmTriggered)
            {
                float distance = Vector3.Distance(transform.position, playerTransform.position);
                if (distance <= innerRadius && !Physics.Raycast(transform.position, (playerTransform.position - transform.position).normalized, distance, obstacleMask))
                {
                    innerAlarmTriggered = true;
                    OnInnerRadiusTriggered();
                }
            }
        }

        // check if player is inside detection cone or inner radius (with line of sight)
        protected virtual bool CheckPlayerDetection()
        {
            Vector3 dirToPlayer = (playerTransform.position - transform.position).normalized;
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            // detect player inside inner radius if no obstacle
            if (distanceToPlayer <= innerRadius)
            {
                if (!Physics.Raycast(transform.position, dirToPlayer, distanceToPlayer, obstacleMask))
                    return true;
            }

            // detect player in cone only if not stealthed and no obstacle
            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2f && distanceToPlayer <= viewRadius)
            {
                if (!Physics.Raycast(transform.position, dirToPlayer, distanceToPlayer, obstacleMask))
                {
                    if (!playerStealthState.IsStealthed)
                        return true;
                }
            }

            return false;
        }

        // called when player first detected (outer radius)
        protected virtual void OnPlayerDetected()
        {
            Debug.Log($"{name}: player detected (outer radius).");
            OnPlayerDetectedEvent?.Invoke();

            if (detectionAudioSource && !detectionAudioSource.isPlaying)
                detectionAudioSource.Play();
        }

        // called when player enters inner radius (alarm triggers)
        protected virtual void OnInnerRadiusTriggered()
        {
            Debug.Log($"{name}: player entered inner radius! alarm activated.");
            if (alarmAudioSource && !alarmAudioSource.isPlaying)
                alarmAudioSource.Play();
        }

        // called when player lost from detection
        protected virtual void OnPlayerLost()
        {
            Debug.Log($"{name}: player lost.");
            OnPlayerLostEvent?.Invoke();

            if (detectionAudioSource && detectionAudioSource.isPlaying)
                detectionAudioSource.Stop();

            if (alarmAudioSource && alarmAudioSource.isPlaying)
                alarmAudioSource.Stop();
        }

        // draw gizmos to visualize detection radius and cone
        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, viewRadius);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, innerRadius);

            // use false to get direction relative to object rotation
            Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
            Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
            Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);
        }

        // helper to get direction vector from an angle
        protected Vector3 DirFromAngle(float angleInDegrees, bool global)
        {
            if (!global)
                angleInDegrees += transform.eulerAngles.y;
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
    }
}
