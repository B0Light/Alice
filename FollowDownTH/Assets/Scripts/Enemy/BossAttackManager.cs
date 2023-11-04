using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static Cinemachine.CinemachineTargetGroup;
using static UnityEngine.GraphicsBuffer;

public class BossAttackManager : MonoBehaviour
{
    private BossManager _bossManager;

    private GameObject _player;
    [SerializeField] private Animator animator;
    [SerializeField] private bool isPerformingAction;
    [SerializeField] private EnemyHealth enemyHealth;
    
    private NavMeshAgent _navMeshAgent;

    [Header("EnemyWeapon")] 
    [SerializeField] private GameObject weaponHat;
    [SerializeField] private GameObject enemyWeaponCollider;
    [SerializeField] private GameObject weaponStaff;

    [Header("BossPattern")]
    public EnemyAttackPattern currPattern;

    [SerializeField] private EnemyAttackPattern patternRange;
    [SerializeField] private List<EnemyAttackPattern> patternMelee;

    [Header("RangeAttack")]
    private float fireCooldown = 0.1f;
    public Transform firePoint;
    public GameObject prefabCast;
    private ParticleSystem Effect;
    public float fireRate = 0.1f;
    public Vector2 uiOffset = new Vector2(0,1);

    private void Start()
    {
        _bossManager = GetComponent<BossManager>();
        _navMeshAgent = GetComponent<NavMeshAgent>();

        //StartCoroutine(Think());
    }

    private void Update()
    {
        isPerformingAction = animator.GetBool("isPerformingAction");
        if (isPerformingAction == false)
        {
            enemyWeaponCollider.SetActive(false);
        }

        if (enemyHealth.isDead)
        {
            StopAllCoroutines();
        }
    }

    public IEnumerator Think()
    {
        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player");
        }
        else
        {
            float distance = Vector3.Distance(this.transform.position, _player.transform.position);
            StartCoroutine(RotateTowardsTarget(this.transform, _player.transform, 0.3f));
            currPattern = patternMelee[Random.Range(0,patternMelee.Count)];
            if (distance < currPattern.range)
            {
                Attack();
            }
            else
            {
                //EnterSecondPhase();
                RangeAttackHandler();
            }
        }
        yield return new WaitForSeconds(2f);
        StartCoroutine(Think());
    }


    private void MoveToPlayer()
    {
        if (isPerformingAction) return;
        Vector3 newDestination = _player.transform.position + (_navMeshAgent.transform.forward * currPattern.range);
        _navMeshAgent.SetDestination(newDestination);
        animator.SetBool("isWalk", true);
    }

    private void Attack()
    {
        if (isPerformingAction) return;
        _navMeshAgent.ResetPath();
        enemyWeaponCollider.GetComponent<EnemyWeapon>().damage = currPattern.damage;
        enemyWeaponCollider.SetActive(true);
        transform.LookAt(_player.transform);
        PlayActionAnimation(currPattern.attackName, true);
    }

    private void Parried()
    {
        if (isPerformingAction)
        {
            
        }
    }

    private void EnterSecondPhase()
    {
        if(isPerformingAction) return;
        PlayActionAnimation("TakeOnHat",true);
        StartCoroutine(RangeAttackCombo());
    }

    public void ChangeWeapon()
    {
        weaponStaff.SetActive(false);
        weaponHat.SetActive(true);
    }

    private void RangeAttackHandler()
    {
        if(isPerformingAction) return;
        PlayActionAnimation("RangeAttack",true);
        currPattern = patternRange;
        StartCoroutine(RangeAttackCombo());
    }
    IEnumerator RangeAttackCombo()
    {
        yield return new WaitForSeconds(0.5f);
        for(int i = 0; i < 5; i++)
        {
            GameObject projectile = Instantiate(prefabCast, firePoint.position, firePoint.rotation);
            projectile.GetComponent<HatProjectile>().UpdateTarget(_player.transform, (Vector3)uiOffset);
            projectile.GetComponentInChildren<EnemyWeapon>().damage = currPattern.damage;
            projectile.GetComponentInChildren<EnemyWeapon>().attackType = currPattern.attackType;
            Effect = prefabCast.GetComponent<ParticleSystem>();
            Effect.Play();
            //camShake?
            yield return new WaitForSeconds(fireCooldown);
        }
    }


    IEnumerator RotateTowardsTarget(Transform aTransform, Transform bTransform, float duration)
    {
        Vector3 directionToTarget = bTransform.position - aTransform.position;
        directionToTarget.y = 0;

        Quaternion startRotation = aTransform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            aTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        aTransform.rotation = targetRotation;
    }


    private void PlayActionAnimation(string animation, bool isPerformingAction)
    {
        animator.SetBool("isPerformingAction", isPerformingAction);
        animator.CrossFade(animation, 0.2f);
    }
    
}
