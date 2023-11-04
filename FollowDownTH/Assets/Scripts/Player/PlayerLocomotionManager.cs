using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerLocomotionManager : MonoBehaviour
{
    Animator animator;
    private CharacterController characterController;

    
    public EnumList.States curPerformingAction;

    //INPUT VARIABLES
    [Header("INPUTS")]
    [SerializeField] float verticalMovement;
    [SerializeField] float horizontalMovement;
    [SerializeField] float mouseX;
    [SerializeField] float mouseY;

    [Header("Component")]
    [SerializeField] private PlayerAttackManager playerAttackManager;

    [SerializeField] private PlayerStamina playerStamina;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Mana playerMana;
     
    //PLAYER VARIABLES
    [Header("Player")]
    [SerializeField] bool isLockOn;
    [SerializeField] bool isSprinting;
    public bool isRunning;
    [SerializeField] bool isWalking;

    public bool isAttack;
    public bool isCombo;

    public bool isTwoHandingWeapon;

    [SerializeField] float rotationSpeed = 20f;
    [SerializeField] float sprintSpeed = 6f;
    [SerializeField] float runningSpeed = 4f;
    [SerializeField] float walkingSpeed = 1.5f;

    [Header("Player Debug")]
    [SerializeField] float moveAmount;
    [SerializeField] bool isPerformingAction;
    [SerializeField] bool isPerformingBackStep;
    Vector3 moveDirection;
    Vector3 planeNormal;

    [Header("Cinematic")]
    public bool isPlayerActive = true;

    //CAMERA VARIABLES
    [Header("Camera")]
    [SerializeField] float leftAndRightLookSpeed = 500f;
    [SerializeField] float upAndDownLookSpeed = 500f;
    [SerializeField] float minimumPivot = -35f;
    [SerializeField] float maximumPivot = 35f;
    [SerializeField] GameObject playerCamera;
    [SerializeField] GameObject playerCameraPivot;
    [SerializeField] CinemachineVirtualCamera cameraObject;
    [Header("Camera Debug")]
    [SerializeField] float leftandRightLookAngle;
    [SerializeField] float upAndDownLookAngle;
    Vector3 cameraFollowVelocity = Vector3.zero;
    
    [Header("Targeting")]
    public float sphereRadius = 10f;
    public LayerMask enemyLayer;
    private Transform playerTransform;
    [SerializeField] private Transform targetTransform;

    //ATTACK VARIABLES
    public string attackLastPerformed;
    [SerializeField] private bool horizontalAttackInput;
    [SerializeField] private bool verticalAttackInput;
    [SerializeField] private bool chargeAttackInput;
    [SerializeField] private bool parryingInput;

    [Header("Weapon")]
    public ParticleSystem weaponFX;
    public ParticleSystem weaponChargeFX;

    [Header("Dodge")]
    private Vector3 rollDirection;
    [SerializeField] float dodgeStaminaCost = 25;
    [SerializeField] private float backStaminaCost = 10;

    [Header("AttackCost")] 
    private float horizontalCost = 10;
    private float verticalCost = 15;
    private float chargeCost = 20;

    public bool canRecovering = true;

    string oh_Light_Attack_01 = "OH_Light_Attack_01";
    string oh_Light_Attack_02 = "OH_Light_Attack_02";
    string oh_Heavy_Attack_01 = "OH_Heavy_Attack_01";
    string oh_Heavy_Attack_02 = "OH_Heavy_Attack_02";

    string th_Light_Attack_01 = "TH_Light_Attack_01";
    string th_Light_Attack_02 = "TH_Light_Attack_02";
    string th_Heavy_Attack_01 = "TH_Heavy_Attack_01";
    string th_Heavy_Attack_02 = "TH_Heavy_Attack_02";

    string oh_Charge_Attack_01 = "OH_Charge_Attack_01_Wind_Up";
    string oh_Charge_Attack_02 = "OH_Charge_Attack_02_Wind_Up";
    string th_Charge_Attack_01 = "TH_Charge_Attack_01_Wind_Up";
    string th_Charge_Attack_02 = "TH_Charge_Attack_02_Wind_Up";
    
    [SerializeField] private KeyCode key_Dodge = KeyCode.Space;
    [SerializeField] private KeyCode key_ChangeWeapon = KeyCode.Y;
    [SerializeField] private KeyCode key_Sprint = KeyCode.LeftShift;
    [SerializeField] private KeyCode key_ChargeAttack = KeyCode.Q;
    [SerializeField] private KeyCode key_MoveType = KeyCode.Tab;
    [SerializeField] private KeyCode key_Targeting = KeyCode.E;
    [SerializeField] private KeyCode key_Giant = KeyCode.Alpha1;
    [SerializeField] private KeyCode key_Small = KeyCode.Alpha2;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        playerTransform = transform;
        CursorLock();
    }

    private void Update()
    {
        if (isPlayerActive)
        {
            HandleInputs();
            UpdateAnimatorParameters();
            isPerformingAction = animator.GetBool("isPerformingAction");
            isPerformingBackStep = animator.GetBool("isPerformingBackStep");
            if (isPerformingAction == false)
            {
                curPerformingAction = EnumList.States.Idle;
                animator.SetBool("isCombo", false);
                isAttack = false;
                isCombo = false;
                if(canRecovering)
                    playerStamina.isRecoverable = true;
            }
            else
            {
                playerStamina.isRecoverable = false;
            }
        }

        if (targetTransform == null)
        {
            isLockOn = false;
        }
    }

    private void FixedUpdate()
    {
        if (!isPerformingAction)
        {
            HandleAllPlayerLocomotion();
        }
    }

    private void LateUpdate()
    {
        HandleCameraActions();
    }

    private void UpdateAnimatorParameters()
    {
        float snappedVertical;
        float snappedHorizontal;

        #region Horizontal
        //This if chain will round the horizontal movement to -1, -0.5, 0, 0.5 or 1

        if (horizontalMovement > 0 && horizontalMovement <= 0.5f)
        {
            snappedHorizontal = 0.5f;
        }
        else if (horizontalMovement > 0.5f)
        {
            snappedHorizontal = 1;
        }
        else if (horizontalMovement < 0 && horizontalMovement >= -0.5f)
        {
            snappedHorizontal = -0.5f;
        }
        else if (horizontalMovement < -0.5f)
        {
            snappedHorizontal = -1;
        }
        else
        {
            snappedHorizontal = 0;
        }

        #endregion

        #region Vertical
        //This if chain will round the vertical movement to -1, -0.5, 0, 0.5 or 1

        if (verticalMovement > 0 && verticalMovement <= 0.5f)
        {
            snappedVertical = 0.5f;
        }
        else if (verticalMovement > 0.5f)
        {
            snappedVertical = 1;
        }
        else if (verticalMovement < 0 && verticalMovement >= -0.5f)
        {
            snappedVertical = -0.5f;
        }
        else if (verticalMovement < -0.5f)
        {
            snappedVertical = -1;
        }
        else
        {
            snappedVertical = 0;
        }

        #endregion

        if (isSprinting)
        {
            isLockOn = false;
            animator.SetFloat("Horizontal", 0, 0.2f, Time.deltaTime);
            animator.SetFloat("Vertical", 2, 0.2f, Time.deltaTime);
        }
        else
        {
            if (isLockOn)
            {
                if (isWalking)
                {
                    animator.SetFloat("Vertical", snappedVertical / 2, 0.2f, Time.deltaTime);
                    animator.SetFloat("Horizontal", snappedHorizontal / 2, 0.2f, Time.deltaTime);
                }
                else
                {
                    animator.SetFloat("Vertical", snappedVertical, 0.2f, Time.deltaTime);
                    animator.SetFloat("Horizontal", snappedHorizontal, 0.2f, Time.deltaTime);
                }
            }
            else
            {
                if (isWalking)
                {
                    animator.SetFloat("Vertical", moveAmount / 2, 0.2f, Time.deltaTime);
                    animator.SetFloat("Horizontal", 0, 0.2f, Time.deltaTime);
                }
                else
                {
                    animator.SetFloat("Vertical", moveAmount, 0.2f, Time.deltaTime);
                    animator.SetFloat("Horizontal", 0, 0.2f, Time.deltaTime);
                }
            }
        }

        if (moveAmount == 0)
        {
            animator.SetFloat("Vertical", 0, 0.2f, Time.deltaTime);
            animator.SetFloat("Horizontal", 0, 0.2f, Time.deltaTime);
        }
    }

    private void HandleInputs()
    {
        HandleLightAttackCombo();
        HandleHeavyAttackCombo();
        HandleChargeAttackCombo();
        HandleBackStepAttack();

        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        HandleGiantWeapon();
        HandleSmallBody();
        
        HandleDodge();
        HandleSprint();
        HandleWalkOrRun();
        HandleLockOn();
        if (playerAttackManager.isEquipWeapon)
        {
            HandleTwoHand();
            HandleLightAttack();
            HandleHeavyAttack();
            HandleChargeAttack();
        }
        

        if (Input.GetKey(KeyCode.W))
        {
            verticalMovement = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            verticalMovement = -1;
        }
        else
        {
            verticalMovement = 0;
        }

        if (Input.GetKey(KeyCode.D))
        {
            horizontalMovement = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            horizontalMovement = -1;
        }
        else
        {
            horizontalMovement = 0;
        }

        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalMovement) + Mathf.Abs(verticalMovement));
    }

    private void HandleAllPlayerLocomotion()
    {
        HandlePlayerRotation();
        HandlePlayerMovement();
    }

    private void HandleGiantWeapon()
    {
        if (Input.GetKeyDown(key_Giant))
        {
            if (playerMana.mana.Value >= 30 && playerAttackManager.isEquipWeapon)
            {
                playerMana.UseMana(30f);
                playerAttackManager.GiantWeapon();
            }
            
        }
            
    }

    private void HandleSmallBody()
    {
        if (Input.GetKeyDown(key_Small))
        {
            if (playerMana.mana.Value >= 20 && playerAttackManager.isEquipWeapon)
            {
                playerMana.UseMana(20f);
                gameObject.transform.localScale = (Vector3.one * 0.1f);
                StartCoroutine(ResetWeaponSize());
            }
        }
            
    }
    
    IEnumerator ResetWeaponSize()
    {
        yield return new WaitForSeconds(5f);
        gameObject.transform.localScale = Vector3.one;
    }

    private void HandleTwoHand()
    {
        if (Input.GetKeyDown(key_ChangeWeapon))
        {
            isTwoHandingWeapon = !isTwoHandingWeapon;
            animator.SetBool("isTwoHandingWeapon", isTwoHandingWeapon);
        }
    }

    private void HandleLightAttack()
    {
        if (isPerformingAction)
            return;
        if(playerStamina.stamina.Value < horizontalCost)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            playerStamina.UseStamina(horizontalCost);
            isAttack = true;
            horizontalAttackInput = true;
            weaponFX.Stop();
            weaponFX.Play();
        }

        if (horizontalAttackInput)
        {
            curPerformingAction = EnumList.States.HorizontalAttack;
            horizontalAttackInput = false;

            if (isTwoHandingWeapon)
            {
                if (isSprinting)
                {
                    PlayActionAnimation("TH_Running_Attack_01", true);
                    return;
                }

                PlayActionAnimation("TH_Light_Attack_01", true);
                attackLastPerformed = th_Light_Attack_01;
            }
            else
            {
                if (isSprinting)
                {
                    PlayActionAnimation("OH_Running_Attack_01", true);
                    return;
                }

                PlayActionAnimation("OH_Light_Attack_01", true);
                attackLastPerformed = oh_Light_Attack_01;
            }
        }
    }

    private void HandleLightAttackCombo()
    {
        if(playerStamina.stamina.Value < horizontalCost)
            return;
        
        if (isPerformingAction)
        {
            if (attackLastPerformed == oh_Light_Attack_01)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    animator.SetBool("isCombo", true);
                    isCombo = true;
                    playerStamina.UseStamina(horizontalCost);
                    weaponFX.Stop();
                    weaponFX.Play();
                    attackLastPerformed = oh_Light_Attack_02;
                    return;
                }
            }
            
            else if (attackLastPerformed == th_Light_Attack_01)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    animator.SetBool("isCombo", true);
                    isCombo = true;
                    playerStamina.UseStamina(horizontalCost);
                    weaponFX.Stop();
                    weaponFX.Play();
                    attackLastPerformed = th_Light_Attack_02;
                }
            }
        }
    }

    private void HandleHeavyAttack()
    {
        if (isPerformingAction)
            return;
        
        if(playerStamina.stamina.Value < verticalCost)
            return;

        if (Input.GetMouseButtonUp(1))
        {
            verticalAttackInput = true;
            playerStamina.UseStamina(verticalCost);
            isAttack = true;
        }

        if (verticalAttackInput)
        {
            curPerformingAction = EnumList.States.VerticalAttack;
            verticalAttackInput = false;

            if (isTwoHandingWeapon)
            {
                if (isSprinting)
                {
                    PlayActionAnimation("TH_Jumping_Attack_01", true);
                    weaponFX.Stop();
                    weaponFX.Play();
                    return;
                }

                PlayActionAnimation("TH_Heavy_Attack_01", true);
                weaponFX.Stop();
                weaponFX.Play();
                attackLastPerformed = th_Heavy_Attack_01;
            }
            else
            {
                if (isSprinting)
                {
                    PlayActionAnimation("OH_Jumping_Attack_01", true);
                    weaponFX.Stop();
                    weaponFX.Play();
                    return;
                }

                PlayActionAnimation("OH_Heavy_Attack_01", true);
                weaponFX.Stop();
                weaponFX.Play();
                attackLastPerformed = oh_Heavy_Attack_01;
            }
        }
    }

    private void HandleHeavyAttackCombo()
    {
        if(playerStamina.stamina.Value < verticalCost)
            return;
        
        if (isPerformingAction)
        {
            if (attackLastPerformed == oh_Heavy_Attack_01)
            {
                if (Input.GetMouseButtonUp(1))
                {
                    animator.SetBool("isCombo", true);
                    isCombo = true;
                    playerStamina.UseStamina(verticalCost);
                    weaponFX.Stop();
                    weaponFX.Play();
                    attackLastPerformed = oh_Heavy_Attack_02;
                }
            }
            else if (attackLastPerformed == th_Heavy_Attack_01)
            {
                if (Input.GetMouseButtonUp(1))
                {
                    animator.SetBool("isCombo", true);
                    isCombo = true;
                    playerStamina.UseStamina(verticalCost);
                    weaponFX.Stop();
                    weaponFX.Play();
                    attackLastPerformed = th_Heavy_Attack_02;
                }
            }
            
        }
    }

    private void HandleChargeAttack()
    {
        if (Input.GetKeyDown(key_ChargeAttack))
        {
            chargeAttackInput = true;
        }

        if (isPerformingAction)
            return;
        
        if(playerStamina.stamina.Value < chargeCost)
            return;

        if (chargeAttackInput)
        {
            curPerformingAction = EnumList.States.ChargeAttack;
            playerStamina.UseStamina(chargeCost);
            isAttack = true;
            chargeAttackInput = false;

            if (isTwoHandingWeapon)
            {
                PlayActionAnimation("TH_Charge_Attack_01_Wind_Up", true);
                weaponChargeFX.Stop();
                weaponChargeFX.Play();
                attackLastPerformed = th_Charge_Attack_01;
            }
            else
            {
                PlayActionAnimation("OH_Charge_Attack_01_Wind_Up", true);
                weaponChargeFX.Stop();
                weaponChargeFX.Play();
                attackLastPerformed = oh_Charge_Attack_01;
            }
        }
    }

    private void HandleChargeAttackCombo()
    {
        if(playerStamina.stamina.Value < chargeCost)
            return;
        
        if (isPerformingAction)
        {
            if (Input.GetKeyDown(key_ChargeAttack))
            {
                chargeAttackInput = true;
            }
                
            if (chargeAttackInput)
            {
                chargeAttackInput = false;

                if (attackLastPerformed == oh_Charge_Attack_01)
                {
                    if (Input.GetKeyDown(key_ChargeAttack))
                    {
                        animator.SetBool("isCombo", true);
                        isCombo = true;
                        playerStamina.UseStamina(chargeCost);
                        weaponChargeFX.Stop();
                        weaponChargeFX.Play();
                        attackLastPerformed = oh_Charge_Attack_02;
                    }
                }
                
                else if (attackLastPerformed == th_Charge_Attack_01)
                {
                    if (Input.GetKeyDown(key_ChargeAttack))
                    {
                        animator.SetBool("isCombo", true);
                        isCombo = true;
                        playerStamina.UseStamina(chargeCost);
                        weaponChargeFX.Stop();
                        weaponChargeFX.Play();
                        attackLastPerformed = th_Charge_Attack_02;
                    }
                }
                
            }
        }
    }

    private void HandleDodge()
    {
        if (isPerformingAction)
            return;

        
        if (playerStamina.stamina.Value < dodgeStaminaCost)
            return;
        
        if (Input.GetKeyDown(key_Dodge))
        {
            curPerformingAction = EnumList.States.Dodge;
            if (moveAmount > 0)
            {
                rollDirection = moveDirection;
                rollDirection.y = 0;
                rollDirection.Normalize();

                Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
                this.transform.rotation = playerRotation;

                PlayActionAnimation("Roll_Forward_01", true);
                playerStamina.UseStamina(dodgeStaminaCost);
            }
            else
            {
                attackLastPerformed = null;
                playerStamina.UseStamina(backStaminaCost);
                animator.SetBool("isPerformingBackStep", true);

                if (isTwoHandingWeapon)
                {
                    PlayActionAnimation("TH_Backstep", true);
                }
                else
                {
                    PlayActionAnimation("OH_Backstep", true);
                }
            }
        }
        

    }

    private void HandleBackStepAttack()
    {
        if (isPerformingBackStep)
        {
            if (Input.GetMouseButtonDown(0))
            {
                curPerformingAction = EnumList.States.HorizontalAttack;
                isAttack = true;
                animator.SetBool("isPerformingBackStep", false);

                if (isTwoHandingWeapon)
                {
                    PlayActionAnimation("TH_Backstep_Attack", true);
                    weaponFX.Stop();
                    weaponFX.Play();
                }
                else
                {
                    PlayActionAnimation("OH_Backstep_Attack", true);
                    weaponFX.Stop();
                    weaponFX.Play();
                }
            }
        }
    }

    private void HandleLockOn()
    {
        if (Input.GetKeyDown(key_Targeting))
        {
            if (isLockOn == false)
            {
                GameObject targetObject = FindClosestEnemyObject();

                if (targetObject != null)
                {
                    targetTransform = targetObject.transform;
                    isLockOn = true;
                    cameraObject.LookAt = targetTransform;
                    
                    return;
                }
            }
            
            isLockOn = false;
            cameraObject.LookAt = null;
            targetTransform = null;
            
        }
        
    }
    
    GameObject FindClosestEnemyObject()
    {
        Collider[] hitColliders = Physics.OverlapSphere(playerTransform.position, sphereRadius, enemyLayer);
        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider hitCollider in hitColliders)
        {
            float distance = Vector3.Distance(playerTransform.position, hitCollider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = hitCollider.gameObject;
            }
        }

        return closestEnemy;
    }

    private void HandleSprint()
    {
        if (isPerformingAction)
            return;

        if (Input.GetKey(key_Sprint) && moveAmount > 0)
        {
            isSprinting = true;
            isWalking = false;
            isRunning = true;
        }
        else
        {
            isSprinting = false;
        }
    }

    private void HandleWalkOrRun()
    {
        if (isSprinting || isPerformingAction)
            return;

        if (Input.GetKeyUp(key_MoveType))
        {
            if (isWalking || !isRunning)
            {
                isWalking = false;
                isRunning = true;
            }
            else if (!isWalking || isRunning)
            {
                isWalking = true;
                isRunning = false;
            }
        }
    }

    private void HandlePlayerRotation()
    {
        if (isLockOn)
        {
            Vector3 rotationDirection = moveDirection;
            rotationDirection = cameraObject.transform.forward;
            rotationDirection.y = 0;
            rotationDirection.Normalize();
            Quaternion tr = Quaternion.LookRotation(rotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);
            transform.rotation = targetRotation;
        }
        else
        {
            Vector3 targetDirection = Vector3.zero;
            targetDirection = cameraObject.transform.forward * verticalMovement;
            targetDirection = targetDirection + cameraObject.transform.right * horizontalMovement;
            targetDirection.Normalize();
            targetDirection.y = 0;

            if (targetDirection == Vector3.zero)
            {
                targetDirection = transform.forward;
            }

            Quaternion turnRotation = Quaternion.LookRotation(targetDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, turnRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = targetRotation;
        }
    }

    private void HandlePlayerMovement()
    {
        moveDirection = cameraObject.transform.forward * verticalMovement;
        moveDirection = moveDirection + cameraObject.transform.right * horizontalMovement;
        moveDirection.Normalize();
        //moveDirection.y = 0;

        if (isSprinting)
        {
            moveDirection = moveDirection * sprintSpeed;
        }
        else if (isRunning)
        {
            moveDirection = moveDirection * runningSpeed;
        }
        else if (isWalking)
        {
            moveDirection = moveDirection * walkingSpeed;
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void HandleCameraActions()
    {
        HandleCameraFollowPlayer();
        HandleCameraRotate();
    }

    private void HandleCameraFollowPlayer()
    {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(playerCamera.transform.position, transform.position, ref cameraFollowVelocity, 0.1f);
        playerCamera.transform.position = targetCameraPosition;
    }

    private void HandleCameraRotate()
    {
        Vector3 cameraRotation;
        Quaternion targetCameraRotation;
        Quaternion lockOnCamRotation;

        leftandRightLookAngle += (mouseX * leftAndRightLookSpeed) * Time.deltaTime;
        upAndDownLookAngle -= (mouseY * upAndDownLookSpeed) * Time.deltaTime;
        upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

        cameraRotation = Vector3.zero;
        cameraRotation.y = leftandRightLookAngle;
        targetCameraRotation = Quaternion.Euler(cameraRotation);

        if (isLockOn)
        {
            playerCamera.transform.LookAt(cameraObject.LookAt);
        }
        else
        {
            playerCamera.transform.rotation = targetCameraRotation;
        }
        cameraRotation = Vector3.zero;
        cameraRotation.x = upAndDownLookAngle;
        targetCameraRotation = Quaternion.Euler(cameraRotation);

        lockOnCamRotation = Quaternion.Euler(30f, -15f, 0f);
        
        playerCameraPivot.transform.localRotation = 
            isLockOn ? lockOnCamRotation : targetCameraRotation;
    }

    public void FollowDownTheRabbitHole()
    {
        PlayActionAnimation("FallDown", true);
        canRecovering = false;
        StartCoroutine(WakeUp());
    }

    IEnumerator WakeUp()
    {
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("isSleep",false);
        playerStamina.UseStamina(90f);
    }
    
    private void CursorLock()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void CursorUnlock()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void PlayActionAnimation(string animation, bool isPerformingAction)
    {
        animator.SetBool("isPerformingAction", isPerformingAction);
        animator.CrossFade(animation, 0.2f);
    }
}
