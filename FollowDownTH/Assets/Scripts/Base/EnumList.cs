using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumList : MonoBehaviour
{
    public enum States
    {
        Idle,
        HorizontalAttack,
        VerticalAttack,
        ChargeAttack,
        Dodge,
        Parry,
        Damaged,
    }
    
    public enum EnemyType
    {
        Ant,
        HeartSoldier,
    }
}
