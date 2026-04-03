using Photon.Pun;
using UnityEngine;

public class StarCoral : MonoBehaviourPun
{
    public Color color;
    
    [Header("Settings")]
    [SerializeField] private Renderer targetRenderer;

    private void Awake()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
        }
    }

    public void ChangeColorWhenHit()
    {
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC(nameof(RPC_ChangeColor), RpcTarget.All);
        }
        else
        {
            RPC_ChangeColor();
        }
    }

    [PunRPC]
    private void RPC_ChangeColor()
    {
        if (targetRenderer != null)
        {
            // สร้าง Material Instance ใหม่และทำการเปลี่ยนสี
            targetRenderer.material.color = color;
        }
    }
}
