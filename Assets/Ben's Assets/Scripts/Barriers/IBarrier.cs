using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBarrier
{
    bool IsOpen { get; }
    event System.Action OnOpened;
    event System.Action OnClosed;

    void Open();
    void Close();
    void Toggle();
}
