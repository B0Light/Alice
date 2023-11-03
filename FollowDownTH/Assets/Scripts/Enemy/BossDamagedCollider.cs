using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDamagedCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerWeapon"))
        {
            PlayerWeaponController playerWeapon = other.GetComponent<PlayerWeaponController>();
        }
    }
}
