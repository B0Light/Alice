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
                RecoverMana(100f);
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

}