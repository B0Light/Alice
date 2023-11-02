using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeneGames.BugController
{
    public class LegEventsExample : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] stepSounds;

        public void FootHitGround()
        {
            float _randomPitch = Random.Range(0.8f, 1.2f);
            int _randomInt = Random.Range(0, stepSounds.Length);

            audioSource.PlayOneShot(stepSounds[_randomInt]);
            audioSource.pitch = _randomPitch;
        }
    }
}