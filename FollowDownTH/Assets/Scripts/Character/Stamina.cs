using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MonoBehaviour
{

    public Gauge<float> stamina;
    [SerializeField] float maxStamina = 100f;

    void Start()
    {
        stamina = new Gauge<float>(maxStamina);
    }

    void FixedUpdate()
    {
        {
            RecoverStamina(0.5f);
        }
    }

    private void RecoverStamina(float value)
    {
        if(stamina.Value < maxStamina)
        {
            stamina.Value += value;
        }
    }
    public void UseStamina(float value)
    {
        stamina.Value -= value;
    }

    public void GetStamina(float value)
    {
        stamina.MaxValue = value;
    }
}
