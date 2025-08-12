using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bottle : MonoBehaviour
{
    //The collision sound's Audio Source
    public AudioSource hitSound;
    //How long the bottle stays in the world
    public float lifeTime = 3;

    [SerializeField] public float bottleNoiseRadius = 2f;

    private void Update()
    {
        //How long the thrown bottle stays in the world for
        lifeTime -= Time.deltaTime;
        if (lifeTime < 0)
            Destroy(gameObject);
    }

    //When something collides with the gameObject
    void OnCollisionEnter()
    {
        //The collision sound will play
        hitSound.Play();

        SoundManager.MakeNoise(transform.position, bottleNoiseRadius);
    }


}
