using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [SerializeField] protected string promptText = "Press F to Interact";
    [SerializeField] protected float interactRadius = 2.0f;
    [SerializeField] protected float maxInteractAngle = 70f; // degrees, facing cone

    public virtual string PromptText => promptText;

    // Basic range + facing check; override for custom rules
    public virtual bool CanInteract(GameObject interactor)
    {
        if (interactor == null) return false;
        Vector3 toMe = transform.position - interactor.transform.position;
        float dist = toMe.magnitude;                                  // uses vector magnitude
        if (dist > interactRadius) return false;

        // Check facing using dot product (angle)
        Vector3 fwd = interactor.transform.forward;
        float angle = Vector3.Angle(fwd, toMe);                        // uses angles
        return angle <= maxInteractAngle;
    }

    public abstract void Interact(GameObject interactor);

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
#endif
}
