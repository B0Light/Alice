using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    [SerializeField] private Health playerhealth;

    public float damage;
    public int attackType;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerhealth = other.GetComponent<Health>();
            playerhealth.TakeDmg(damage, attackType);
        }
    }
}
