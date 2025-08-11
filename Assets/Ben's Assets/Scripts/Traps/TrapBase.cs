using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class TrapBase : MonoBehaviour, ITrap
{
    [Header("Trap Base")]
    [SerializeField] protected bool startActive = true;
    [SerializeField] protected bool oneShot = false;         // if true, deactivates forever after first trigger
    [SerializeField] protected float retriggerCooldown = 1f; // grace period between triggers
    [SerializeField] protected LayerMask triggerLayers;      // e.g., Player layer

    protected float _cooldownTimer;
    protected float _suppressedTimer;
    protected bool _consumed;

    public bool IsActive => startActive && !_consumed && _suppressedTimer <= 0f && _cooldownTimer <= 0f;
    public event System.Action<ITrap, GameObject> OnTriggered;

    protected virtual void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    protected virtual void Update()
    {
        if (_cooldownTimer > 0f) _cooldownTimer -= Time.deltaTime;
        if (_suppressedTimer > 0f) _suppressedTimer -= Time.deltaTime;
    }

    public virtual void Suppress(float duration)
    {
        _suppressedTimer = Mathf.Max(_suppressedTimer, duration);
    }

    public void Trigger(GameObject instigator)
    {
        if (!IsActive) return;

        _cooldownTimer = retriggerCooldown;
        if (oneShot) _consumed = true;

        OnTriggered?.Invoke(this, instigator);
        OnTriggeredInternal(instigator);
    }

    // Let subclasses implement their effect
    protected abstract void OnTriggeredInternal(GameObject instigator);

    protected virtual bool ShouldTrigger(Collider other)
    {
        // layer check
        return ((1 << other.gameObject.layer) & triggerLayers) != 0;
    }

    protected void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trap hit by: {other.name} (layer {LayerMask.LayerToName(other.gameObject.layer)})");

        if (!IsActive) return;
        if (!ShouldTrigger(other)) return;
        Trigger(other.gameObject);
    }
}
