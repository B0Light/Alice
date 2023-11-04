using UnityEngine;
[CreateAssetMenu(fileName = "New Item", menuName = "Item/Creat New Item")]
public class Item : ScriptableObject
{
    public int id;
    public bool isEquip = false;
    
    public string itemName;
    public Sprite icon;
    public GameObject itemObject;
    public int value = 10;

    public enum ItemTier
    {
        Common,
        UnCommon,
        Rare,
        Unique,
        Legendary,
        Mythic,
    }
    public ItemTier tier;
    
    public enum ItemType
    {
        Weapon,
        Consumables
    }
    public ItemType type;

    public int attackType = 0;
    public float horizontalCoefficient = 1.0f;
    public float verticalCoefficient = 1.0f;
    public float stingCoefficient = 1.0f;

    public float totalAttackPower = 10;
}
