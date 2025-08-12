using PeekabooPro.Observers;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    // variable for chasing 
    private bool isChasing = false;           

    // subscribe to events on awake
    protected override void Awake()
    {
        base.Awake();
        OnPlayerDetectedEvent += HandlePlayerDetected;
        OnPlayerLostEvent += HandlePlayerLost;
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

    // move towards player position smoothly
    private void MoveTowardPlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    // called when player detected, start chasing
    private void HandlePlayerDetected()
    {
        if (playerTransform == null)
            playerTransform = playerStealthState.transform;

        isChasing = true;
        Debug.Log($"{name}: begin chase.");
    }

    // called when player lost, stop chasing and reset target
    private void HandlePlayerLost()
    {
        Debug.Log($"{name}: lost player. returning to scan.");
        isChasing = false;
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
