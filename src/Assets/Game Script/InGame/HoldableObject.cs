using Photon.Pun;
using Photon.Realtime;
using Sausagecat.PlayerControlSystem;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PhotonView))]
public class HoldableObject : MonoBehaviour, IInteractable
{
    [Header("Offsets")]
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector3 rotationOffset;

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
        // จัดการ Input การขว้างเฉพาะเจ้าของที่ถืออยู่
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

        // 1. จังหวะเริ่มกด (ชาร์จ)
        if (pLocomotion.SendMainActionSignal && !isCharging)
        {
            isCharging = true;
            chargeStartTime = Time.time;
        }

        // 2. จังหวะปล่อยปุ่ม (โยน)
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
            // ส่งแรงขว้างผ่าน RPC
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
        // เก็บ Reference ทิศทางก่อนปลด Parent
        Vector3 throwDir = ownerPlayer.transform.TransformDirection(ownerPlayer.throwDirectionOffset).normalized;

        LocalDrop(); // ปลด Parent และคืนค่า Action

        // ใส่แรงขว้าง
        rb.AddForce(throwDir * force, ForceMode.Impulse);
    }
    public void OnBeginIntereact(GameObject player, bool getBoolean = false)
    {
        if (isBeingHeld) return;

        // ถ้าเป็น Multiplayer ต้องขอเป็นเจ้าของวัตถุก่อนเพื่อให้ขยับได้ลื่นไหล
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

        // สำคัญ: ส่ง Action ว่างไปเพื่อ Override การโจมตีปกติ
        // ไม่ต้องใส่ Throw(10f) ตรงนี้ เพราะเราจะเช็คแรงจาก Update
        if (ownerPlayer.photonView.IsMine && PhotonNetwork.InRoom)
        {
            Player.SetActionLeftClick(() => {});

            ownerPlayer.GetComponent<PlayerAnimation>().SetArmLayerWeight(1f);
        }
        else
        {
            Player.SetActionLeftClick(() => {});

            ownerPlayer.GetComponent<PlayerAnimation>().SetArmLayerWeight(1f);
        }
            isBeingHeld = true;
        rb.isKinematic = true;
        rb.useGravity = false;

        Transform holdSlot = player.GetComponent<PlayerEquipment>().HoldSlot;
        transform.SetParent(holdSlot);
        transform.localPosition = positionOffset;
        transform.localRotation = Quaternion.Euler(rotationOffset);

    }

    // ฟังก์ชันสำหรับวางของ (Drop)
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

        // คืนค่า Action ก่อนจะปลด Parent หรือล้างค่า ownerPlayer
        if (ownerPlayer != null)
        {
            if (ownerPlayer.photonView.IsMine && PhotonNetwork.InRoom)
            {
                PlayerCombat combat = ownerPlayer.GetComponent<PlayerCombat>();
                Player.SetActionLeftClick(combat.OnInvokeAttack);

                ownerPlayer.GetComponent<PlayerAnimation>().SetArmLayerWeight(0f);
            }
            else
            {
                PlayerCombat combat = ownerPlayer.GetComponent<PlayerCombat>();
                Player.SetActionLeftClick(combat.OnInvokeAttack);

                ownerPlayer.GetComponent<PlayerAnimation>().SetArmLayerWeight(0f);
            }
        }

        transform.SetParent(null);
        rb.isKinematic = false;
        rb.useGravity = true;

        ownerPlayer = null; // ล้างค่าหลังจากวางแล้ว
    }

    public void OnCancelInteract() { }
    public void OnHoldInteract(GameObject player, float progress) { }
    // --- IInteractable Implementation ---
    public void ShowWorldInterectUI() { /* โชว์ปุ่ม E */ }
    public void HideWorldInterectUI() { /* ปิดปุ่ม E */ }
}