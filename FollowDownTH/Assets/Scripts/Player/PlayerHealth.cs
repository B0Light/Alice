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
    
    [SerializeField] private GameObject recoverEffect;
    [SerializeField] private GameObject potion;

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

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (healthPotion >= 1)
            {
                healthPotion--;
                DrinkPotion();
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
        VFX(impactVfx,1f);
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
    
    [ContextMenu("DrinkPotion")]
    public void DrinkPotion()
    {
        potion.SetActive(true);
        RecoverHealth(50f);
        PlayActionAnimation("Drink",true);
        StartCoroutine(RemovePotion());
    }
    
    IEnumerator RemovePotion()
    {
        yield return new WaitForSeconds(1f);
        potion.SetActive(false);
    }
    
    
    void HealPlayerFromEffect()
    {
        VFX(recoverEffect,3f);
    }
    
    void VFX(GameObject vfx, float impactVfxLifetime)
    {
        if (vfx != null)
        {
            GameObject impactVfxInstance = Instantiate(vfx, transform);
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
