using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bottle : MonoBehaviour
{
    //The collision sound's Audio Source
    public AudioSource hitSound;

    [SerializeField] public float footstepNoiseRadius = 2f;

    //When something collides with the gameObject
    void OnCollisionEnter()
    {
        //The collision sound will play
        hitSound.Play();

        SoundManager.MakeNoise(transform.position, footstepNoiseRadius);
    }
}
