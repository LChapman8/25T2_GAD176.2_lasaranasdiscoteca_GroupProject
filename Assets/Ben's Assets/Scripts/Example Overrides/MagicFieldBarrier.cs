using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicFieldBarrier : BarrierBase
{
    [Header("Magic Field FX (optional)")]
    [SerializeField] private ParticleSystem openFX;
    [SerializeField] private ParticleSystem closeFX;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openClip;
    [SerializeField] private AudioClip closeClip;

    protected override void ApplyState(bool open, bool force = false)
    {
        base.ApplyState(open, force);

        if (open)
        {
            if (openFX) openFX.Play();
            if (audioSource && openClip) audioSource.PlayOneShot(openClip);
        }
        else
        {
            if (closeFX) closeFX.Play();
            if (audioSource && closeClip) audioSource.PlayOneShot(closeClip);
        }
    }
}
