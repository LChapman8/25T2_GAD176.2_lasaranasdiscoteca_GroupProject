using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITrap
{
    bool IsActive { get; }
    event Action<ITrap, GameObject> OnTriggered;

    void Trigger(GameObject instigator);
    void Suppress(float duration);  // temporarily disable
}
