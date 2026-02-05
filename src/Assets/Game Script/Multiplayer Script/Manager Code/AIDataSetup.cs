using NodeCanvas.Framework;
using Photon.Pun;
using UnityEngine;

public class AIDataSetup : MonoBehaviourPun
{
    [SerializeField] private AssetBlackboard shareBlackboard;

    public void FSM_OnSetupDataForAI()
    {
        GameObject payloadRef = GameObject.FindGameObjectWithTag("Payload");
        if(payloadRef == null)
        {
            Debug.LogWarning("Not found Payload");
            return;
        }
        if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {
            int payloadPv = payloadRef.GetComponent<PhotonView>().ViewID;
            photonView.RPC(nameof(RPC_DataSetUp),RpcTarget.All, payloadPv);
        }
        else
        {
            LocalDataSetup(payloadRef);
        }
    }
    [PunRPC]
    private void RPC_DataSetUp(int getPv)
    {
        PhotonView payloadPv = PhotonView.Find(getPv);

        if(payloadPv != null)
        {
            GameObject payloadRef = payloadPv.gameObject;
            LocalDataSetup(payloadRef);
        }
        else
        {
            Debug.LogWarning("Cannot Find Payload pvID");
        }
    }

    private void LocalDataSetup(GameObject payloadRef)
    {
        shareBlackboard.SetVariableValue("PayloadGameobject", payloadRef);
        Debug.Log("Data Setup!");
    }
}
