using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BossPatern", menuName = "BossPattern/Create BossPattern", order = 1)]
public class BossPattern : ScriptableObject
{
    public string attackType;

    public float range;

    public float damage;

    
}
