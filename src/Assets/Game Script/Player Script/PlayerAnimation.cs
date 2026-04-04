using Photon.Pun;
using Sausagecat.PlayerControlSystem;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerAnimation : MonoBehaviourPun
{
    [SerializeField] private Animator animator;
    [SerializeField] private float simepleBlendSpeed = 20f;
    [SerializeField] private float locomotionBlendSpeed = 0.25f;
    [SerializeField] private VelocityType velocityType = VelocityType.CharacterController;

    private PlayerLocomotion playerLocomotion;

    [Header("IK Rig Settings")]
    [SerializeField] private Rig handRig; // ลาก Rig ของธนูมาใส่ที่นี่
    [SerializeField] private Rig bodyRig; // ความเร็วในการเปลี่ยนน้ำหนัก IK
    [SerializeField] private Transform _aimTarget;

    private PlayerMovement playerMovement;
    private PlayerState playerState;
    private PlayerRagdollMovement playerRagdollMovement;
    private PlayerCombat playerCombat;

    [Header("Arm Layer Settings")]
    [SerializeField] private float weightLerpSpeed = 10f;
    private int armLayerIndex;
    private int armBowIndex;
    private float currentArmBowWeight = 0f;
    private float targetArmBowWeight = 0f;
    private float targetArmWeight = 0f;
    private float currentArmWeight = 0f;

    private static int magnitudeHash = Animator.StringToHash("Magnitude");
    private static int inputXHash = Animator.StringToHash("inputX");
    private static int inputYHash = Animator.StringToHash("inputY");
    private static int isGroundedHash = Animator.StringToHash("isGrounded");
    private static int isJumpHash = Animator.StringToHash("isJump");
    private static int isFalling = Animator.StringToHash("isFalling");
    private static int verticalMagnitudeHash = Animator.StringToHash("VerticalMagnitude");
    private static int isUsingOneHandedWeapon = Animator.StringToHash("isUsingOneHanded");
    private static int isDrawOneHandedHash = Animator.StringToHash("isDrawOneHanded");
    private static int isDrawBowHash = Animator.StringToHash("isDrawBow");
    private static int isSheathedOneHandedHash = Animator.StringToHash("isSheathedOneHanded");
    private static int animationFloatStateHash = Animator.StringToHash("animationFloatState");
    private static int lightAttackTriggerHash = Animator.StringToHash("lightAttackTrigger");
    
    private static int isChargingHash = Animator.StringToHash("IsCharging");
    private static int isSheathedDrawBowHash = Animator.StringToHash("isSheathedDrawBow");
    private static int aimAngleHash = Animator.StringToHash("AimAngle");
    private float syncAimAngle;

    Vector3 locomotionMagnitude = Vector3.zero;
    Vector3 currentBlendInput = Vector3.zero;

    private float currentAnimationfloat = 0f;
    private float targethandRigWeight = 0f;

    public float TargetAnimationfloat { get; set; }

    public enum VelocityType
    {
        CharacterController,
        Rigidbody
    }

    private void Awake()
    {
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerMovement = GetComponent<PlayerMovement>();
        playerState = GetComponent<PlayerState>();
        playerRagdollMovement = GetComponent<PlayerRagdollMovement>();
        playerCombat = GetComponent<PlayerCombat>();


        armLayerIndex = animator.GetLayerIndex("Arm Holding");
        armBowIndex = animator.GetLayerIndex("Arm Sheathed - Bow Drawed");
        
        RigBuilder rigBuilder = GetComponent<RigBuilder>();
        rigBuilder.Build();
    }
    private void Update()
    {
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        
        if (armLayerIndex != -1) // ��Ǩ�ͺ����� Layer ��������ԧ
        {
            currentArmWeight = Mathf.Lerp(currentArmWeight, targetArmWeight, weightLerpSpeed * Time.deltaTime);
            animator.SetLayerWeight(armLayerIndex, currentArmWeight);
        }
        if (armBowIndex != -1) // Ǩͺ Layer ԧ
        {
            currentArmBowWeight = Mathf.Lerp(currentArmBowWeight, targetArmBowWeight, weightLerpSpeed * Time.deltaTime);
            animator.SetLayerWeight(armBowIndex, currentArmBowWeight);
        }
        
        if (PhotonNetwork.InRoom)
        {
            if(!photonView.IsMine) return;
        }
        Vector3 velocity = VelocityByComponent();
        velocity.y = 0;
        locomotionMagnitude = Vector3.Lerp(locomotionMagnitude, velocity, simepleBlendSpeed * Time.deltaTime);
        float speedRatio = locomotionMagnitude.magnitude / GetMaxSpeed();
        animator.SetFloat(magnitudeHash, Mathf.Clamp01(speedRatio));

        Vector3 targetInput = playerLocomotion.MovementInput;
        currentBlendInput = Vector3.Lerp(currentBlendInput, targetInput, locomotionBlendSpeed * Time.deltaTime);
        animator.SetFloat(inputXHash,currentBlendInput.x);
        animator.SetFloat(inputYHash, currentBlendInput.y);

        animator.SetBool(isJumpHash, playerState.CurrentPlayerMovementState == PlayerState.PlayerMovementState.Jumping);
        animator.SetBool(isFalling, playerState.CurrentPlayerMovementState == PlayerState.PlayerMovementState.Falling);
        animator.SetBool(isGroundedHash, IsGroundByComponentController());

        animator.SetBool(isChargingHash,playerCombat.IsCharging);
        float verticalVelocity = VelocityByComponent().y;
        float verticalRatio = verticalVelocity / GetMaxSpeed();
        float finalVerticalValue = Mathf.Clamp(verticalRatio, 0f, 1f);
        animator.SetFloat(verticalMagnitudeHash, finalVerticalValue);

        currentAnimationfloat = Mathf.Lerp(currentAnimationfloat, TargetAnimationfloat, 10f * Time.deltaTime);
        animator.SetFloat(animationFloatStateHash, currentAnimationfloat);
        if (!photonView.IsMine && bodyRig != null && bodyRig.weight <= 0.01f)
        {
            // ถ้า Rig ไม่ทำงาน ให้ย้าย Aim Target กลับมาที่หน้าตัวละครตรงๆ (Default Position)
            // เพื่อป้องกันกระดูกดีดหรือบิดค้าง
            _aimTarget.localPosition = new Vector3(0, 1.5f, 2f); 
        }
        /*if (!photonView.IsMine && bodyRig != null && bodyRig.weight > 0.1f)
        {
            UpdateRemoteRigTarget(syncAimAngle);
        }*/
    }
    private void UpdateRemoteRigTarget(float angle)
    {
        // อ้างอิงจาก PlayerMovement เพื่อให้ได้จุด Pivot เดียวกัน
        Vector3 pivotPosition = transform.position + Vector3.up * 1.5f; // ใช้ค่าเดียวกับ _aimHeightOffset
        
        // คำนวณทิศทางจากมุม Angle
        Quaternion rotation = Quaternion.Euler(angle, transform.eulerAngles.y, 0f);
        Vector3 aimDirection = rotation * Vector3.forward;

        Vector3 targetPosition = pivotPosition + (aimDirection * 10f); // ใช้ค่าเดียวกับ _aimDistance
        
        // อัปเดตตำแหน่ง AimTarget ในเครื่อง Client อื่นๆ
        // (หมายเหตุ: _aimTarget ต้องเป็นตัวแปรที่ PlayerAnimation เข้าถึงได้)
        // หรือให้ PlayerMovement เป็นคนคุม UpdateRigTargetFixed สำหรับ Remote
    }
    public void SetAimAngle(float angle)
    {
        animator.SetFloat(aimAngleHash, angle);
    }
    public void SetBodyRigWeight(float weight)
    {
        if (bodyRig != null)
        {
            bodyRig.weight = weight;
            
            if(bodyRig.weight >= 1f)
            {
                playerMovement.LockRotating = true;
            }
            else
            {
                playerMovement.LockRotating = false;
            }
        }
    }
    public void SetArmLayerWeight(float weight)
    {
        targetArmWeight = Mathf.Clamp01(weight);
        
        if(weight == 0f)
        {
            targethandRigWeight  = 0f;
        }
        else if (weight == 1f)
        {
            targethandRigWeight  = 1f;
        }
    }
    public void SetArmBowLayerWeight(float weight)
    {
        targetArmBowWeight = Mathf.Clamp01(weight);
        if(weight == 0f)
        {
            targethandRigWeight  = 0f;
        }
        else if (weight == 1f)
        {
            targethandRigWeight  = 1f;
        }
    }
    private Vector3 VelocityByComponent()
    {
        if(velocityType == VelocityType.CharacterController)
        {
            return playerMovement.CurrentVelocity;
        }
        else
        {
            return playerRagdollMovement.RBvelocity;
        }
    }

    private float GetMaxSpeed()
    {
        if (velocityType == VelocityType.CharacterController)
        {
            return playerMovement.maxSpeed;
        }
        else
        {
            return playerRagdollMovement.MaxSpeed;
        }
    }

    private bool IsGroundByComponentController()
    {
        if (velocityType == VelocityType.CharacterController)
        {
            return playerMovement.IsGround;
        }
        else
        {
            return playerRagdollMovement.IsGrounded;
        }
    }

    public void SetOnUsingWeaponAnimation(bool isUsingBoolean,UtilityDev.WeaponType weaponType)
    {
        if(weaponType == UtilityDev.WeaponType.OneHandedMelee)
            animator.SetBool(isUsingOneHandedWeapon, isUsingBoolean);
        else if(weaponType == UtilityDev.WeaponType.SlingshotOrBow)
        {
            
        }
    }

    public void OnTriggerDrawOrSheathed(UtilityDev.DrawOrSheath drawOrSheath,UtilityDev.WeaponType weaponType)
    {
        // 1. อัปเดตเครื่องตัวเองทันที (Local)
        UpdateWeaponVisualState(drawOrSheath, weaponType);

        // 2. ส่ง RPC ไปบอกเครื่องคนอื่น (Multiplayer)
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC(nameof(RPC_SyncWeaponWeight), RpcTarget.Others, drawOrSheath, weaponType);
        }
    }
    // เพิ่มฟังก์ชันนี้เพื่อรับคำสั่งจากเครื่องเจ้าของ
    [PunRPC]
    public void RPC_SyncWeaponWeight(UtilityDev.DrawOrSheath state, UtilityDev.WeaponType type)
    {
        // ถ้าเป็นเครื่องตัวเอง ไม่ต้องรันซ้ำ (เพราะรันไปแล้วก่อนส่ง RPC)
        if (photonView.IsMine) return; 

        // รัน Logic เดียวกับเครื่องเจ้าของเพื่อให้เครื่อง Client อื่นเปลี่ยน Weight ตาม
        UpdateWeaponVisualState(state, type);
    }

    // แยก Logic ออกมาเป็นฟังก์ชันกลางเพื่อให้เรียกใช้ได้ทั้ง Local และ RPC
    private void UpdateWeaponVisualState(UtilityDev.DrawOrSheath drawOrSheath, UtilityDev.WeaponType weaponType)
    {
        switch (weaponType)
        {
            case UtilityDev.WeaponType.OneHandedMelee:
                if(drawOrSheath == UtilityDev.DrawOrSheath.Draw)
                {
                    SetArmBowLayerWeight(0f);
                    SetBodyRigWeight(0f);
                    animator.SetTrigger(isDrawOneHandedHash);
                    animator.ResetTrigger(isSheathedOneHandedHash);
                    Debug.Log("Draw Animation Triggered");
                }
                else
                {
                    animator.SetTrigger(isSheathedOneHandedHash);
                    animator.ResetTrigger(isDrawOneHandedHash);
                    Debug.Log("Sheathed Animation Triggered");
                }
                break;
            case UtilityDev.WeaponType.SlingshotOrBow:
                if (drawOrSheath == UtilityDev.DrawOrSheath.Draw)
                {
                    SetArmBowLayerWeight(1f);
                    SetBodyRigWeight(1f);
                    animator.SetTrigger(isDrawBowHash);
                    animator.ResetTrigger(isSheathedDrawBowHash);
                } 
                else
                {
                    SetArmBowLayerWeight(0f);
                    SetBodyRigWeight(0f);
                    animator.SetTrigger(isSheathedDrawBowHash);
                    animator.ResetTrigger(isDrawBowHash);
                }
                break;
        }
    }
    public void PerformAttackAnimation(ComboNode getComboNode)
    {
        // ��������� Animator �ͧ�س ��ҵ������Ի���� "LightAttack_Base"
        AnimatorOverrideController aoc = getComboNode.AnimationOverrideCtrl;
        animator.runtimeAnimatorController = aoc;
        animator.SetTrigger(lightAttackTriggerHash);
    }

    public float GetNormalizedTime(string nameTag, int layerIndex = 2)
    {

        if (animator.IsInTransition(layerIndex))
        {
            AnimatorStateInfo nextState = animator.GetNextAnimatorStateInfo(layerIndex);
            if (nextState.IsTag(nameTag)) return 0f;
            return -1f;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);

        if (stateInfo.IsTag(nameTag))
        {
            // ����͹����ѹ�����Ǣ�� �� normalizedTime ���ѧ����觨ҡ 0 � 1 ����
            return Mathf.Clamp01(stateInfo.normalizedTime);
        }

        return -1f;
    }

    public void SetAttackSpeed(float speed)
    {
        // �觤�Ҥ������Ƿ���ͧ�������� Parameter
        animator.SetFloat("AnimationAttackSpeed", speed);
    }
}
