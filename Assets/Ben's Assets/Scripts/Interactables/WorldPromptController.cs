using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorldPromptController : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private CanvasGroup cg;
    [SerializeField] private float fadeSpeed = 12f;
    [SerializeField] private LayerMask occlusionMask = ~0; // set to your level geometry layers
    [SerializeField] private float occlusionRadius = 0.05f;

    private Camera _cam;
    private Transform _anchor;
    private bool _shouldShow;

    public void Initialize(Camera cam) => _cam = cam;

    public void Bind(Transform anchor, string prompt)
    {
        _anchor = anchor;
        text.text = prompt;
        _shouldShow = true;
    }

    public void Unbind() => _shouldShow = false;

    void Update()
    {
        if (!_anchor || !_cam) { FadeTo(0f); return; }

        // Simple LOS check: if something blocks the line between camera and anchor, hide.
        Vector3 from = _cam.transform.position;
        Vector3 to = _anchor.position;
        Vector3 dir = (to - from);
        float len = dir.magnitude;
        bool blocked = Physics.SphereCast(from, occlusionRadius, dir.normalized, out var hit, len, occlusionMask);

        FadeTo(!_shouldShow || blocked ? 0f : 1f);
    }

    private void FadeTo(float target)
    {
        if (!cg) return;
        cg.alpha = Mathf.MoveTowards(cg.alpha, target, fadeSpeed * Time.deltaTime);
        cg.blocksRaycasts = cg.interactable = cg.alpha > 0.99f;
    }
}
