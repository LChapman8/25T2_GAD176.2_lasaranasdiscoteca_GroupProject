using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    //Tracks if the item is being held
    public bool isHeld = false;

    //Reference to the player
    public GameObject player;

    void Update()
    {
        //Allows item to be picked up if the player is nearby and presses 'E'
        if(player != null && Input.GetKeyDown(KeyCode.E) && !isHeld && Vector3.Distance(transform.position, player.transform.position) < 2f)
        {
            //Changed isHeld to true and sets the parent and location to the player
            isHeld = true;
            transform.SetParent(player.transform);
            transform.localPosition = new Vector3(0, 1, 1);
        }
    }

    //When the player enters the area
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
        }
    }

    //When the player leaves the area
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
        }
    }
}
