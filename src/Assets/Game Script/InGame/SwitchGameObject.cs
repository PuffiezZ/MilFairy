using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class SwitchGameObject : MonoBehaviourPun,IInteractable
{
    [SerializeField] private UnityEvent actionToInvoke;

    public void OnBeginIntereact(GameObject player, bool getBoolean = false)
    {
        // ในระบบ Multiplayer ควรให้เจ้าของเครื่องเป็นคนสั่งรันผ่าน RPC
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC(nameof(RPC_SyncInteraction), RpcTarget.All);
        }
        else
        {
            ExecuteInteraction();
        }
    }

    [PunRPC]
    private void RPC_SyncInteraction()
    {
        ExecuteInteraction();
    }

    private void ExecuteInteraction()
    {
        if (actionToInvoke != null)
        {
            // เรียกใช้งาน UnityEvent ได้ทันทีโดยไม่ต้องใช้ Reflection
            actionToInvoke.Invoke();
            Debug.Log("<color=yellow>Interaction Invoked via UnityEvent</color>");
        }
    }

    // ส่วนของ UI (ใช้ตามความเหมาะสม)
    public void ShowWorldInterectUI() { /* แสดงปุ่ม E */ }
    public void HideWorldInterectUI() { /* ซ่อนปุ่ม E */ }
}
