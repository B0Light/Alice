using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class ItemUI : MonoBehaviour
{
    public static ItemUI Instance;
    public Transform itemPickupUITransform;
    public GameObject itemPickupUI;
    public Sprite[] itemTierBackground;
    private List<Color> colors = new List<Color>();

    private void Awake()
    {
        Instance = this;
        SetColorList();
    }
    
    private void SetColorList()
    {
        colors.Add(new Color(0.7169812f,0.5083325f,0.01690993f,1f)); // common
        colors.Add(new Color(0.2722067f,0.5849056f,0.13519505f,1f)); // uncommom
        colors.Add(new Color(0.1541919f,0.3933419f,0.71223475f,1f)); //rare
        colors.Add(new Color(0.4543215f,0.2126654f,0.99174132f,1f)); //epic
        colors.Add(new Color(0.8971235f,0.8946123f,0.21643756f,1f)); //legendary
        colors.Add(new Color(0.9912354f,0.3451256f,0.61234353f,1f)); //mythic
    }
    public GameObject SetPickupItemUI(ItemPickUp itemPickup)
    {
        GameObject obj = Instantiate(itemPickupUI, itemPickupUITransform);
        var itemMain = obj.GetComponent<Image>();
        var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
        var itemBack = obj.transform.Find("ItemBack").GetComponent<Image>();
        var itemName = obj.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
        var itemInfo = obj.transform.Find("ItemInfo").GetComponent<TextMeshProUGUI>();
        var itemPickupBtn = obj.transform.Find("Pickup").GetComponent<Button>();
    
        itemMain.color = colors[(int) itemPickup.item.tier];
        itemIcon.sprite = itemPickup.item.icon;
        itemBack.sprite = itemTierBackground[(int)itemPickup.item.tier];
        itemName.text = itemPickup.item.itemName;
        itemInfo.text = itemPickup.item.value.ToString();
        
        itemPickupBtn.onClick.AddListener(() => OnClickEvent(itemPickup));
        
        return obj;
    }

    private void OnClickEvent(ItemPickUp itemPickup)
    {
        itemPickup.PickupItem();
    }
}
