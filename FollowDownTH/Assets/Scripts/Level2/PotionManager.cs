using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PotionManager : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("DrinkPotion");
            PlayerStamina playerStamina = other.GetComponent<PlayerStamina>();
            playerStamina.DrinkPotion();
            other.transform.localScale = Vector3.one;
        }
    }
}
