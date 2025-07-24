using PeekabooPro.Observers;
using UnityEngine;




public class StationaryObserver : BaseObserver
{
    // variables for scanning for player 
    [Header("Scanning Rotation")]
    public bool enableScanning = true;
    public float rotationSpeed = 30f;             
    public float rotationAngle = 45f;              

    private float startingYRotation;
    private float currentRotation;
    private bool rotatingRight = true;

    protected override void Awake()
    {
        base.Awake();
        startingYRotation = transform.eulerAngles.y;

        // subscribe to base events
        OnPlayerDetectedEvent += HandlePlayerDetected;
        OnPlayerLostEvent += HandlePlayerLost;
    }
    // on update rotate if player not found 
    protected override void Update()
    {
        base.Update();

        if (enableScanning && !playerDetected)
            RotateScanner();
    }
    // function for rotating the scanning 
    private void RotateScanner()
    {
        float rotationStep = rotationSpeed * Time.deltaTime;
        currentRotation += rotatingRight ? rotationStep : -rotationStep;

        float clampedRotation = Mathf.Clamp(currentRotation, -rotationAngle, rotationAngle);
        transform.rotation = Quaternion.Euler(0, startingYRotation + clampedRotation, 0);

        if (Mathf.Abs(currentRotation) >= rotationAngle)
            rotatingRight = !rotatingRight;
    }
    // need to define logic for what happens when player is detected and lost 
    private void HandlePlayerDetected()
    {
        Debug.Log($"{name}: Alarm triggered by StationaryObserver.");
        
    }

    private void HandlePlayerLost()
    {
        Debug.Log($"{name}: Player escaped detection zone.");
        
    }
}

