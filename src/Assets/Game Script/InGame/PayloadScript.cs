using Photon.Pun;
using Photon.Realtime;
using Sausagecat.PlayerControlSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

public class PayloadScript : MonoBehaviourPun,IInteractable
{
    [Header("Payload Setting")]
    public float SphereAreaRadius = 3f;
    public float MoveSpeedTarget = 5f;
    public float AccelerationTime = 2f;
    public float TurnSpeed = 60f;
    public Transform RearHitch;

    [Header("Interact Sitting and Control")]
    [SerializeField] private Transform sitPosition;

    [Header("Physics Settings")]
    [SerializeField] private Rigidbody payloadRb;

    private float distancePercentage = 0f;
    private float verticalInput = 0f;
    private float horizontalInput = 0f;
    private float currentMoveSpeed = 0f;

    private bool isTurnOn = false;

    private Collider[] collisionPlayer;
    private PayloadSpawner pSpawner;

    public UnityAction OnPayloadMoveAction;

    public Player CurrentPlayerControl { get; set; }
    private PlayerLocomotion reciveLocomotion;
    public void PayloadOnSetup()
    {
        pSpawner = GetComponent<PayloadSpawner>();
        pSpawner.spawnPointParent = GameObject.FindGameObjectWithTag("SpawnPoint").transform;

        OnPayloadMoveAction += PayloadMoveController;
    }
    private void Update()
    {
        // ในระบบ Multiplayer เรามักจะให้ Master Client เป็นคนตัดสินใจเรื่อง "เงื่อนไขการวิ่ง"
        HandlePayloadLogic();

        // ทุกเครื่อง (รวมถึง Client) จะต้องรันการขยับตำแหน่งตามความเร็วปัจจุบันเพื่อให้ภาพนุ่มนวล
        //if (currentMoveSpeed > 0)
        //{
        //    PayloadPositionHandler();
        //}
    }
    private void FixedUpdate()
    {
        // *** สำคัญ: ย้ายการเรียก UnityAction มาที่ FixedUpdate เพราะเราควบคุมด้วย Physics แล้ว ***
        //if (currentMoveSpeed > 0f)
        //{
        //    OnPayloadMoveAction?.Invoke();
        //}
    }
    private void HandlePayloadLogic()
    {
        bool hasPlayer = CheckPlayerNearby();

        float target = (hasPlayer && isTurnOn) ? MoveSpeedTarget : 0f;
        float newSpeed = Mathf.MoveTowards(currentMoveSpeed, target, Time.deltaTime * AccelerationTime);

        if (!Mathf.Approximately(currentMoveSpeed, newSpeed))
        {
            if (PhotonNetwork.InRoom)
            {
                // ส่งค่าความเร็วไปให้ทุกคนซิงค์ตาม
                photonView.RPC(nameof(RPC_SyncPayloadSpeed), RpcTarget.All, newSpeed);
            }
            else
            {
                currentMoveSpeed = newSpeed;
            }
        }
    }
    [PunRPC]
    private void RPC_SyncPayloadSpeed(float speed)
    {
        // ทุกเครื่องจะได้รับความเร็วเท่ากัน และนำไปรันใน PayloadPositionHandler() ต่อไป
        currentMoveSpeed = speed;
    }
    public void SetPayloadSpeed(float speed)
    {
        currentMoveSpeed = speed;
    }

