using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverInteractable : InteractableBase
{
    [SerializeField] private BarrierBase[] linkedBarriers;
    [SerializeField] private bool toggle = true;   // if false, only opens
    [SerializeField] private bool oneShot = false; // if true, lever disables itself after use
    [SerializeField] private Animator animator;    // optional for lever pull

    private bool _used;

    protected override bool IsAvailable(GameObject interactor)
    {
        if (oneShot && _used) return false;

        if (!toggle)
        {
            bool anyClosed = false;
            if (linkedBarriers != null)
            {
                foreach (var b in linkedBarriers)
                {
                    if (!b) continue;
                    if (!b.IsOpen) { anyClosed = true; break; }
                }
            }
            if (!anyClosed) return false;
        }

        return true;
    }

    public override void Interact(GameObject interactor)
    {
        if (!IsAvailable(interactor)) return;

        if (animator) animator.SetTrigger("Pull");

        if (linkedBarriers != null)
        {
            foreach (var b in linkedBarriers)
            {
                if (!b) continue;
                if (toggle) b.Toggle();
                else b.Open();
            }
        }

        if (oneShot) _used = true;
    }
}
