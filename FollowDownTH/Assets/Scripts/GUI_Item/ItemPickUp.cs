using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public Item item;
    private GameObject showUI = null;

    private bool isPickable = false;
    
    private void ShowItem()
    {
        if(showUI == null)
            showUI = ItemUI.Instance.SetPickupItemUI(this);
        showUI.SetActive(true);
        isPickable = true;
    }

    private void HideItem()
    {
        showUI.SetActive(false); 
        isPickable = false;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ShowItem();
        }
            
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HideItem();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                PickupItem();
                other.GetComponent<PlayerAttackManager>().PickUpWeapon(item);
            }
        }
    }

    public void PickupItem()
    {
        if(!isPickable) return;
        isPickable = false;
        HideItem();
        DestroyThisObj();
    }

    void DestroyThisObj()
    {
        Destroy(showUI);
        Destroy(gameObject);
    }
}
