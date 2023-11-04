using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger01 : MonoBehaviour
{
    [SerializeField] private GameObject skybox;
    [SerializeField] private GameObject vCam;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Animator>().CrossFade("Falling", 0.2f);
            skybox.SetActive(false);
            vCam.SetActive(true);
            Invoke("NextScene",1f);
        }
    }

    private void NextScene()
    {
        SceneManager.LoadScene("Stage02");
    }
}
