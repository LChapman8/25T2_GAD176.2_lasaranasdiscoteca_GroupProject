using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PeekabooPro.Observers;

[DisallowMultipleComponent]
public class ObserverInvestigation : MonoBehaviour, IInvestigatingObserver
{
    [Header("Investigation Movement")]
    [SerializeField] private float approachSpeed = 2.5f;
    [SerializeField] private float rotateSpeed = 5f;
    [SerializeField] private float lingerTime = 1.0f;
    [SerializeField] private float arriveThreshold = 0.2f;

    [Header("Observer Return Settings")]
    [SerializeField] private bool returnHomeAfterInvestigation = true;
    [SerializeField] private Transform homeAnchor;      // optional explicit home
    [SerializeField] private float homeArriveThreshold = 0.2f;
    [SerializeField] private float homeRotateSpeed = 6f;

    private BaseObserver _baseObs;
    private Vector3 _homePos;
    private Quaternion _homeRot;
    private UnityEngine.AI.NavMeshAgent _agent;

    // Optional handles if these components exist
    private StationaryObserver _stationary;
    private MovingObserver _moving;

    // State backups so we can restore exactly
    private float _prevMoveSpeed;
    private bool _prevScanning;

    private Coroutine _investRoutine;

    void Awake()
    {
        _baseObs = GetComponent<BaseObserver>();
        _stationary = GetComponent<StationaryObserver>();
        _moving = GetComponent<MovingObserver>();
        _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        // snapshot home if not provided
        _homePos = homeAnchor ? homeAnchor.position : transform.position;
        _homeRot = homeAnchor ? homeAnchor.rotation : transform.rotation;

        if (_baseObs == null)
        {
            Debug.LogError($"{name}: ObserverInvestigation requires a BaseObserver-derived component.");
            enabled = false;
            return;
        }

        // Subscribe to detection events so investigation cancels immediately on sight
        _baseObs.OnPlayerDetectedEvent += CancelInvestigationAndRestore;   // fires when outer detection hits
    }

    void OnDestroy()
    {
        if (_baseObs != null)
        {
            _baseObs.OnPlayerDetectedEvent -= CancelInvestigationAndRestore;
        }
    }

    public void Investigate(Vector3 worldPoint, float duration)
    {
        if (!isActiveAndEnabled) return;

        if (_investRoutine != null) StopCoroutine(_investRoutine);
        _investRoutine = StartCoroutine(CoInvestigate(worldPoint, duration));
    }

    private IEnumerator CoInvestigate(Vector3 point, float duration)
    {
        CacheAndPauseNativeMovement();

        // Move to investigation point (agent if available)
        yield return MoveTo(point, arriveThreshold);

        // Linger (only if we actually arrived close enough)
        if (Vector3.Distance(transform.position, point) <= arriveThreshold)
            yield return new WaitForSeconds(lingerTime);

        // If we didn’t detect the player and option is on, go home
        if (returnHomeAfterInvestigation)
            yield return ReturnHome();

        RestoreNativeMovement();
        _investRoutine = null;
    }

    private IEnumerator MoveTo(Vector3 target, float stopDistance)
    {
        if (_agent && _agent.isOnNavMesh)
        {
            // drive with NavMesh
            _agent.isStopped = false;
            _agent.speed = Mathf.Max(approachSpeed, 0.01f);
            _agent.SetDestination(target);

            while (true)
            {
                if (_agent.pathPending) { yield return null; continue; }
                if (_agent.remainingDistance <= Mathf.Max(stopDistance, _agent.stoppingDistance)) break;

                // face move direction
                Vector3 vel = _agent.desiredVelocity;
                if (vel.sqrMagnitude > 0.001f)
                {
                    Quaternion look = Quaternion.LookRotation(vel.normalized);
                    transform.rotation = Quaternion.Slerp(transform.rotation, look, rotateSpeed * Time.deltaTime);
                }
                yield return null;
            }
            _agent.isStopped = true;
        }
        else
        {
            // simple straight-line glide
            while (Vector3.Distance(transform.position, target) > stopDistance)
            {
                Vector3 dir = (target - transform.position).normalized;
                transform.position += dir * approachSpeed * Time.deltaTime;
                if (dir.sqrMagnitude > 0.0001f)
                {
                    Quaternion look = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, look, rotateSpeed * Time.deltaTime);
                }
                yield return null;
            }
        }
    }

    private IEnumerator ReturnHome()
    {
        // Stationary: keep scanning off until we're back at our post
        bool restoreScanAtEnd = false;
        if (_stationary != null)
        {
            restoreScanAtEnd = true;
            _stationary.enableScanning = false;
        }

        yield return MoveTo(_homePos, homeArriveThreshold);

        // Snap/lerp to original facing so they don’t keep the “look at trap” rotation
        float t = 0f;
        while (t < 1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _homeRot, t);
            t += Time.deltaTime * homeRotateSpeed;
            yield return null;
        }
        transform.rotation = _homeRot;

        if (restoreScanAtEnd) _stationary.enableScanning = true;
    }

    private void CacheAndPauseNativeMovement()
    {
        if (_moving != null)
        {
            _prevMoveSpeed = _moving.moveSpeed;
            _moving.moveSpeed = 0f; // pauses Patrol/Chase movement without touching detection
        }
        if (_stationary != null)
        {
            _prevMoveSpeed = _stationary.moveSpeed;
            _prevScanning = _stationary.enableScanning;
            _stationary.moveSpeed = 0f;
            _stationary.enableScanning = false; // stop spinning while we steer
        }
    }

    private void RestoreNativeMovement()
    {
        if (_moving != null) _moving.moveSpeed = _prevMoveSpeed;
        if (_stationary != null)
        {
            _stationary.moveSpeed = _prevMoveSpeed;
            _stationary.enableScanning = _prevScanning;
        }
    }

    private void CancelInvestigationAndRestore()
    {
        if (_investRoutine != null)
        {
            StopCoroutine(_investRoutine);
            _investRoutine = null;
            RestoreNativeMovement();
        }
    }
}
