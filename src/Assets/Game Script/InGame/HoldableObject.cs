using Photon.Pun;
using Photon.Realtime;
using Sausagecat.PlayerControlSystem;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PhotonView))]
public class HoldableObject : MonoBehaviourPun, IInteractable
{
    [Header("Identification")]
    [Tooltip("ID ที่ไม่ซ้ำกันสำหรับ Prefab นี้ (เช่น 'WoodLog', 'Stone')")]
    [SerializeField] private string itemID;
    public string ItemID => itemID;

    [Header("Offsets")]
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector3 rotationOffset;
    
    [Header("Invoke Event When Holding First time")]
    [SerializeField] private UnityEvent onFirstTimeHoldEvent;
    private bool hasBeenHeldBefore = false;

    private Rigidbody rb;
    private PhotonView pv;
    private Player ownerPlayer;

    private float chargeStartTime;
    private bool isCharging = false;
    private bool isBeingHeld = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        pv = GetComponent<PhotonView>();
    }
    private void Update()
    {
        // �Ѵ��� Input ��â��ҧ੾����Ңͧ���������
        if (PhotonNetwork.InRoom)
        {
            if (isBeingHeld && ownerPlayer != null && ownerPlayer.photonView.IsMine)
            {
                HandleThrowInput();
            }
        }
        else
        {
            if (isBeingHeld && ownerPlayer != null)
            {
                HandleThrowInput();
            }
        }
    }
    private void HandleThrowInput()
    {
        PlayerLocomotion pLocomotion = ownerPlayer.GetComponent<PlayerLocomotion>();

        // 1. �ѧ���������� (����)
        if (pLocomotion.SendMainActionSignal && !isCharging)
        {
            isCharging = true;
            chargeStartTime = Time.time;
        }

        // 2. �ѧ��л���»��� (�¹)
        if (!pLocomotion.SendMainActionSignal && isCharging)
        {
            float chargeDuration = Time.time - chargeStartTime;
            float chargePercent = Mathf.Clamp01(chargeDuration / ownerPlayer.maxChargeTime);
            float finalForce = Mathf.Lerp(ownerPlayer.minThrowForce, ownerPlayer.maxThrowForce, chargePercent);

            Throw(finalForce);
            isCharging = false;
        }
    }

    public void Throw(float force)
    {
        if (PhotonNetwork.InRoom)
        {
            // ���ç���ҧ��ҹ RPC
            pv.RPC(nameof(RPC_HandleThrow), RpcTarget.AllBuffered, force);
        }
        else
        {
            LocalThrow(force);
        }
    }
    [PunRPC]
    private void RPC_HandleThrow(float force)
    {
        LocalThrow(force);
    }

    private void LocalThrow(float force)
    {
        // �� Reference ��ȷҧ��͹�Ŵ Parent
        Vector3 throwDir = ownerPlayer.transform.TransformDirection(ownerPlayer.throwDirectionOffset).normalized;

        LocalDrop(); // �Ŵ Parent ��Ф׹��� Action

        // ����ç���ҧ
        rb.AddForce(throwDir * force, ForceMode.Impulse);
    }
    public void OnBeginIntereact(GameObject player, bool getBoolean = false)
    {
        if (isBeingHeld) return;

        // ����� Multiplayer ��ͧ������Ңͧ�ѵ�ء�͹��������Ѻ��������
        if (PhotonNetwork.InRoom)
        {
            pv.RequestOwnership();
            pv.RPC(nameof(RPC_HandlePickUp), RpcTarget.AllBuffered, player.GetComponent<PhotonView>().ViewID);
        }
        else
        {
            LocalPickUp(player);
        }
    }

    [PunRPC]
    private void RPC_HandlePickUp(int playerViewID)
    {
        PhotonView targetPlayerPV = PhotonView.Find(playerViewID);
        if (targetPlayerPV != null)
        {
            LocalPickUp(targetPlayerPV.gameObject);
        }
    }

    private void LocalPickUp(GameObject player)
    {
        ownerPlayer = player.GetComponent<Player>();
        if (ownerPlayer.photonView.IsMine && PhotonNetwork.InRoom)
        {
            if(!ownerPlayer.photonView.IsMine) return;
        }
        
        Player.SetActionLeftClick(ownerPlayer.photonView, () => {});
        ownerPlayer.SetHoldableObject(ownerPlayer.photonView, this);
        ownerPlayer.GetComponent<PlayerAnimation>().SetArmLayerWeight(1f);
        
        isBeingHeld = true;
        rb.isKinematic = true;
        rb.useGravity = false;

        Transform holdSlot = player.GetComponent<PlayerEquipment>().HoldSlot;
        transform.SetParent(holdSlot);
        transform.localPosition = positionOffset;
        transform.localRotation = Quaternion.Euler(rotationOffset);
        
        if (!hasBeenHeldBefore)
        {
            if(PhotonNetwork.InRoom)
            {
                photonView.RPC(nameof(RPC_InvokeEventOnFirstHold), RpcTarget.AllBuffered);
            }
            else
            {
                LocalInvokeEventOnFirstHold();
            }
        }

    }
    
    [PunRPC]
    private void RPC_InvokeEventOnFirstHold()
    {
        LocalInvokeEventOnFirstHold();
    }
    private void LocalInvokeEventOnFirstHold()
    {
        onFirstTimeHoldEvent?.Invoke();
        hasBeenHeldBefore = true;
    }

    // �ѧ��ѹ����Ѻ�ҧ�ͧ (Drop)
    public void Drop()
    {
        if (PhotonNetwork.InRoom)
        {
            pv.RPC(nameof(RPC_HandleDrop), RpcTarget.AllBuffered);
        }
        else
        {
            LocalDrop();
        }
    }

    [PunRPC]
    private void RPC_HandleDrop()
    {
        LocalDrop();
    }

    private void LocalDrop()
    {
        isBeingHeld = false;

        // �׹��� Action ��͹�лŴ Parent ������ҧ��� ownerPlayer
        if (ownerPlayer != null)
        {
            if (ownerPlayer.photonView.IsMine && PhotonNetwork.InRoom)
            {
                PlayerCombat combat = ownerPlayer.GetComponent<PlayerCombat>();
                Player.SetActionLeftClick(ownerPlayer.photonView, combat.OnInvokeAttack);

                ownerPlayer.GetComponent<PlayerAnimation>().SetArmLayerWeight(0f);
            }
            else
            {
                PlayerCombat combat = ownerPlayer.GetComponent<PlayerCombat>();
                Player.SetActionLeftClick(ownerPlayer.photonView, combat.OnInvokeAttack);

                ownerPlayer.GetComponent<PlayerAnimation>().SetArmLayerWeight(0f);
            }
        }

        transform.SetParent(null);
        rb.isKinematic = false;
        rb.useGravity = true;

        ownerPlayer = null; // ��ҧ�����ѧ�ҡ�ҧ����
    }

    public void OnCancelInteract() { }
    public void OnHoldInteract(GameObject player, float progress) { }
    // --- IInteractable Implementation ---
    public void ShowWorldInterectUI() { /* ������ E */ }
    public void HideWorldInterectUI() { /* �Դ���� E */ }
}