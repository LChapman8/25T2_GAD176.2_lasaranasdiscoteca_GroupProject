using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public delegate void OnNoise(Vector3 position, float radius);
    public static event OnNoise SoundEvent;

    public static void MakeNoise(Vector3 position, float radius)
    {
        if (SoundEvent != null)
        {
            SoundEvent.Invoke(position, radius);
        }
    }
}
