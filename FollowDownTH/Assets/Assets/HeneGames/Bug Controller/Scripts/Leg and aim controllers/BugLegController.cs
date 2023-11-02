using UnityEngine;

namespace HeneGames.BugController
{
    public class BugLegController : MonoBehaviour
    {
        private float inAirTimer;

        public BugController bugController;
        public CharacterController characterController;
        public float legsHorizontalPositionWhenInAir = 1f;
        public float legsVerticalPositionWhenInAir = 1f;

        private void Update()
        {
            //In air timer
            if (characterController.isGrounded)
            {
                inAirTimer = 0f;
            }
            else
            {
                inAirTimer += Time.deltaTime;
            }

            //If character is in air over 0.3 seconds move legs
            if (inAirTimer > 0.3f)
            {
                bugController.SetLegsHorizontalPosition(true, legsHorizontalPositionWhenInAir);
                bugController.SetLegsVerticalPostition(true, legsVerticalPositionWhenInAir);
            }
            else
            {
                bugController.SetLegsHorizontalPosition(false, legsHorizontalPositionWhenInAir);
                bugController.SetLegsVerticalPostition(false, legsVerticalPositionWhenInAir);
            }
        }
    }
}