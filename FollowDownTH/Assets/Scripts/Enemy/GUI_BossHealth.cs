using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_BossHealth : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private Health bossHealth;

    private void Update()
    {
        healthBar.fillAmount = bossHealth.health.Value / bossHealth.health.MaxValue;
    }
}
