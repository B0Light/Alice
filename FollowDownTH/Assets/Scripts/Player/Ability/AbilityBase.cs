using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class AbilityBase : MonoBehaviour
{
    public class MyFloatEvent : UnityEvent<float> { }
    public MyFloatEvent OnAbilityUse = new MyFloatEvent();
    [Header("Ability Info")]
    public string title;
    public Sprite icon;
    public float cooldownTime = 1;
    private bool canUse = true;
    [Range(10f, 30f)]
    [SerializeField] private float Mana = 10f;
    [SerializeField] private Stamina _stamina;
    [SerializeField] private KeyCode SkillKey = KeyCode.C;
    private void Awake()
    {
        _stamina = GetComponent<Stamina>();
    }

    private void Update()
    {
        if (Input.GetKey(SkillKey) && _stamina.stamina.Value > Mana)
        {
            TriggerAbility();
        }
    }
    public void TriggerAbility()
    {
        if (canUse)
        {
            OnAbilityUse.Invoke(cooldownTime);
            _stamina.UseStamina(Mana);
            Ability();
            StartCooldown();
        }

    }
    public abstract void Ability();
    void StartCooldown()
    {
        StartCoroutine(Cooldown());
        IEnumerator Cooldown()
        {
            canUse = false;
            yield return new WaitForSeconds(cooldownTime);
            canUse = true;
        }
    }
}
