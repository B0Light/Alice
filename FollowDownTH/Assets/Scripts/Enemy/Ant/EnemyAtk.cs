using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAtk : MonoBehaviour
{
    private bool isAtk = false;
    [Header("atk Option")]
    [Range(1f, 5f)]  public float atkRange = 1f;
    [Range(1f, 20f)] public float atkPower = 1f;
    [SerializeField] float atkSpd = 1f;
    [SerializeField] private Animator animator;

    [SerializeField] Transform atkPos;
    [SerializeField] LayerMask atkTarget;

    [SerializeField] GameObject impactVfx;


    public void Attack_Base()
    {
        if(isAtk == false)
        {
            isAtk = true;
            StartCoroutine("Attack_punch");
        }
    }

    IEnumerator Attack_punch()
    {
        Debug.Log("DoATK");
        yield return new WaitForSeconds(0.3f);
        animator.SetTrigger("doAttack");
        RaycastHit[] hits = Physics.SphereCastAll(transform.position,
            5f, transform.forward, atkRange, atkTarget);
        foreach (var hit in hits)
        {
            Health health = hit.collider.GetComponent<Health>();
            if (health)
            {
                VFX(0.5f);
                health.TakeDmg(atkPower, 0);
            }
        }

        yield return new WaitForSeconds(atkSpd);
        isAtk = false;
    }

    void VFX(float impactVfxLifetime)
    {
        if (impactVfx)
        {
            GameObject impactVfxInstance = Instantiate(impactVfx, atkPos.position,
                Quaternion.LookRotation(atkPos.position));
            if (impactVfxLifetime > 0)
            {
                Destroy(impactVfxInstance, impactVfxLifetime);
            }
        }
    }
}
