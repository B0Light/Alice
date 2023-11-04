using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    
    [SerializeField] private Transform weaponSlot;
    [SerializeField] private PlayerLocomotionManager playerLocomotionManager;
    [SerializeField] private Item weaponValue;
    [SerializeField] private PlayerHealth playerHealth;
    
    public  bool isEquipWeapon = false;

    private bool isSetTotalDamage = false;
    
    public void PickUpWeapon(Item weapon)
    {
        isEquipWeapon = true;
        UnEquipWeapon();
        Instantiate(weapon.itemObject, weaponSlot);
        weaponValue = weapon;
    }

    private void UnEquipWeapon()
    {
        if (weaponSlot != null)
        {
            foreach (Transform child in weaponSlot)
            {
                child.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.Log("WeaponSlot is Null");
        }
    }

    private void Update()
    {
        if (isEquipWeapon)
        {
            if (playerLocomotionManager.isAttack && !isSetTotalDamage)
            {
                SetWeaponDamage();
            }

            if (playerLocomotionManager.isAttack == false)
            {
                weaponValue.totalAttackPower = 0;
                weaponValue.attackType = 0;
                isSetTotalDamage = false;
            }
        }
        
    }

    private void SetWeaponDamage()
    {
        isSetTotalDamage = true;
        switch (playerLocomotionManager.curPerformingAction)
        {
            case EnumList.States.HorizontalAttack:
                weaponValue.totalAttackPower = weaponValue.horizontalCoefficient * weaponValue.value;
                weaponValue.attackType = 1;
                break;
            case EnumList.States.VerticalAttack:
                weaponValue.totalAttackPower = weaponValue.verticalCoefficient * weaponValue.value;
                weaponValue.attackType = 2;
                break;
            case EnumList.States.ChargeAttack:
                weaponValue.totalAttackPower = weaponValue.stingCoefficient * weaponValue.value;
                weaponValue.attackType = 3;
                break;
            // not used
            case EnumList.States.Idle:
                weaponValue.totalAttackPower = weaponValue.value;
                weaponValue.attackType = 0;
                break;
        }
    }
}
