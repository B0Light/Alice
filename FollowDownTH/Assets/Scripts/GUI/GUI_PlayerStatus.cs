using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_PlayerStatus : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerStamina playerStamina;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image manaBar;
    [SerializeField] private Image staminaBar;

    private void Update()
    {
        healthBar.fillAmount = playerHealth.health.Value / playerHealth.health.MaxValue;
        staminaBar.fillAmount = playerStamina.stamina.Value / playerStamina.stamina.MaxValue;
    }
}
