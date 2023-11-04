using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldspaceHealthBar : MonoBehaviour
{
    public Health health;
    public Image healthBarImage;
    public Transform healthBarPivot;

    public bool hideFullHealthBar = true;

    private void Awake()
    {
        health = GetComponent<Health>();
    }
    void Update()
    {
        healthBarImage.fillAmount = health.health.Value / health.health.MaxValue;

        healthBarPivot.LookAt(Camera.main.transform.position);

        if (hideFullHealthBar)
            healthBarPivot.gameObject.SetActive(healthBarImage.fillAmount != 1);
    }
}
