using NaughtyAttributes;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Splines;

public class PayloadSetup : MonoBehaviourPun
{
    [SerializeField] private bool enablePayload = false;

    [BoxGroup("Payload (Leader)")]
    [ShowIf(nameof(enablePayload))]
    [SerializeField] private GameObject payloadPrefab; // Prefab หัวลาก
    [BoxGroup("Payload (Leader)")]
    [ShowIf(nameof(enablePayload))]
    [SerializeField] private Transform payloadInstancePOS;

    [BoxGroup("Cart (Trailer)")]
    [ShowIf(nameof(enablePayload))]
    [SerializeField] private bool enableCart = true; // เปิด/ปิดรถพ่วง
    [BoxGroup("Cart (Trailer)")]
    [ShowIf(nameof(enableCart))]
    [SerializeField] private GameObject cartPrefab;    // Prefab รถพ่วง (ToothCart)
    [BoxGroup("Cart (Trailer)")]
    [ShowIf(nameof(enableCart))]
    [SerializeField] private Transform cartInstancePOS; // จุดเกิดรถพ่วง (ถ้าไม่ใส่จะเกิดต่อท้ายเอง)

    [ShowIf(nameof(enablePayload))]
    [SerializeField] private GameObject objectiveCart; // (อันเดิมของคุณ)
    public void OnInstancePayload()
    {
        // ในระบบ Multiplayer เราจะให้ Master Client เป็นคนสั่งสร้างเพียงคนเดียว
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                InstancePayload();
            }
        }
        else
        {
            // กรณีเล่นคนเดียว (Offline)
            InstancePayload();
        }
    }
    private void InstancePayload()
    {
        if (enablePayload == false) return;

        // --- 1. Spawn Payload (หัวลาก) ---
        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;

        if (payloadInstancePOS != null)
        {
            spawnPos = payloadInstancePOS.position;
            Vector3 forward = payloadInstancePOS.forward;
            if (forward != Vector3.zero)
            {
                spawnRot = Quaternion.LookRotation(forward);
            }
        }

        GameObject payloadGO = null;
        GameObject cartGO = null;

        if (PhotonNetwork.InRoom)
        {
            // Spawn ผ่านเน็ต
            payloadGO = PhotonNetwork.Instantiate(payloadPrefab.name, spawnPos, spawnRot);
        }
        else
        {
            // Spawn ออฟไลน์
            payloadGO = Instantiate(payloadPrefab, spawnPos, spawnRot);
        }

        // --- 2. Spawn Cart (รถพ่วง) ---
        if (enableCart && cartPrefab != null)
        {
            // คำนวณจุดเกิดรถพ่วง
            Vector3 cartPos = spawnPos;
            Quaternion cartRot = spawnRot;

            if (cartInstancePOS != null)
            {
                // ถ้ามีจุดเกิดกำหนดไว้
                cartPos = cartInstancePOS.position;
                cartRot = cartInstancePOS.rotation;
            }
            else
            {
                // ถ้าไม่มีจุดเกิด ให้เกิดข้างหลัง Payload ประมาณ 2 เมตร
                cartPos = spawnPos - (payloadGO.transform.forward * 2.5f);
            }

            if (PhotonNetwork.InRoom)
            {
                cartGO = PhotonNetwork.Instantiate(cartPrefab.name, cartPos, cartRot);
            }
            else
            {
                cartGO = Instantiate(cartPrefab, cartPos, cartRot);
            }
        }

        // --- 3. สั่ง Setup และ Connect ผ่าน RPC ---
        if (PhotonNetwork.InRoom)
        {
            int payloadID = payloadGO.GetComponent<PhotonView>().ViewID;
            // ถ้าไม่มีรถพ่วง ให้ส่ง ID เป็น -1 หรือ 0 ไปแทน
            int cartID = (cartGO != null) ? cartGO.GetComponent<PhotonView>().ViewID : -1;

            photonView.RPC(nameof(RPC_SetupPayloadAndCart), RpcTarget.AllBuffered, payloadID, cartID);
        }
        else
        {
            // Offline Setup
            LocalPayloadSetup(payloadGO, cartGO);
        }
    }

    [PunRPC]
    public void RPC_SetupPayloadAndCart(int payloadViewID, int cartViewID)
    {
        GameObject payloadGO = null;
        GameObject cartGO = null;

        // หา Payload
        PhotonView pView = PhotonView.Find(payloadViewID);
        if (pView != null) payloadGO = pView.gameObject;

        // หา Cart (ถ้าส่งมา)
        if (cartViewID != -1)
        {
            PhotonView cView = PhotonView.Find(cartViewID);
            if (cView != null) cartGO = cView.gameObject;
        }

        if (payloadGO != null)
        {
            LocalPayloadSetup(payloadGO, cartGO);
        }
    }
    private void LocalPayloadSetup(GameObject payloadGO, GameObject cartGO)
    {
        // 1. Setup ตัว Payload หลัก (เหมือนเดิม)
        PayloadScript payloadScript = payloadGO.GetComponent<PayloadScript>();
        if (payloadScript != null)
        {
            payloadScript.PayloadOnSetup();
            RoomManager rManager = GetComponent<RoomManager>();
            if (rManager != null)
            {
                rManager.CurrentPlayingPayload = payloadScript;
            }
        }

        // 2. Setup รถพ่วงและการเชื่อมต่อ
        if (cartGO != null)
        {
            // หาสคริปต์เชื่อมต่อที่รถพ่วง
            CartConnector connector = cartGO.GetComponent<CartConnector>();

            // หาจุดเชื่อม "RearHitch" ที่ Payload (ต้องตั้งชื่อให้ตรงใน Prefab)
            Transform rearHitch = payloadScript.RearHitch;
            Rigidbody leaderRb = payloadGO.GetComponent<Rigidbody>();

            if (connector != null && rearHitch != null && leaderRb != null)
            {
                // สั่งเชื่อมต่อทันที
                connector.ConnectTo(leaderRb, rearHitch);
                Debug.Log("PayloadSetup: Connected Cart to Payload successfully.");
            }
            else
            {
                Debug.LogWarning("PayloadSetup: Failed to connect Cart. Missing Connector, Rigidbody, or 'RearHitch' object.");
            }
        }
    }
}
