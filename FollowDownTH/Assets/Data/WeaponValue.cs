using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "WeaponValue", menuName = "WeaponValue/Create Weapon", order = 2)]
public class WeaponValue : ScriptableObject
{
    public int attackValue = 10;
    public float horizontalCoefficient = 1.0f;
    public float verticalCoefficient = 1.0f;
    public float stingCoefficient = 1.0f;

    public float totalAttackPower = 10;
}
