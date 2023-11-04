using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnter : MonoBehaviour
{
    [SerializeField]
    GameObject vCamera;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("BossEnter");
        vCamera.gameObject.SetActive(true);
        StartCoroutine(WaitForIt());
    }

    IEnumerator WaitForIt()
    {
        yield return new WaitForSeconds(5.0f);
        vCamera.gameObject.SetActive(false);
    }
}