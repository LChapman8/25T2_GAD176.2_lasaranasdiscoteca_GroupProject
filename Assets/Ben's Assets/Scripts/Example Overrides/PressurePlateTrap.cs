using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateTrap : TrapBase
{
    [Header("Alert Settings")]
    [SerializeField] private float alertRadius = 12f;
    [SerializeField] private float investigateDuration = 4f;
    [SerializeField] private LayerMask observerMask = ~0; // layer(s) the observers exist on
    [SerializeField] private AudioSource sfx;
    [SerializeField] private AudioClip triggerClip;

    protected override void OnTriggeredInternal(GameObject instigator)
    {
        if (sfx && triggerClip) sfx.PlayOneShot(triggerClip);

        Vector3 point = transform.position;

        // Find observers in range and nudge them to investigate
        var hits = Physics.OverlapSphere(point, alertRadius, observerMask);
        foreach (var h in hits)
        {
            var investigate = h.GetComponentInParent<IInvestigatingObserver>();
            if (investigate != null)
            {
                investigate.Investigate(point, investigateDuration);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.6f, 0f, 0.6f);
        Gizmos.DrawWireSphere(transform.position, alertRadius);
    }
#endif
}
