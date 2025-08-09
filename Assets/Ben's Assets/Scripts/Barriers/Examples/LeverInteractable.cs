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

    public override void Interact(GameObject interactor)
    {
        if (_used && oneShot) return;

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
