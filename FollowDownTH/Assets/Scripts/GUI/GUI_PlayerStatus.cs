using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_PlayerStatus : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerStamina playerStamina;
    [SerializeField] private Mana playerMana;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image manaBar;
    [SerializeField] private Image staminaBar;

    private void Update()
    {
        healthBar.fillAmount = playerHealth.health.Value / playerHealth.health.MaxValue;
        manaBar.fillAmount = playerMana.mana.Value / playerMana.mana.MaxValue;
        staminaBar.fillAmount = playerStamina.stamina.Value / playerStamina.stamina.MaxValue;
    }
}
