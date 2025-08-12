using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Peekaboopro.Throw
{
    public class ThrowItem : MonoBehaviour
    {
        public GameObject itemPrefab; // The item that will be thrown
        public Transform throwPoint; // Where the item will be thrown from
        public float throwForce = 10f; // Throw power/force

        public float cooldownTime = 5f; // The cooldown timer
        private float nextReadyTime;
        private bool isCoolingDown = false;

        void Update()
        {
            if (isCoolingDown && Time.time >= nextReadyTime)
            {
                isCoolingDown = false;
            }

            //Check if a key is pressed 
            if (Input.GetKeyDown(KeyCode.R) && !isCoolingDown)
            {
                Throw();
            }
        }

        public void Throw()
        {

            if (itemPrefab != null && throwPoint != null)
            {
                GameObject thrownItem = Instantiate(itemPrefab, throwPoint.position, throwPoint.rotation);
                Rigidbody itemRigidbody = thrownItem.GetComponent<Rigidbody>();

                if (itemRigidbody != null)
                {
                    itemRigidbody.AddForce(throwPoint.forward * throwForce, ForceMode.Impulse);
                }
                else
                {
                    Debug.LogWarning("Thrown item does not have a Rigidbody component!");
                }
            }
            else
            {
                Debug.LogWarning("Item Prefab or Throw Point not assigned in the Inspector!");
            }

            // To check if it is working
            Debug.Log("Item Used!");

            // Start the cooldown
            isCoolingDown = true;
            nextReadyTime = Time.time + cooldownTime;
        }
    }
}
