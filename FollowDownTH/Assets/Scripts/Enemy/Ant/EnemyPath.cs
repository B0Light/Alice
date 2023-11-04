using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyPath : MonoBehaviour
{
    public NavMeshAgent navMesh = null;
    EnemyManager antManager;
    EnemyAtk enemyAtk;
    
    public float stopDistance = 3.0f;

    private int currNode = 0;
    [SerializeField, Range(1f,10f)] private float fov;
    [SerializeField] private LayerMask targetLayer;

    [Header("Soldier")] 
    [SerializeField] private Animator animator;

    private void Awake()
    {
        antManager = GetComponent<EnemyManager>();
        enemyAtk = GetComponent<EnemyAtk>();
        navMesh = GetComponent<NavMeshAgent>();
    }
    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
        if (navMesh == null) return;
        if(antManager.isDead) return;

        if(antManager.target != null)
        {
            Move(antManager.target.transform);
            if (navMesh.remainingDistance <= stopDistance)
            {
                navMesh.isStopped = true;
                AtkReady();
            }
            else
            {
                navMesh.isStopped = false;
            }
        }
        
        if (navMesh.velocity == Vector3.zero)
        {
            MoveToNextNode();
            if(animator)
                animator.SetBool("isWalk",false);
        }
        else
        {
            Sight();
            if (animator)
            {
                animator.SetBool("isWalk",true);
            }
        }
    }

    void MoveToNextNode()
    {
        if (antManager.isDead) return;
        if (antManager.target != null) return;

        if (navMesh.velocity == Vector3.zero)
        {
            currNode = Random.Range(0, antManager.enemyPath.Length);
            Move(antManager.enemyPath[currNode]);
        }
    }

    private void Sight()
    {
        if (antManager.isDead) return;
        Collider[] cols = Physics.OverlapSphere(transform.position, fov, targetLayer);

        if (cols.Length > 0) // Follow Enmey
        {
            Debug.Log("FIND Target");
            antManager.target = cols[0].gameObject; 
        } 
        else                
        {
            if (antManager.target != null)
            {
                antManager.target = null;
                MoveToNextNode();
            }
        }
    }

    void Move(Transform destination)
    {
        if (antManager.isDead) return;
        if (navMesh == null) return;

        navMesh.SetDestination(destination.position);
        gameObject.transform.LookAt(destination.position);
    }

    private void AtkReady()
    {
        if (antManager.isDead) return;
        
        Collider[] cols = Physics.OverlapSphere(transform.position, 5f, targetLayer);
        if (cols.Length > 0) // Atk Ready
        {
            enemyAtk.Attack_Base();
            navMesh.ResetPath();
        }
    }
}
