using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    public bool isRecoverable;
    public Gauge<float> stamina;
    [SerializeField] float maxStamina = 100f;

    protected virtual void Start()
    {
        stamina = new Gauge<float>(maxStamina);
    }

    protected void FixedUpdate()
    {
        if(isRecoverable)
        {
            RecoverStamina(0.5f);
        }
    }

    protected void RecoverStamina(float value)
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
