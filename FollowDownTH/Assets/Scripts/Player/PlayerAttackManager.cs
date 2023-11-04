using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerAttackManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform atkPos;
    
    [SerializeField] private Transform weaponSlot;
    [SerializeField] private PlayerLocomotionManager playerLocomotionManager;
    [SerializeField] private Item weaponValue;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private GameObject weaponIcon;
    

    public AudioSource slashSFX;
    
    public  bool isEquipWeapon = false;

    private int isSetTotalDamage = 0;
    
    [SerializeField] private List<GameObject> impactVfx;
    public void PickUpWeapon(Item weapon)
    {
        isEquipWeapon = true;
        UnEquipWeapon();
        Instantiate(weapon.itemObject, weaponSlot);
        weaponValue = weapon;
        if(weaponIcon)
            weaponIcon.SetActive(true);
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
            if (playerLocomotionManager.isAttack)
            {
                if ((isSetTotalDamage == 0) || (playerLocomotionManager.isCombo && isSetTotalDamage == 1))
                {
                    SetWeaponDamage();
                }
            }
            

            if (playerLocomotionManager.isAttack == false)
            {
                weaponValue.totalAttackPower = 0;
                weaponValue.attackType = 0;
                isSetTotalDamage = 0;
            }
        }
        
    }

    private void SetWeaponDamage()
    {
        switch (playerLocomotionManager.curPerformingAction)
        {
            case EnumList.States.HorizontalAttack:
                weaponValue.totalAttackPower = weaponValue.horizontalCoefficient * weaponValue.value;
                weaponValue.attackType = 1;
                if (isSetTotalDamage == 0)
                {
                    StartCoroutine(VFX(0, 3f, 0.3f));
                    isSetTotalDamage = 1;
                }
                else
                {
                    StartCoroutine(VFX(3, 3f, 0.3f));
                    isSetTotalDamage = 2;
                }
                    
                break;
            case EnumList.States.VerticalAttack:
                weaponValue.totalAttackPower = weaponValue.verticalCoefficient * weaponValue.value;
                weaponValue.attackType = 2;
                if (isSetTotalDamage == 0)
                {
                    StartCoroutine(VFX(1, 3f, 0.5f));
                    isSetTotalDamage = 1;
                }
                else
                {
                    StartCoroutine(VFX(4, 3f, 0.5f));
                    isSetTotalDamage = 2;
                }
                    
                break;
            case EnumList.States.ChargeAttack:
                weaponValue.totalAttackPower = weaponValue.stingCoefficient * weaponValue.value;
                weaponValue.attackType = 3;
                if (isSetTotalDamage == 0)
                {
                    StartCoroutine(VFX(2,3f, .8f));
                    isSetTotalDamage = 1;
                }
                else
                {
                    StartCoroutine(VFX(5, 3f, 1.5f));
                    isSetTotalDamage = 2;
                }
                    
                break;
            // not used
            case EnumList.States.Idle:
                weaponValue.totalAttackPower = weaponValue.value;
                weaponValue.attackType = 0;
                break;
        }
    }

    public void GiantWeapon()
    {
        weaponSlot.localScale = (Vector3.one * 5f);
        StartCoroutine(ResetWeaponSize());
    }

    IEnumerator ResetWeaponSize()
    {
        yield return new WaitForSeconds(5f);
        weaponSlot.localScale = Vector3.one;
    }
    
    
    IEnumerator VFX(int type, float impactVfxLifetime, float delay)
    {
        if (impactVfx[type])
        {
            yield return new WaitForSeconds(delay);
            GameObject impactVfxInstance = Instantiate(impactVfx[type], atkPos);
            slashSFX.Play();
            impactVfxInstance.transform.localScale = weaponSlot.localScale;
            if (impactVfxLifetime > 0)
            {
                Destroy(impactVfxInstance, impactVfxLifetime);
            }
        }
    }
}
