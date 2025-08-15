using PeekabooPro.Observers;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// this script is responsible for managing the specific behaviour of the moving observer
/// its a child of the base observer.
/// </summary>

public class MovingObserver : BaseObserver
{
    // array for waypoints and variables for move speed and waitime 
    [Header("movement settings")]
    public Transform[] waypoints;
    public float moveSpeed = 2f;           
    public float waitTimeAtWaypoint = 1f;

    // chase acceleration settings
    [Header("chase acceleration")]
    public float acceleration = 1f;       
    public float maxChaseSpeed = 10f;     
    private float currentSpeed;           

    // variables for waiting/chasing as well as waypoint index
    private int currentWaypointIndex = 0;
    private float waitTimer = 0f;
    private bool waiting = false;
    private bool isChasing = false;
    // reference to rigidbody
    private Rigidbody rb;                    

    // subscribe to detection events
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

    // update patrol or chase behavior
    protected override void Update()
    {
        base.Update();

        if (isChasing && playerTransform != null)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    // patrol between waypoints with waiting
    private void Patrol()
    {
        if (waypoints.Length == 0)
            return;

        if (waiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                waiting = false;
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            }
            return;
        }

        Transform target = waypoints[currentWaypointIndex];
        Vector3 direction = (target.position - rb.position).normalized;

        // use Rigidbody movement for physics consistency
        currentSpeed = moveSpeed; 
        Vector3 targetPosition = rb.position + direction * currentSpeed * Time.deltaTime;
        rb.MovePosition(targetPosition);

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, Time.deltaTime * 5f));
        }

        // start waiting when close enough to waypoint
        if (Vector3.Distance(rb.position, target.position) < 0.2f)
        {
            waiting = true;
            waitTimer = waitTimeAtWaypoint;
        }
    }

    // chase player by moving towards them with acceleration
    private void ChasePlayer()
    {
        // increase speed until max
        currentSpeed += acceleration * Time.deltaTime;
        currentSpeed = Mathf.Clamp(currentSpeed, moveSpeed, maxChaseSpeed);

        Vector3 direction = (playerTransform.position - rb.position).normalized;
        Vector3 targetPosition = rb.position + direction * currentSpeed * Time.deltaTime;
        rb.MovePosition(targetPosition);

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, Time.deltaTime * 5f));
    }

    // start chasing on player detected
    private void HandlePlayerDetected()
    {
        if (playerTransform == null)
            playerTransform = playerStealthState.transform;

        isChasing = true;
        currentSpeed = moveSpeed; 
        Debug.Log($"{name}: begin chase.");
    }

    // stop chasing and resume patrol on player lost
    private void HandlePlayerLost()
    {
        Debug.Log($"{name}: lost player. resume patrol.");
        isChasing = false;
        currentSpeed = moveSpeed; 
    }

    // reload scene immediately on inner radius triggered
    protected override void OnInnerRadiusTriggered()
    {
        base.OnInnerRadiusTriggered();
        Debug.Log($"{name}: inner radius triggered. scene will reload.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
