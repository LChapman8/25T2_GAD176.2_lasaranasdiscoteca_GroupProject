using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Acts like a barrier for levers, but only opens the real barrier
// once 'requiredCount' levers have called Open().
public class DualLeverGate : BarrierBase
{
    [Header("Dual Lever Gate")]
    [SerializeField] private BarrierBase targetBarrier; // the real barrier to open/close
    [SerializeField] private int requiredCount = 2;

    private int _currentCount = 0;

    protected override void Awake()
    {
        base.Awake();
        // Mirror starting state to the target at load
        if (targetBarrier)
        {
            if (startOpen) targetBarrier.Open();
            else targetBarrier.Close();
        }
    }

    public override void Open()
    {
        // Called by each lever set to oneShot=false/true or toggle=false
        if (IsOpen) return;               // gate already satisfied
        _currentCount = Mathf.Min(_currentCount + 1, requiredCount);

        if (_currentCount >= requiredCount)
        {
            base.Open();                   // flips IsOpen and fires OnOpened
            if (targetBarrier) targetBarrier.Open();
        }
    }

    public override void Close()
    {
        // Optionally allow for barrier to be closed (reset)
        _currentCount = 0;
        base.Close();
        if (targetBarrier) targetBarrier.Close();
    }

    public override void Toggle()
    {
        // Avoid partial toggles; treat as Open() attempts
        Open();
    }
}
