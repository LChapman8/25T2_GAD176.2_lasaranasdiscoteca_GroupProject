using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablePrompt : MonoBehaviour
{
    [SerializeField] private float faceLerp = 12f;
    [SerializeField] private Vector2 distanceScale = new Vector2(2f, 12f); // min/max meters
    [SerializeField] private Vector2 sizeRange = new Vector2(0.9f, 1.4f);  // min/max scale
    [SerializeField] private float verticalHover = 0.0f;                   // extra Y offset
    [SerializeField] private float forwardOffset = 0.25f;                  // push toward camera a bit

    private Camera _cam;
    private Transform _anchor;

    public void Attach(Transform anchor, Camera cam)
    {
        _anchor = anchor;
        _cam = cam;
        gameObject.SetActive(true);
        UpdateNow(); // snap on first frame
    }

    public void Detach()
    {
        _anchor = null;
        gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        if (!_anchor || !_cam) return;
        UpdateNow();
    }

    private void UpdateNow()
    {
        // Base position = anchor with slight push toward camera & optional hover
        Vector3 basePos = _anchor.position + Vector3.up * verticalHover;
        Vector3 camDir = (_cam.transform.position - basePos).normalized;
        Vector3 pos = basePos + (-camDir) * forwardOffset; // push toward camera (negative camDir)

        transform.position = pos;

        // Face camera (smooth)
        Quaternion look = Quaternion.LookRotation(transform.position - _cam.transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, look, 1f - Mathf.Exp(-faceLerp * Time.deltaTime));

        // Distance-based scale
        float d = Vector3.Distance(_cam.transform.position, pos);
        float t = Mathf.InverseLerp(distanceScale.x, distanceScale.y, d);
        float s = Mathf.Lerp(sizeRange.x, sizeRange.y, t);
        transform.localScale = Vector3.one * s;
    }
}
