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
        bool hasPlayer = CheckPlayerNearby();

        float target = (hasPlayer && isTurnOn) ? MoveSpeedTarget : 0f;
        float newSpeed = Mathf.MoveTowards(currentMoveSpeed, target, Time.deltaTime * AccelerationTime);

        if (!Mathf.Approximately(currentMoveSpeed, newSpeed))
        {
            if (PhotonNetwork.InRoom)
            {
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
            //if(!targetPv.IsMine) return;

            Debug.Log($"RPC_SitOnPayload");
            GameObject playerObj = targetPv.gameObject;
            SitOnPayload(playerObj);
        }
    }

    private void SitOnPayload(GameObject player)
    {
        if (sitPosition == null) return;
        Player p = player.GetComponent<Player>();

        if (p == null) return;
        //if (p.photonView.IsMine == false) return;
        Debug.Log($"player Sit On Payload = {p.name}");
        
        CurrentPlayerControl = p;
        reciveLocomotion = p.GetComponent<PlayerLocomotion>();

        
        p.OnMountingPayload();
        Debug.Log($"sitPosition {sitPosition}");
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
           
            p.transform.SetParent(null);

            p.OnDismountingPayload();


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
