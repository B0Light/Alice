using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEffectsManager : MonoBehaviour
{
    [Header("Damage FX")]
    public GameObject bloodSplatterFX;

    public virtual void PlayBloodSplatterFX(Vector3 bloodSplatterLocation)
    {
        GameObject blood = Instantiate(bloodSplatterFX, bloodSplatterLocation, Quaternion.identity);
    }
}
