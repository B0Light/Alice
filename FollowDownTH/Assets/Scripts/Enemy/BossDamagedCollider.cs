using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDamagedCollider : MonoBehaviour
{
    [SerializeField] private float damagedCoefficient = 1.0f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerWeapon"))
        {
            PlayerWeaponController playerWeapon = other.GetComponent<PlayerWeaponController>();
        }
    }
}
