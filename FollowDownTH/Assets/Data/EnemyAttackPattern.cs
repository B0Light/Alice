using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "BossPatern", menuName = "BossPattern/Create BossPattern", order = 1)]
public class EnemyAttackPattern : ScriptableObject
{
    public string attackName;

    public float range;

    public float damage;
    public int attackType;
}
