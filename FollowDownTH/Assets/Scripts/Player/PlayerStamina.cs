using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStamina : Stamina
{
    private Animator animator;
    [SerializeField] private GameObject recoverEffect;
    [SerializeField] private GameObject potion;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    [ContextMenu("DrinkPotion")]
    public void DrinkPotion()
    {
        potion.SetActive(true);
        RecoverStamina(100f);
        PlayActionAnimation("Drink",true);
        StartCoroutine(RemovePotion());

    }

    IEnumerator RemovePotion()
    {
        yield return new WaitForSeconds(1f);
        potion.SetActive(false);
    }

    private void PlayActionAnimation(string animation, bool isPerformingAction)
    {
        animator.SetBool("isPerformingAction", isPerformingAction);
        animator.CrossFade(animation, 0.2f);
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
}
