using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Mana : MonoBehaviour
{
    public Gauge<float> mana;
    [SerializeField] float maxMana = 100f;
    [SerializeField] private TextMeshProUGUI potionCount;

    [SerializeField] private Animator animator;
    [SerializeField] private GameObject recoverEffect;
    [SerializeField] private GameObject potion;

    public int manaPotion = 0;

    protected virtual void Start()
    {
        mana = new Gauge<float>(maxMana);
        if(potionCount)
            potionCount.text = manaPotion.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (manaPotion > 0)
            {
                manaPotion--;
                DrinkPotion();
                if(potionCount)
                    potionCount.text = manaPotion.ToString();
            }
            
        }
    }


    protected void RecoverMana(float value)
    {
        if(mana.Value < maxMana)
        {
            mana.Value += value;
        }
    }
    public void UseMana(float value)
    {
        mana.Value -= value;
    }
    
    [ContextMenu("DrinkPotion")]
    public void DrinkPotion()
    {
        potion.SetActive(true);
        RecoverMana(50f);
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
    
    private void PlayActionAnimation(string animation, bool isPerformingAction)
    {
        animator.SetBool("isPerformingAction", isPerformingAction);
        animator.CrossFade(animation, 0.2f);
    }

}