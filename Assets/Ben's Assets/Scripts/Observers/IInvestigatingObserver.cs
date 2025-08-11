using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInvestigatingObserver
{
    /// Orders the observer to move to a point, linger briefly, then resume normal behavior.
    void Investigate(Vector3 worldPoint, float duration);
}
