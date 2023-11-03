using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2Manager : MonoBehaviour
{
    [SerializeField]
    private PlayerLocomotionManager playerLocomotionManager;

    [SerializeField] private GameObject backgroundSkyBox;
    [SerializeField] private CameraShaker cameraShaker;
    

    private void Start()
    {
        playerLocomotionManager.FollowDownTheRabbitHole();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("PlayerEnterNextPhase");
            cameraShaker.StartShake(1,0.05f);
            ChangeSky();
        }
    }

    private void ChangeSky()
    {
        backgroundSkyBox.SetActive(false);
    }
}
