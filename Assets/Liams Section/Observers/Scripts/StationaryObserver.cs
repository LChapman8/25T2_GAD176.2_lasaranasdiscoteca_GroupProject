using PeekabooPro.Observers;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This script is responsible for managing the specific behaviour of the stationary observer
/// its a child of the base observer. 
/// </summary>

public class StationaryObserver : BaseObserver
{
    // variables for rotation/scanning 
    [Header("scanning rotation")]
    public bool enableScanning = true;
    public float rotationSpeed = 30f;

    // variables for movespead and alarm delay 
    [Header("chase settings")]
    public float moveSpeed = 2f;
    public float alarmDelay = 2f;

    // chase acceleration settings
    [Header("chase acceleration")]
    public float acceleration = 1f;         
    public float maxChaseSpeed = 10f;        
    private float currentSpeed;               

    // variable for chasing 
    private bool isChasing = false;
    // reference to rigid body for accelration
    private Rigidbody rb;                     

    // subscribe to events on awake
    protected override void Awake()
    {
        base.Awake();
        OnPlayerDetectedEvent += HandlePlayerDetected;
        OnPlayerLostEvent += HandlePlayerLost;
        rb = GetComponent<Rigidbody>();

        if (rb == null)
            Debug.LogError($"{name}: No Rigidbody found! Please add one for physics movement.");

        // make sure Rigidbody is set up for physics based movement
        if (rb != null)
        {
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }

    // update rotation and chasing movement
    protected override void Update()
    {
        base.Update();

        // rotate while scanning and not chasing or detecting player
        if (enableScanning && !playerDetected && !isChasing)
        {
            transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
        }

        // chase player if detected
        if (isChasing && playerTransform != null)
        {
            MoveTowardPlayer();
        }
    }

    // move towards player position smoothly with acceleration
    private void MoveTowardPlayer()
    {
        // increase current speed using acceleration each frame until reaching maxChaseSpeed
        currentSpeed += acceleration * Time.deltaTime;
        currentSpeed = Mathf.Clamp(currentSpeed, moveSpeed, maxChaseSpeed);

        // physics based movement
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Vector3 targetPosition = rb.position + direction * currentSpeed * Time.deltaTime;
        rb.MovePosition(targetPosition);

        // smoothly rotate towards player
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, Time.deltaTime * 5f));
    }

    // called when player detected, start chasing
    private void HandlePlayerDetected()
    {
        if (playerTransform == null)
            playerTransform = playerStealthState.transform;

        isChasing = true;
        currentSpeed = moveSpeed; // start from patrol speed
        Debug.Log($"{name}: begin chase.");
    }

    // called when player lost, stop chasing and reset target
    private void HandlePlayerLost()
    {
        Debug.Log($"{name}: lost player. returning to scan.");
        isChasing = false;
        currentSpeed = moveSpeed; // reset speed
    }

    // override alarm trigger to add delay before reloading scene
    protected override void OnInnerRadiusTriggered()
    {
        base.OnInnerRadiusTriggered();
        Debug.Log($"{name}: inner radius triggered. scene will reload in {alarmDelay}s.");
        Invoke(nameof(TriggerAlarm), alarmDelay);
    }

    // reload current scene to reset game
    private void TriggerAlarm()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
