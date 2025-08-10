using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeBarrier : BarrierBase
{
    [Header("Bridge Motion")]
    [SerializeField] private Transform bridgeTransform;
    [SerializeField] private Vector3 downLocalPosition;
    [SerializeField] private Vector3 upLocalPosition;
    [SerializeField] private float moveTime = 1.2f;
    [SerializeField] private AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Edge Blockers (enabled when closed)")]
    [SerializeField] private Collider[] blockerColliders;

    private Coroutine _moveCo;

    protected override void Awake()
    {
        base.Awake();
        // Snap to initial position
        if (bridgeTransform)
            bridgeTransform.localPosition = startOpen ? upLocalPosition : downLocalPosition;

        SetBlockersEnabled(!startOpen);
    }

    protected override void ApplyState(bool open, bool force = false)
    {
        if (!force && open == IsOpen) return;

        base.ApplyState(open, force);   // updates IsOpen

        // Move bridge
        if (_moveCo != null) StopCoroutine(_moveCo);
        _moveCo = StartCoroutine(CoMoveBridge(open ? upLocalPosition : downLocalPosition));

        // Toggle edge blockers
        SetBlockersEnabled(!open);
    }

    private IEnumerator CoMoveBridge(Vector3 targetLocalPos)
    {
        if (!bridgeTransform || moveTime <= 0f)
        {
            if (bridgeTransform) bridgeTransform.localPosition = targetLocalPos;
            yield break;
        }

        Vector3 start = bridgeTransform.localPosition;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / moveTime;
            float k = moveCurve.Evaluate(Mathf.Clamp01(t));
            bridgeTransform.localPosition = Vector3.LerpUnclamped(start, targetLocalPos, k);
            yield return null;
        }
        bridgeTransform.localPosition = targetLocalPos;
        _moveCo = null;
    }

    private void SetBlockersEnabled(bool enabled)
    {
        if (blockerColliders == null) return;
        for (int i = 0; i < blockerColliders.Length; i++)
        {
            if (blockerColliders[i]) blockerColliders[i].enabled = enabled;
        }
    }
}
