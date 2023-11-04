using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : Health
{
    private Animator animator;
    private PlayerLocomotionManager playerLocomotionManager;
    [SerializeField] private Color parryColor;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TextMeshProUGUI potionCount;
    
    [SerializeField] private GameObject impactVfx;
    [SerializeField] private Transform impactPos;

    public int healthPotion;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        if(potionCount)
            potionCount.text = healthPotion.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            DamageDebugger();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (healthPotion >= 1)
            {
                healthPotion--;
                RecoverHealth(50f);
                if(potionCount)
                    potionCount.text = healthPotion.ToString();
            }
            
        }
    }

    public void DamageDebugger()
    {
        DamagedHandler(2, 10);
    }

    public void DamagedHandler(int attackType, float dmg)
    {
        if(isDead) return;
        // attackType 1 : horizontal
        // attackType 2 : vertical
        switch (playerLocomotionManager.curPerformingAction)
        {
            case EnumList.States.Idle:
                TakeDmg(dmg);
                break;
            case EnumList.States.HorizontalAttack:
                if(attackType == 2)
                {
                    StartCoroutine(OnParry());
                }
                else
                {
                    TakeDmg(dmg);
                }
                break;
            case EnumList.States.VerticalAttack:
                if (attackType == 1)
                {
                    StartCoroutine(OnParry());
                }
                else
                {
                    TakeDmg(dmg);
                }
                break;
            case EnumList.States.Dodge:
                break;
        }
    }

    IEnumerator OnParry()
    {
        PlayActionAnimation("Parrying", true);
        playerLocomotionManager.curPerformingAction = EnumList.States.Parry;
        StartCoroutine(VFX(1f));
        if(itemMesh)
            itemMesh.material.color = parryColor;
        yield return new WaitForSeconds(0.2f);
        if(itemMesh)
            itemMesh.material.color = Color.white;
    }

    public override void TakeDmg(float dmg, int attackType = 0)
    {
        base.TakeDmg(dmg, attackType);

        playerLocomotionManager.curPerformingAction = EnumList.States.Damaged;
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
            gameOverUI.SetActive(true);
            StartCoroutine(GameOverSceneChange());
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
    
    IEnumerator VFX(float impactVfxLifetime)
    {
        yield return new WaitForSeconds(0.1f);
        if (impactVfx)
        {
            GameObject impactVfxInstance = Instantiate(impactVfx, impactPos);
            if (impactVfxLifetime > 0)
            {
                Destroy(impactVfxInstance, impactVfxLifetime);
            }
        }
    }

    IEnumerator GameOverSceneChange()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("LobbyLevel");
    }
    
    private void PlayActionAnimation(string animation, bool isPerformingAction)
    {
        animator.SetBool("isPerformingAction", isPerformingAction);
        animator.CrossFade(animation, 0.2f);
    }
}
