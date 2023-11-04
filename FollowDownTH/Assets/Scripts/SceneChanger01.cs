using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger01 : MonoBehaviour
{
    [SerializeField] private GameObject skybox;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Animator>().CrossFade("Falling", 0.2f);
            skybox.SetActive(false);
            Invoke("NextScene",1.5f);
        }
    }

    private void NextScene()
    {
        SceneManager.LoadScene(1);
    }
}
