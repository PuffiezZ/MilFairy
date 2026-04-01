using NaughtyAttributes;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Splines;

public class PayloadSetup : MonoBehaviourPun
{
    [SerializeField] private bool enablePayload = false;

    [BoxGroup("Payload (Leader)")]
    [ShowIf(nameof(enablePayload))]
    [SerializeField] private GameObject payloadPrefab; // Prefab ����ҡ
    [BoxGroup("Payload (Leader)")]
    [ShowIf(nameof(enablePayload))]
    [SerializeField] private Transform payloadInstancePOS;

    [BoxGroup("Cart (Trailer)")]
    [ShowIf(nameof(enablePayload))]
    [SerializeField] private bool enableCart = true; // �Դ/�Դö��ǧ
    [BoxGroup("Cart (Trailer)")]
    [ShowIf(nameof(enableCart))]
    [SerializeField] private GameObject cartPrefab;    // Prefab ö��ǧ (ToothCart)
    [BoxGroup("Cart (Trailer)")]
    [ShowIf(nameof(enableCart))]
    [SerializeField] private Transform cartInstancePOS; // �ش�Դö��ǧ (�����������Դ��ͷ����ͧ)

    [ShowIf(nameof(enablePayload))]
    [SerializeField] private GameObject objectiveCart; // (�ѹ����ͧ�س)
    public void OnInstancePayload()
    {
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                InstancePayload();
            }
        }
        else
        {
            // �ó���蹤����� (Offline)
            InstancePayload();
        }
    }
    private void InstancePayload()
    {
        if (enablePayload == false) return;

        // --- 1. Spawn Payload (����ҡ) ---
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
            // Spawn ��ҹ��
            payloadGO = PhotonNetwork.Instantiate(payloadPrefab.name, spawnPos, spawnRot);
        }
        else
        {
            // Spawn �Ϳ�Ź�
            payloadGO = Instantiate(payloadPrefab, spawnPos, spawnRot);
        }

        // --- 2. Spawn Cart (ö��ǧ) ---
        if (enableCart && cartPrefab != null)
        {
            // �ӹǳ�ش�Դö��ǧ
            Vector3 cartPos = spawnPos;
            Quaternion cartRot = spawnRot;

            if (cartInstancePOS != null)
            {
                // ����ըش�Դ��˹����
                cartPos = cartInstancePOS.position;
                cartRot = cartInstancePOS.rotation;
            }
            else
            {
                // �������ըش�Դ ����Դ��ҧ��ѧ Payload ����ҳ 2 ����
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
            
            if(cartGO != null)
            {
                PayloadScript payloadScript = payloadGO.GetComponent<PayloadScript>();
                if(payloadScript != null)
                {
                    payloadScript.CurrentPlayingToothCart = cartGO.GetComponent<ToothCart>();
                }   
                
            }
        }

        // --- 3. ��� Setup ��� Connect ��ҹ RPC ---
        if (PhotonNetwork.InRoom)
        {
            int payloadID = payloadGO.GetComponent<PhotonView>().ViewID;
            // ��������ö��ǧ ����� ID �� -1 ���� 0 �᷹
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

        // �� Payload
        PhotonView pView = PhotonView.Find(payloadViewID);
        if (pView != null) payloadGO = pView.gameObject;

        // �� Cart (�������)
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
        // 1. Setup ��� Payload ��ѡ (����͹���)
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

        if (cartGO != null)
        {
            CartConnector connector = cartGO.GetComponent<CartConnector>();

            Transform rearHitch = payloadScript.RearHitch;
            Rigidbody leaderRb = payloadGO.GetComponent<Rigidbody>();

            if (connector != null && rearHitch != null && leaderRb != null)
            {
                // ����������ͷѹ��
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
