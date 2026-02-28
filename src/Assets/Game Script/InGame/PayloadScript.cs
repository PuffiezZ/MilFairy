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
        HandlePayloadLogic();
    }

    private void HandlePayloadLogic()
    {
        // เฉพาะเจ้าของเท่านั้นที่เป็นคนคำนวณความเร็ว เพื่อป้องกันการส่ง RPC ซ้อนกันจากหลายเครื่อง
        if (PhotonNetwork.InRoom && !photonView.IsMine) return;

        bool hasPlayer = CheckPlayerNearby();

        float target = (hasPlayer && isTurnOn) ? MoveSpeedTarget : 0f;
        float newSpeed = Mathf.MoveTowards(currentMoveSpeed, target, Time.deltaTime * AccelerationTime);

        if (!Mathf.Approximately(currentMoveSpeed, newSpeed))
        {
            if (PhotonNetwork.InRoom)
            {
                // �觤�Ҥ�����������ء���ԧ����
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
        // �ء����ͧ�����Ѻ����������ҡѹ ��й���ѹ� PayloadPositionHandler() ����
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
        //// 1. �ӹǳ���зҧ���� (0-1)
        //distancePercentage += (currentMoveSpeed * Time.deltaTime) / splineLenght;
        //distancePercentage = Mathf.Clamp01(distancePercentage); // �ѹ����Թ 1

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
            // สำคัญ: ขอสิทธิ์เป็นเจ้าของรถเข็นก่อน เพื่อให้ส่ง RPC และควบคุมความเร็วได้
            photonView.RequestOwnership();

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
            // �� Logic ����������ʹ��� playerObj
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

        // ปิด CharacterController และฟิสิกส์ของตัวละครก่อนทำ Parenting เพื่อไม่ให้ตำแหน่งดีด
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // ปิดการ Sync ตำแหน่งชั่วคราว (ถ้ามี PhotonTransformView) เพื่อให้มันเคลื่อนที่ไปพร้อมกับ Parent ได้แม่นยำ
        PhotonTransformView ptv = player.GetComponent<PhotonTransformView>();
        if (ptv != null) ptv.enabled = false;

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
                // �� RPC �͡�ء����ͧ���Ŵ�����蹤�����͡
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
            // 1. �Ŵ�������͡�ҡ Parent (SitPosition) ��Ѻ����š����
            p.transform.SetParent(null);

            // 2. �׹���ʶҹ��������� (�� �Դ CharacterController, ��Ѻ���� Gravity ����)
            // *�س��ͧ���ҧ�ѧ��ѹ OnDismountingPayload() � Script Player ���¹Ф�Ѻ
            p.OnDismountingPayload();

            // 3. Reset ʶҹС�á��ⴴ�ͧ PlayerLocomotion ���ͻ�ͧ�ѹ��á��ⴴ���������ŧ���
            PlayerLocomotion pLocomotion = p.GetComponent<PlayerLocomotion>();
            if (pLocomotion != null)
            {
                pLocomotion.OnJumping = false;
            }
        }

        // 4. �������Ҥ��Ѻ��� Input �ͧö�͡ �������ö��شʹԷ��о�����Ѻ������
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


        Vector3 targetDirection = transform.forward * verticalInput;


        Vector3 targetVelocity = targetDirection * MoveSpeedTarget;

        payloadRb.velocity = new Vector3(targetVelocity.x, payloadRb.velocity.y, targetVelocity.z);


        // --- ��ǹ�����ع (MoveRotation ��������͹��� ����������ⴹ�֧) ---
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
