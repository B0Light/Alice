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
            DrinkPotion(other.gameObject);
        }
    }

    public void DrinkPotion(GameObject player)
    {
        Debug.Log("DrinkPotion");
        PlayerStamina playerStamina = player.GetComponent<PlayerStamina>();
        playerStamina.DrinkPotion();
        player.transform.localScale = Vector3.one;
        player.GetComponent<PlayerLocomotionManager>().canRecovering = true;
    }
}
