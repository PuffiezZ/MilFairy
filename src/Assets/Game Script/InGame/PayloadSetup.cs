using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class PayloadSetup : MonoBehaviourPun
{
    [Header("Settings")]
    [SerializeField] private GameObject payloadPrefab; // ชื่อ Prefab ในโฟลเดอร์ Resources
    [SerializeField] private SplineContainer targetSpline; // ลาก Spline ที่ต้องการให้รถไปวางมาใส่
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
        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;

        // 1. คำนวณตำแหน่งเริ่มต้นจากจุดที่ 0 ของ Spline
        if (targetSpline != null)
        {
            spawnPos = targetSpline.EvaluatePosition(0); // หาตำแหน่งที่ 0% ของเส้น
            Vector3 forward = targetSpline.EvaluateTangent(0); // หาทิศทางของเส้น
            if (forward != Vector3.zero)
            {
                spawnRot = Quaternion.LookRotation(forward);
            }
        }

        // 2. สั่งสร้างวัตถุผ่านเครือข่าย
        if (PhotonNetwork.InRoom)
        {
            // ต้องแน่ใจว่า Prefab อยู่ในโฟลเดอร์ Resources และลงทะเบียนใน Network Prefabs แล้ว
            GameObject payload = PhotonNetwork.Instantiate(payloadPrefab.name, spawnPos, spawnRot);
            int viewID = payload.GetComponent<PhotonView>().ViewID;
            photonView.RPC(nameof(RPC_SyncPayloadSplineContainer), RpcTarget.AllBuffered, viewID);
        }
        else
        {
            // สร้างแบบธรรมดาถ้าไม่ได้อยู่ในห้อง
            GameObject payloadGO = Instantiate(payloadPrefab, spawnPos, spawnRot);
            LocalPayloadSplineContainer(payloadGO);
        }
    }

    [PunRPC]
    private void RPC_SyncPayloadSplineContainer(int photonViewID)
    {
        PhotonView payloadView = PhotonView.Find(photonViewID);
        if (payloadView != null)
        {
            // แก้ไข: ดึง GameObject จากตัว photonView โดยตรง
            GameObject payloadGO = payloadView.gameObject;

            // ตรวจสอบ targetSpline ในเครื่องนั้นๆ ด้วย
            if (payloadGO != null && targetSpline != null)
            {
                LocalPayloadSplineContainer(payloadGO);
            }
            else
            {
                Debug.LogError("Client side error: Payload GO or targetSpline is missing!");
            }
        }
    }
    private void LocalPayloadSplineContainer(GameObject gameObject)
    {
        PayloadScript payloadScript = gameObject.GetComponent<PayloadScript>();
        payloadScript.PayloadOnSetup(targetSpline);
    }
}
