using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponPickUp : MonoBehaviour
{
    [SerializeField] private GameObject weaponMesh;
    [SerializeField] private GameObject pickUpMsg;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            weaponMesh.SetActive(false);
        }
    }

    private void PickUpMessage()
    {
        
    }
}
