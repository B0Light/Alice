using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeneGames
{
    public class MultipleTargetsCameraSystem : MonoBehaviour
    {
        private int currentTargetIndex;

        [SerializeField] private float cameraSpeed = 10f;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Transform[] targets;

        private void Update()
        {
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targets[currentTargetIndex].position, cameraSpeed * Time.deltaTime);
        }

        public void ChangeTargetUp()
        {
            if(currentTargetIndex < targets.Length -1)
            {
                currentTargetIndex += 1;
            }
            else
            {
                currentTargetIndex = 0;
            }
        }

        public void ChangeTargetDown()
        {
            if (currentTargetIndex > 0)
            {
                currentTargetIndex -= 1;
            }
            else
            {
                currentTargetIndex = targets.Length - 1;
            }
        }
    }
}