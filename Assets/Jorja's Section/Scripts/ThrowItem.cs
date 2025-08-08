using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ThrowItem : MonoBehaviour
{
    public GameObject itemPrefab; // The item that will be trown
    public Transform throwPoint; // Where the item will be thrown from
    public float throwForce = 10f; // Throw power/force

    void Update()
    {
        // Press 'Q' to throw
        if (Input.GetKeyDown(KeyCode.Q)) 
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
    }
}