    private bool CheckPlayerNearby()
    {
        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, SphereAreaRadius, LayerMask.GetMask("Player"));
        return hitPlayers.Length > 0;
    }
    private void PayloadPositionHandler()
    {
        //// 1. คำนวณระยะทางสะสม (0-1)
        //distancePercentage += (currentMoveSpeed * Time.deltaTime) / splineLenght;
        //distancePercentage = Mathf.Clamp01(distancePercentage); // กันค่าเกิน 1

        //Vector3 currentPosition = splineAnimate.Container.EvaluatePosition(distancePercentage);
        //transform.position = currentPosition;

        //Vector3 forwardDirection = splineAnimate.Container.EvaluateTangent(distancePercentage);

        //Vector3 upDirection = splineAnimate.Container.EvaluateUpVector(distancePercentage);

        //if (forwardDirection != Vector3.zero)
        //{
        //    transform.rotation = Quaternion.LookRotation(forwardDirection, upDirection);
        //}
        //if (distancePercentage >= 1.0f)
        //{
        //    RoomManager.Instance.TriggerWinCondition();
        //}
    }
    public void PlayloadSwitchFunction()
    {
        isTurnOn = !isTurnOn;
        Debug.Log("Payload Engine: " + (isTurnOn ? "ON" : "OFF"));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isTurnOn ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, SphereAreaRadius);
    }

    public void ShowWorldInterectUI()
    {
        
    }

    public void HideWorldInterectUI()
    {
        
    }

    public void OnBeginIntereact(GameObject player, bool getBoolean = false)
    {
        if (PhotonNetwork.InRoom)
        {
            int getViewID = player.GetComponent<PhotonView>().ViewID;
            photonView.RPC(nameof(RPC_SitOnPayload), RpcTarget.AllBuffered, getViewID);
        }
        else
        {
            SitOnPayload(player);
        }
    }

    [PunRPC]
    public void RPC_SitOnPayload(int playerID)
    {
        PhotonView targetPv = PhotonView.Find(playerID);

        if (targetPv != null)
        {
            GameObject playerObj = targetPv.gameObject;
            // ทำ Logic การสวมใส่ต่อด้วย playerObj
            PlayerEquipment playerEquipment = playerObj.GetComponent<PlayerEquipment>();
            SitOnPayload(playerObj);
        }
    }

    private void SitOnPayload(GameObject player)
    {
        if (sitPosition == null) return;
        
        Player p = player.GetComponent<Player>();

        if (p == null) return;

        CurrentPlayerControl = p;
        reciveLocomotion = p.GetComponent<PlayerLocomotion>();

        p.OnMountingPayload();

        p.transform.SetParent(sitPosition);
        p.transform.localPosition = Vector3.zero;
        p.transform.localRotation = Quaternion.identity;
        p.transform.localScale = Vector3.one;
    }
    private void InvokeJumpOffPayload()
    {
        if (CurrentPlayerControl == null) return;

        if (PhotonNetwork.InRoom)
        {
            PhotonView playerPv = CurrentPlayerControl.GetComponent<PhotonView>();
            if (playerPv.IsMine)
            {
                int getViewID = playerPv.ViewID;
                // ส่ง RPC บอกทุกเครื่องให้ปลดผู้เล่นคนนี้ออก
                photonView.RPC(nameof(RPC_JumpOffPayload), RpcTarget.AllBuffered, getViewID);
            }
        }
        else
        {
            JumpOffPayload(CurrentPlayerControl.gameObject);
        }
    }
    [PunRPC]
    public void RPC_JumpOffPayload(int playerID)
    {
        PhotonView targetPv = PhotonView.Find(playerID);
        if (targetPv != null)
        {
            JumpOffPayload(targetPv.gameObject);
        }
    }

    private void JumpOffPayload(GameObject playerObj)
    {
        Player p = playerObj.GetComponent<Player>();
        if (p != null)
        {
            // 1. ปลดผู้เล่นออกจาก Parent (SitPosition) กลับสู่โลกปกติ
            p.transform.SetParent(null);

            // 2. คืนค่าสถานะให้ผู้เล่น (เช่น เปิด CharacterController, กลับมาใช้ Gravity ปกติ)
            // *คุณต้องสร้างฟังก์ชัน OnDismountingPayload() ใน Script Player ด้วยนะครับ
            p.OnDismountingPayload();

            // 3. Reset สถานะการกระโดดของ PlayerLocomotion เพื่อป้องกันการกระโดดเบิ้ลเมื่อลงพื้น
            PlayerLocomotion pLocomotion = p.GetComponent<PlayerLocomotion>();
            if (pLocomotion != null)
            {
                pLocomotion.OnJumping = false;
            }
        }

        // 4. เคลียร์ค่าคนขับและ Input ของรถออก เพื่อให้รถหยุดสนิทและพร้อมรับคนใหม่
        CurrentPlayerControl = null;
        reciveLocomotion = null;
        verticalInput = 0f;
        horizontalInput = 0f;
    }
    public void PayloadMoveController()
    {
        if (payloadRb == null) return;

        verticalInput = reciveLocomotion.MovementInput.y;
        horizontalInput = reciveLocomotion.MovementInput.x;

        // --- แก้ไขส่วนการเคลื่อนที่ ---

        // 1. คำนวณทิศทางที่ต้องการจะไป (Local Forward)
        Vector3 targetDirection = transform.forward * verticalInput;

        // 2. คำนวณความเร็วเป้าหมาย (Speed N)
        // เราจะเอาความเร็ว Y เดิมไว้ (เผื่อตกหลุม/แรงโน้มถ่วง) แล้วเปลี่ยนแค่ X, Z
        Vector3 targetVelocity = targetDirection * MoveSpeedTarget;

        // 3. ยัดความเร็วใส่ Rigidbody โดยตรง (Override Physics Forces)
        // รักษาค่า Y เดิมไว้ เพื่อให้แรงโน้มถ่วงทำงานปกติ
        payloadRb.velocity = new Vector3(targetVelocity.x, payloadRb.velocity.y, targetVelocity.z);


        // --- ส่วนการหมุน (MoveRotation ใช้ได้เหมือนเดิม เพราะไม่ค่อยโดนดึง) ---
        if (verticalInput != 0f && horizontalInput != 0f)
        {
            float turnMultiplier = (verticalInput < 0) ? -1f : 1f;
            float turnAngle = horizontalInput * turnMultiplier * TurnSpeed * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turnAngle, 0f);
            payloadRb.MoveRotation(payloadRb.rotation * turnRotation);
        }

        if (reciveLocomotion.OnJumping)
        {
            InvokeJumpOffPayload();
        }
    }

    public void OnHoldInteract(GameObject player, float progress)
    {
        throw new System.NotImplementedException();
    }

    public void OnCancelInteract()
    {
        throw new System.NotImplementedException();
    }
}
