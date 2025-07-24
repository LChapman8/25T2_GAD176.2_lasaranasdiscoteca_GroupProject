using System;
using UnityEngine;

namespace PeekabooPro.Observers
{
    public abstract class BaseObserver : MonoBehaviour
    {
        // variables for detection range of non stealth and stealth
        [Header("Detection Ranges")]
        public float largeDetectionRadius = 10f;
        public float smallDetectionRadius = 3f;
        //reference to the players layermask 
        [Header("Detection Settings")]
        public LayerMask playerLayerMask;
        // reference for the alarm audio 
        [Header("Audio")]
        public AudioSource alarmAudioSource;
        // VFX for the stealth detection zone 
        [Header("Visuals")]
        public GameObject stealthDetectionVFXPrefab;
        // reference to player transform and stealth state 
        protected Transform playerTransform;
        protected PlayerStealthState playerStealthState;
        //variables for player detected 
        protected bool playerDetected = false;

        protected GameObject stealthVFXInstance;
        //events for player detection and player lost 
        public event Action OnPlayerDetectedEvent;
        public event Action OnPlayerLostEvent;

        
        protected virtual void Awake()
        {
            // on awake grab player transform
            playerTransform = FindObjectOfType<PlayerStealthState>()?.transform;
            playerStealthState = playerTransform?.GetComponent<PlayerStealthState>();
            // make sure there is an audio source 
            if (alarmAudioSource == null)
            {
                Debug.LogWarning($"{name}: No AudioSource assigned for alarm.");
            }

            // instantiate VFX but disable initially
            if (stealthDetectionVFXPrefab != null)
            {
                stealthVFXInstance = Instantiate(stealthDetectionVFXPrefab, transform);
                stealthVFXInstance.transform.localPosition = Vector3.zero;
                stealthVFXInstance.transform.localScale = Vector3.one * smallDetectionRadius * 2f; // diameter
                stealthVFXInstance.SetActive(false);
            }
        }

        protected virtual void Update()
        {
            if (playerTransform == null || playerStealthState == null)
                return;

            // show/hide VFX depending on player stealth state
            if (stealthVFXInstance != null)
            {
                stealthVFXInstance.SetActive(playerStealthState.IsStealthed());
            }

            bool detected = CheckPlayerDetection();

            if (detected && !playerDetected)
            {
                playerDetected = true;
                OnPlayerDetected();
            }
            else if (!detected && playerDetected)
            {
                playerDetected = false;
                OnPlayerLost();
            }
        }
        // function for checking if the players been detected 
        protected virtual bool CheckPlayerDetection()
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);

            if (distance <= smallDetectionRadius && playerStealthState.IsStealthed())
                return true;

            if (distance <= largeDetectionRadius && !playerStealthState.IsStealthed())
                return true;

            return false;
        }
        // function for what to do if the player is detected 
        protected virtual void OnPlayerDetected()
        {
            Debug.Log($"{name}: Player detected!");

            OnPlayerDetectedEvent?.Invoke();

            if (alarmAudioSource != null && !alarmAudioSource.isPlaying)
                alarmAudioSource.Play();
        }
        // function for what to do if the player is lost 
        protected virtual void OnPlayerLost()
        {
            Debug.Log($"{name}: Player lost.");

            OnPlayerLostEvent?.Invoke();

            if (alarmAudioSource != null && alarmAudioSource.isPlaying)
                alarmAudioSource.Stop();
        }
        // gizmo for visual in editor of the detectin zones with alternate colours 
        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
            Gizmos.DrawSphere(transform.position, largeDetectionRadius);

            Gizmos.color = new Color(0f, 0.4f, 1f, 0.4f);
            Gizmos.DrawSphere(transform.position, smallDetectionRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, largeDetectionRadius);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, smallDetectionRadius);
        }
    }
}
