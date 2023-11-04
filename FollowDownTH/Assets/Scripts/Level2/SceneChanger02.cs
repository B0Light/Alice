using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger02 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Invoke("NextScene",1f);
        }
    }

    private void NextScene()
    {
        SceneManager.LoadScene("Stage03");
    }
}
