using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class BossAttackManager : MonoBehaviour
{
    private BossManager _bossManager;

    private GameObject _player;
    [SerializeField] private Animator animator;
    [SerializeField] private bool isPerformingAction;

    [SerializeField] private Rigidbody _rigidbody;

    private NavMeshAgent _navMeshAgent;

    [Header("EnemyWeapon")]
    [SerializeField] private GameObject enemyWeaponCollider;

    [Header("BossPattern")]
    private BossPattern currPattern;
    [SerializeField] private BossPattern patternMelee;

    private void Start()
    {
        _bossManager = GetComponent<BossManager>();
        _navMeshAgent = GetComponent<NavMeshAgent>();

        StartCoroutine(Think());
    }

    private void Update()
    {
        isPerformingAction = animator.GetBool("isPerformingAction");
    }

    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f);
        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player");
        }
        else
        {
            float distance = Vector3.Distance(this.transform.position, _player.transform.position);
            StartCoroutine(RotateTowardsTarget(this.transform, _player.transform, 0.5f));
            currPattern = patternMelee;
            if (distance < patternMelee.range)
            {
                Attack();
            }
            else
            {
                MoveToPlayer();
            }
        }
        
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
        enemyWeaponCollider.SetActive(true);
        transform.LookAt(_player.transform);
        PlayActionAnimation(currPattern.attackType, true);
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

    private void OnAnimatorMove()
    {
        if (isPerformingAction)
        {
            _rigidbody.drag = 0;
            Vector3 deltaPosition = animator.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / Time.deltaTime;
            _rigidbody.velocity = velocity;
        }
    }
}
