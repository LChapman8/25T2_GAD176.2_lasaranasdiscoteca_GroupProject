using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PickUpController : MonoBehaviour
{
    //[Header("Bottle Properties")]
    //public Bottle bottleScript;
    //public Rigidbody RigidB;
    //public CapsuleCollider Collider;
    //public Transform player, bottleContainer;

    //    //Pick up and drop force
    //public float pickUpRange;
    //public float dropForwardForce, dropUpwardForce;

    //    //To check if the item is equipped
    //public bool equipped;
    //    //To check if the player is already holding an item
    //public bool slotFull;

    //private void Start()
    //{
    //        //Setup
    //    if (!equipped)
    //    {
    //        bottleScript.enabled = false;
    //        RigidB.isKinematic = false;
    //        Collider.isTrigger = false;
    //    }
    //    if (equipped)
    //    {
    //        bottleScript.enabled = true;
    //        RigidB.isKinematic = true;
    //        Collider.isTrigger = true;
    //        slotFull = true;
    //    }
    //}

    //private void Update()
    //{
    //        //Check if player is in range and 'E' is pressed
    //    Vector3 distanceToPlayer = player.position - transform.position;
    //    if (!equipped && distanceToPlayer.magnitude <= pickUpRange && Input.GetKeyDown(KeyCode.E) && !slotFull) Pickup();

    //        //Drop if equipped and 'Q' is pressed
    //    if (equipped && Input.GetKeyDown(KeyCode.Q)) Drop();
    //}

    //private void Pickup()
    //{
    //        //The item has been equipped
    //    equipped = true;
    //    slotFull = true;

    //        //Make item a child of the player and move it to default position
    //    transform.SetParent(bottleContainer);
    //    transform.localPosition = Vector3.zero;
    //    transform.localRotation = Quaternion.Euler(Vector3.zero);
    //    transform.localScale = Vector3.one;

    //        //Make Rigidbody kinematic and Boxcollider a trigger
    //    RigidB.isKinematic = true;
    //    Collider.isTrigger = true;

    //        //Enable bottle script
    //    bottleScript.enabled = true;
    //}

    //private void Drop()
    //{
    //        //The item has been dropped
    //    equipped = false;
    //    slotFull = false;

    //        //Set parent to null
    //    transform.SetParent(null);

    //        //Make Rigidbody not kinematic and Boxcollider normal
    //    RigidB.isKinematic = false;
    //    Collider.isTrigger = false;

    //        //Item carries momentum of player
    //    RigidB.velocity = player.GetComponent<Rigidbody>().velocity;

    //        //Add force
    //    RigidB.AddForce(player.forward * dropForwardForce, ForceMode.Impulse);
    //    RigidB.AddForce(player.up * dropUpwardForce, ForceMode.Impulse);

    //        //Add random rotation to bottle when thrown
    //    float random = Random.Range(-1f, 1f);
    //    RigidB.AddTorque(new Vector3(random, random, random) * 10);

    //        //Disable bottle script
    //    bottleScript.enabled = false;
    //}
}
