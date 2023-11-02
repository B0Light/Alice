using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeneGames.BugController
{
    public class Food : MonoBehaviour
    {
        private bool eaten;

        [SerializeField] private ParticleSystem eatEffect;
        [SerializeField] private MeshRenderer foodModel;

        private void Update()
        {
            if(eaten)
            {
                if(!eatEffect.isPlaying)
                {
                    Destroy(gameObject);
                }
            }
        }

        public void Eat()
        {
            if (eatEffect != null)
            {
                eatEffect.Play();
                eaten = true;
                foodModel.enabled = false;
            }
        }

        public bool Eaten()
        {
            return eaten;
        }
    }
}