using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class ChangeMaterial : MonoBehaviourPun
{
    [Header("Target Settings")]
    [Tooltip("Renderer ของวัตถุที่ต้องการเปลี่ยน Material (ถ้าว่างจะดึงจากตัวมันเอง)")]
    [SerializeField] private Renderer targetRenderer;

    [Header("Material List")]
    [Tooltip("รายการ Material ที่ต้องการใช้งาน")]
    [SerializeField] private Material[] materials;

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();
    }

    /// <summary>
    /// เรียกฟังก์ชันนี้เพื่อเปลี่ยน Material ตาม Index ใน Array
    /// </summary>
    public void ChangeToMaterial(int index)
    {
        if (materials == null || index < 0 || index >= materials.Length)
        {
            Debug.LogWarning($"[ChangeMaterial] Index {index} out of bounds on {gameObject.name}");
            return;
        }

        if (PhotonNetwork.InRoom)
        {
            // ส่ง RPC ให้ทุกคนเปลี่ยน Material พร้อมกัน (Buffered เพื่อให้คนมาทีหลังเห็นด้วย)
            photonView.RPC(nameof(RPC_ApplyMaterial), RpcTarget.AllBuffered, index);
        }
        else
        {
            LocalApplyMaterial(index);
        }
    }

    [PunRPC]
    public void RPC_ApplyMaterial(int index) => LocalApplyMaterial(index);

    private void LocalApplyMaterial(int index)
    {
        if (targetRenderer != null && materials[index] != null)
        {
            targetRenderer.material = materials[index];
        }
    }
}
