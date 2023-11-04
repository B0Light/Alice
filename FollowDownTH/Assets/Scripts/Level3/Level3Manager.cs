using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3Manager : MonoBehaviour
{
    [SerializeField]
    GameObject vCamera;

    [SerializeField] private PlayerLocomotionManager playerLocomotionManager;
    [SerializeField] private GameObject bossHealthBar;
    [SerializeField] private Health bossHealth;
    [SerializeField] private BossAttackManager bossAttackManager;
    [SerializeField] private GameObject clearGame;
    [SerializeField] private GameObject wall;

    private bool isGameEnd;
    private void OnTriggerEnter(Collider other)
    {
        WelcomeToBossStage();
    }

    IEnumerator WaitForIt()
    {
        yield return new WaitForSeconds(5.0f);
        vCamera.gameObject.SetActive(false);
        playerLocomotionManager.isPlayerActive = true;
        Invoke("BattlePhase",1f);
    }

    private void WelcomeToBossStage()
    {
        Debug.Log("BossEnter");
        vCamera.gameObject.SetActive(true);
        playerLocomotionManager.isPlayerActive = false;
        StartCoroutine(WaitForIt());
    }

    private void BattlePhase()
    {
        bossHealthBar.SetActive(true);
        wall.SetActive(true);
        StartCoroutine(bossAttackManager.Think());
    }

    private void Update()
    {
        if (bossHealth.isDead && isGameEnd == false)
        {
            ClearGame();
        }
    }

    private void ClearGame()
    {
        isGameEnd = true;
        clearGame.SetActive(true);
    }
    
}