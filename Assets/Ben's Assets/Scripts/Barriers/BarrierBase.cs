using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BarrierBase : MonoBehaviour, IBarrier
{
    [Header("Barrier Base")]
    [SerializeField] protected Collider solidCollider; // blocks the player
    [SerializeField] protected Renderer[] visuals;     // optional VFX meshes
    [SerializeField] protected bool startOpen = false;

    public event System.Action OnOpened;
    public event System.Action OnClosed;

    public bool IsOpen { get; protected set; }

    protected virtual void Awake()
    {
        ApplyState(startOpen, force: true);
    }

    public virtual void Open() => ApplyState(true);
    public virtual void Close() => ApplyState(false);
    public virtual void Toggle() => ApplyState(!IsOpen);

    protected virtual void ApplyState(bool open, bool force = false)
    {
        if (!force && open == IsOpen) return;

        IsOpen = open;

        if (solidCollider != null)
            solidCollider.enabled = !open; // closed = collider enabled

        // Visuals (fade out when opening, etc)
        for (int i = 0; i < (visuals?.Length ?? 0); i++)
        {
            if (visuals[i] == null) continue;
            visuals[i].enabled = !open;
        }

        // Hooks for subclasses (such as audio/VFX)
        if (open) OnOpened?.Invoke();
        else OnClosed?.Invoke();
    }
}
