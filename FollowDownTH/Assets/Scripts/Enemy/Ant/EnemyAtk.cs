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
    [SerializeField] private bool isPerformingAction;
    [SerializeField] private GameObject enemyWeaponCollider;

    [SerializeField] Transform atkPos;
    [SerializeField] LayerMask atkTarget;
    
    public EnemyAttackPattern currPattern;
    [SerializeField] private List<EnemyAttackPattern> patternMelee;

    [SerializeField] GameObject impactVfx;

    public EnumList.EnemyType enemyType;

    private void Update()
    {
        isPerformingAction = animator.GetBool("isPerformingAction");
        if (isPerformingAction == false)
        {
            if(enemyWeaponCollider)
                enemyWeaponCollider.SetActive(false);
        }
    }
    
    public void Attack_Base()
    {
        if(isAtk == false)
        {
            isAtk = true;
            if (enemyType == EnumList.EnemyType.Ant)
            {
                StartCoroutine("Ant_Attack");
            }
            else if (enemyType == EnumList.EnemyType.HeartSoldier)
            {
                currPattern = patternMelee[Random.Range(0,patternMelee.Count)];
                StartCoroutine(SoldierAttack());
            }
            
        }
    }

    IEnumerator Ant_Attack()
    {
        Debug.Log("DoATK");
        yield return new WaitForSeconds(0.3f);
        animator.SetTrigger("doAttack");
        RaycastHit[] hits = Physics.SphereCastAll(transform.position,
            3f, transform.forward, atkRange, atkTarget);
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
    
    IEnumerator  SoldierAttack()
    {
        if (isPerformingAction)  yield return null;
        enemyWeaponCollider.SetActive(true);
        enemyWeaponCollider.GetComponent<EnemyWeapon>().damage = atkPower + currPattern.damage / 2;
        enemyWeaponCollider.GetComponent<EnemyWeapon>().attackType = currPattern.attackType;
        PlayActionAnimation(currPattern.attackName, true);
        yield return new WaitForSeconds(atkSpd);
        isAtk = false;
    }
    

    void VFX(float impactVfxLifetime)
    {
        if (impactVfx)
        {
            GameObject impactVfxInstance = Instantiate(impactVfx, atkPos.position,
                Quaternion.identity);
            if (impactVfxLifetime > 0)
            {
                Destroy(impactVfxInstance, impactVfxLifetime);
            }
        }
    }
    
    private void PlayActionAnimation(string animation, bool isPerformingAction)
    {
        animator.SetBool("isPerformingAction", isPerformingAction);
        animator.CrossFade(animation, 0.2f);
    }
}
