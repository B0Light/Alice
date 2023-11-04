using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngineInternal;

public class Health : MonoBehaviour
{
    public Gauge<float> health;
    [SerializeField] float maxHealth = 100f;
    protected MeshRenderer itemMesh;
    protected SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private Color damagedColor;

    [SerializeField] private List<float> CoefValue = new List<float>(){1.0f,1.0f,1.0f,1.0f};
    public bool isDead = false;
    public bool isDmg = false;
    
    
    [Header("Damage FX")]
    public GameObject bloodSplatterFX;
    
    protected virtual void Awake()
    {
        itemMesh = GetComponentInChildren<MeshRenderer>();
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        health = new Gauge<float>(maxHealth);
    }

    public void RecoverHealth(float value)
    {
        if (isDead) return;
        if (health.Value < health.MaxValue)
        {
            health.Value += value;
        }
    }

    public virtual void TakeDmg(float dmg, int attackType = 0)
    {
        if (isDead) {
            return;
        }
        
        if(isDmg == false)
        {
            StartCoroutine("OnDmg",CalculateDamage(dmg, attackType));
        }
    }

    private float CalculateDamage(float dmg, int attackType)
    {
        return dmg * CoefValue[attackType];
    }
            

    IEnumerator OnDmg(float dmg)
    {
        isDmg = true;
        health.Value -= dmg;

        skinnedMeshRenderer.material.color = damagedColor;

        if (health.Value <= 0) { 
            isDead = true; 
        }

        yield return new WaitForSeconds(0.2f);
        isDmg = false;

        if(isDead == false)
        {
            skinnedMeshRenderer.material.color = Color.white;
        }
        else
        {
            skinnedMeshRenderer.material.color = Color.gray;
            health.Value = 0;
        }
    }

    public void getHealth(float value)
    {
        health.MaxValue += value;
    }
   
}
