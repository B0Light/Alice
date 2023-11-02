using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    private Animator animator;
    private PlayerLocomotionManager playerLocomotionManager;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
    }
    
    public override void TakeDmg(float dmg)
    {
        base.TakeDmg(dmg);
        playerLocomotionManager.curPerformingAction = PlayerLocomotionManager.States.Damaged;
        // damaged ani
    }
    
    private void PlayActionAnimation(string animation, bool isPerformingAction)
    {
        animator.SetBool("isPerformingAction", isPerformingAction);
        animator.CrossFade(animation, 0.2f);
    }
}
