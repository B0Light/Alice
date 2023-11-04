using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Manager : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetTrigger("NextPhase");
        }
    }
}
