using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    [SerializeField] private Animator animator;
    
    public override void TakeDmg(float dmg, int attackType = 0)
    {
        base.TakeDmg(dmg, attackType);

        if(isDead)
        {
            animator.SetTrigger("doDie");
        }
        
    }
}
