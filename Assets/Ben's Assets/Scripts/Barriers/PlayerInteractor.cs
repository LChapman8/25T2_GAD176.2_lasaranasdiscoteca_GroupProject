using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float scanRadius = 2.5f;
    [SerializeField] private LayerMask interactableMask = ~0; // or specific layer

    [Header("Input")]
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    private IInteractable _current;

    void Update()
    {
        FindBestInteractable();

        if (_current != null && Input.GetKeyDown(interactKey))
        {
            var interactableGO = (_current as MonoBehaviour)?.gameObject;
            if (_current.CanInteract(gameObject))
                _current.Interact(gameObject);
        }
    }

    void FindBestInteractable()
    {
        _current = null;

        var hits = Physics.OverlapSphere(transform.position, scanRadius, interactableMask);
        float bestScore = float.MaxValue;

        foreach (var col in hits)
        {
            if (!col) continue;
            var interactable = col.GetComponentInParent<IInteractable>();
            if (interactable == null) continue;

            if (!interactable.CanInteract(gameObject)) continue;

            // score by distance (closest wins)
            float dist = (col.transform.position - transform.position).sqrMagnitude;
            if (dist < bestScore)
            {
                bestScore = dist;
                _current = interactable;
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, scanRadius);
    }
#endif
}
