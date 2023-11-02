using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeneGames.BugController
{
    public class PlayerMovementController : MonoBehaviour
    {
        private Vector3 moveDirection;
        private Vector3 smoothMoveDirection;
        private Vector3 smoothMoveVelocity;
        private CharacterController controller;
        private float currentSpeed;
        private float currentMovementSmoothnes;

        [Header("Components")]

        [SerializeField] private Transform cameraBase;

        [SerializeField] private GameObject playerModel;

        [Header("Variables")]

        [Range(0.01f, 0.5f)]
        [SerializeField] private float movementSmoothnes = 0.1f;

        [SerializeField] private float jumpforce = 4f;

        [SerializeField] private float walkSpeed = 3f;

        [SerializeField] private float runSpeed = 5f;

        [SerializeField] private float gravity = 0.8f;

        [SerializeField] private float turnSpeed = 5f;

        [Header("Keys")]
        [SerializeField] private KeyCode runKey = KeyCode.LeftShift;

        [SerializeField] private KeyCode jumpKey = KeyCode.Space;

        void Start()
        {
            controller = GetComponent<CharacterController>();
        }

        void Update()
        {
            Movement();
        }

        private void Movement()
        {
            //Current speed
            if (Input.GetKey(runKey))
            {
                currentSpeed = runSpeed;
                currentMovementSmoothnes = movementSmoothnes * 3f;
            }
            else
            {
                currentSpeed = walkSpeed;
                currentMovementSmoothnes = movementSmoothnes;
            }

            //Movement
            smoothMoveDirection = Vector3.SmoothDamp(smoothMoveDirection, moveDirection, ref smoothMoveVelocity, currentMovementSmoothnes);

            float yStore = moveDirection.y;
            moveDirection = (transform.forward * Input.GetAxisRaw("Vertical")) + (transform.right * Input.GetAxisRaw("Horizontal"));
            moveDirection = moveDirection.normalized * currentSpeed;
            moveDirection.y = yStore;

            Vector3 moveThisDirection = new Vector3(smoothMoveDirection.x, moveDirection.y, smoothMoveDirection.z);
            controller.Move(moveThisDirection * Time.deltaTime);

            //Jump
            if (controller.isGrounded)
            {
                moveDirection.y = -1f;

                if (Input.GetKeyDown(jumpKey))
                {
                    moveDirection.y = jumpforce;
                }
                else
                {
                    moveDirection.y = moveDirection.y + (Physics.gravity.y * gravity * Time.deltaTime);
                }
            }

            //Gravity
            moveDirection.y = moveDirection.y + (Physics.gravity.y * gravity * Time.deltaTime);

            //Move the player based on the camera direction
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                Quaternion _playerRotation = Quaternion.Euler(0f, cameraBase.rotation.eulerAngles.y, 0f);
                transform.rotation = Quaternion.Slerp(transform.rotation, _playerRotation, turnSpeed /2f * Time.deltaTime);

                Quaternion _newRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));
                playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, _newRotation, turnSpeed * Time.deltaTime);
            }
        }

        public bool IsGrounded()
        {
            return controller.isGrounded;
        }
    }
}