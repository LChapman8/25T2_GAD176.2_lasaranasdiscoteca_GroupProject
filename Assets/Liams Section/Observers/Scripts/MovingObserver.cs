using PeekabooPro.Observers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MovingObserver : BaseObserver
{
    // array for waypoints and variables for move speed and waitime 
    [Header("movement settings")]
    public Transform[] waypoints;           
    public float moveSpeed = 2f;            
    public float waitTimeAtWaypoint = 1f;   
    // variables for waiting/chasing as well as waypoint index
    private int currentWaypointIndex = 0;   
    private float waitTimer = 0f;            
    private bool waiting = false;             
    private bool isChasing = false;           

    // subscribe to detection events
    protected override void Awake()
    {
        base.Awake();
        OnPlayerDetectedEvent += HandlePlayerDetected;
        OnPlayerLostEvent += HandlePlayerLost;
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
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        // start waiting when close enough to waypoint
        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            waiting = true;
            waitTimer = waitTimeAtWaypoint;
        }
    }

    // chase player by moving towards them
    private void ChasePlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    // start chasing on player detected
    private void HandlePlayerDetected()
    {
        if (playerTransform == null)
            playerTransform = playerStealthState.transform;

        isChasing = true;
        Debug.Log($"{name}: begin chase.");
    }

    // stop chasing and resume patrol on player lost
    private void HandlePlayerLost()
    {
        Debug.Log($"{name}: lost player. resume patrol.");
        isChasing = false;
        playerTransform = null;
    }

    // reload scene immediately on inner radius triggered
    protected override void OnInnerRadiusTriggered()
    {
        base.OnInnerRadiusTriggered();
        Debug.Log($"{name}: inner radius triggered. scene will reload.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
