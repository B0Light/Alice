using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeneGames.BugController
{
    public class BugAimController : MonoBehaviour
    {
        [SerializeField] private BugController bugController;
        [SerializeField] private Transform forwardLookPos;
        [SerializeField] private Transform aimLookPos;
        [SerializeField] private float aimSpeed = 10f;

        private void Update()
        {
            if(Input.GetMouseButton(1))
            {
                bugController.LookTarget(aimLookPos, aimSpeed);
            }
            else
            {
                bugController.LookTarget(forwardLookPos, aimSpeed);
            }
        }
    }
}