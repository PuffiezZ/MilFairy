using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public abstract class NetworkAction : MonoBehaviourPun
{
    [Header("Network Action Settings")]
    public bool IsProcessing = false; // เช็คว่ากำลังทำงานอยู่หรือไม่

    // เพิ่ม UnityEvent ให้ Artist ใส่ Logic พิเศษได้จาก Inspector (เช่น ให้พลังผู้เล่น)
    [SerializeField] protected UnityEvent onLocalExecute;

    public void TriggerAction()
    {
        // ถ้ากำลังทำงานอยู่ ไม่ให้ส่ง RPC ซ้ำ
        if (IsProcessing) return;

        if (PhotonNetwork.InRoom)
            photonView.RPC(nameof(RPC_Execute), RpcTarget.AllBuffered);
        else
            ExecuteLogic();
    }

    [PunRPC]
    private void RPC_Execute()
    {
        IsProcessing = true;
        ExecuteLogic();
        onLocalExecute?.Invoke(); // รัน Logic เพิ่มเติมที่ Artist ใส่ไว้
    }

    protected abstract void ExecuteLogic();

    // ฟังก์ชันให้ Artist เรียกใช้เมื่อจบการทำงาน (เช่น เรียกใน Animation Event)
    public void FinishAction()
    {
        IsProcessing = false;
    }
}