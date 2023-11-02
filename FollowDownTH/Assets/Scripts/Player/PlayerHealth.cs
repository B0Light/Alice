using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    private Animator animator;
    private PlayerLocomotionManager playerLocomotionManager;
    [SerializeField] private Color parryColor;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            DamageDebugger();
        }
    }

    public void DamageDebugger()
    {
        DamagedHandler(2, 10);
    }

    public void DamagedHandler(int attackType, float dmg)
    {
        // attackType 1 : horizontal
        // attackType 2 : vertial
        switch (playerLocomotionManager.curPerformingAction)
        {
            case PlayerLocomotionManager.States.Idle:
                TakeDmg(dmg);
                break;
            case PlayerLocomotionManager.States.HorizontalAttack:
                if(attackType == 2)
                {
                    StartCoroutine(OnParry());
                }
                else
                {
                    TakeDmg(dmg);
                }
                break;
            case PlayerLocomotionManager.States.VerticalAttack:
                if (attackType == 1)
                {
                    StartCoroutine(OnParry());
                }
                else
                {
                    TakeDmg(dmg);
                }
                break;
            case PlayerLocomotionManager.States.Dodge:
                break;
        }
    }

    IEnumerator OnParry()
    {
        PlayActionAnimation("Parrying", true);
        playerLocomotionManager.curPerformingAction = PlayerLocomotionManager.States.Parry;
        itemMesh.material.color = parryColor;
        yield return new WaitForSeconds(0.2f);
        itemMesh.material.color = Color.white;
    }

    public override void TakeDmg(float dmg)
    {
        base.TakeDmg(dmg);

        playerLocomotionManager.curPerformingAction = PlayerLocomotionManager.States.Damaged;
        if(isDead)
        {
            if (playerLocomotionManager.isTwoHandingWeapon)
            {
                PlayActionAnimation("TH_Death", true);
            }
            else
            {
                PlayActionAnimation("OH_Death", true);
            }
        }
        else
        {
            if (playerLocomotionManager.isTwoHandingWeapon)
            {
                PlayActionAnimation("TH_Hit", true);
            }
            else
            {
                PlayActionAnimation("OH_Hit", true);
            }
        }
        
    }
    
    private void PlayActionAnimation(string animation, bool isPerformingAction)
    {
        animator.SetBool("isPerformingAction", isPerformingAction);
        animator.CrossFade(animation, 0.2f);
    }
}
