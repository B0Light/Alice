using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagedCollider : MonoBehaviour
{
    [SerializeField] private Health health;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("weapon"))
        {
            Debug.Log("ATTACK");
            ItemController weapon = other.GetComponent<ItemController>();
            if (weapon.item.type == Item.ItemType.Weapon)
            {
                health.TakeDmg(weapon.item.totalAttackPower, weapon.item.attackType);
            }
        }
    }
}
