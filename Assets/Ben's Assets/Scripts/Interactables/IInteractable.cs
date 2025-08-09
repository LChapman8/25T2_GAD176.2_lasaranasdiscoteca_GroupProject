using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    string PromptText { get; }
    bool CanInteract(GameObject interactor);
    void Interact(GameObject interactor);
}
