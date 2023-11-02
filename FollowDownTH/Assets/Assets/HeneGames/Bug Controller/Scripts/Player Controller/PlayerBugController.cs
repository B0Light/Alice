using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeneGames.BugController
{
    public class PlayerBugController : MonoBehaviour
    {
        private float inAirTimer;

        [Header("References")]
        [SerializeField] private BugController bugController;
        [SerializeField] private PlayerMovementController playerController;
        [SerializeField] private Transform antForwardDir;
        [SerializeField] private Transform camForwardDir;
        [Header("Values")]
        [SerializeField] private float legsPointWhenJump = 0.5f;
        [SerializeField] private float legsHorizontalPointWhenJump = 0.5f;

        private void Update()
        {
            //Aim
            if(Input.GetMouseButton(1))
            {
                bugController.LookTarget(camForwardDir, 5f);
            }
            else
            {
                bugController.LookTarget(antForwardDir, 5f);
            }

            //Is grounded delay
            if (playerController.IsGrounded())
            {
                inAirTimer = 0f;
            }
            else
            {
                inAirTimer += Time.deltaTime;
            }

            //If in air set legs vertical and horizontal positions
            if (inAirTimer > 0.3f)
            {
                bugController.SetLegsVerticalPostition(true, legsPointWhenJump);
                bugController.SetLegsHorizontalPosition(true, legsHorizontalPointWhenJump);

                //If in air set ground alingnment off
                bugController.SetGroundAlignment(false);
            }
            else
            {
                bugController.SetLegsVerticalPostition(false, legsPointWhenJump);
                bugController.SetLegsHorizontalPosition(false, legsHorizontalPointWhenJump);

                //If in ground set ground alingnment on
                bugController.SetGroundAlignment(true);
            }
        }
    }
}