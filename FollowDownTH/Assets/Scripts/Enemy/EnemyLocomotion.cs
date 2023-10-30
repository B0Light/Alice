using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyLocomotion : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private GameObject _player;

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }


}
