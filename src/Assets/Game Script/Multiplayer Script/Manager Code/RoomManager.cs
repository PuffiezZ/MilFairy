using NaughtyAttributes;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerObject; 
    [SerializeField] private Transform[] spawnPoints; 
    [SerializeField] private Transform winPosition;
    [SerializeField] private float winDistanceThreshold = 5f;

    private const string mainMenuName = "Mainmenu";
    private PayloadSetup payloadSetup;
    private AIDataSetup aiDataSetup;
    public PayloadScript CurrentPlayingPayload { get; set; }
    public static RoomManager Instance { get; private set; }
    private bool isVictoryTriggered = false;
    private bool isDefeatTriggered = false;
    public static UnityAction OnWinTriggered;
    public static UnityAction OnLoseTriggered;

    private void Awake()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.OfflineMode = true;
        }
        payloadSetup = GetComponent<PayloadSetup>();
        aiDataSetup = GetComponent<AIDataSetup>();
        Instance = this;
    }
    public void StartRoomManager()
    {
        if (PhotonNetwork.InRoom)
        {
            payloadSetup.OnInstancePayload();
            aiDataSetup.FSM_OnSetupDataForAI();

            SpawnPlayer();
        }
        else
        {
            Transform selectedPoint = spawnPoints[0];
            GameObject player = Instantiate(playerObject, selectedPoint.position, Quaternion.identity);

            payloadSetup.OnInstancePayload();
            aiDataSetup.FSM_OnSetupDataForAI();

            player.GetComponent<PlayerSetup>().IsLocalPlayer();
        }
    }

    // �óշ���Ҩ����Ŵ Scene �ҡ�͹�����ͧ�����
    public override void OnJoinedRoom()
    {
        SpawnPlayer();
    }

    private void Update()
    {
        if (isVictoryTriggered || CurrentPlayingPayload == null || winPosition == null) return;

        // ให้ MasterClient หรือ Offline mode เป็นคนประมวลผล เพื่อไม่ให้ยิง RPC ซ้ำซ้อนกัน
        if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient) return;

        float distance = Vector3.Distance(CurrentPlayingPayload.transform.position, winPosition.position);
        if (distance <= winDistanceThreshold)
        {
            TriggerWinCondition();
        }
    }

    private void SpawnPlayer()
    {
        Transform selectedPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject player = PhotonNetwork.Instantiate(playerObject.name, selectedPoint.position, Quaternion.identity);

        player.GetComponent<PlayerSetup>().IsLocalPlayer();
    }

    public void TriggerWinCondition()
    {
        if (isVictoryTriggered) return;
        isVictoryTriggered = true;

        if (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
        {
            photonView.RPC(nameof(RPC_OnPayloadReachedGoal), RpcTarget.All);
        }
        else
        {
            LocalWinHandle();
        }
    }
    
    public void TriggerLoseCondition()
    {
        if(isDefeatTriggered) return;
        isDefeatTriggered = true;

        if(PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
        {
            photonView.RPC(nameof(RPC_Lose), RpcTarget.All);
        }
        else
        {
            LocalLoseHandle();
        }
    }
    [PunRPC]
    private void RPC_Lose()
    {
        LocalLoseHandle();
        Debug.Log("<color=red>!!! DEFEAT !!!</color>");
    }
    
    public void LocalLoseHandle()
    {
        CurrentPlayingPayload.SetPayloadSpeed(0);
        OnLoseTriggered?.Invoke();
        
        StartCoroutine(DelayDisconnect(5f));
    }

    [PunRPC]
    private void RPC_OnPayloadReachedGoal()
    {
        LocalWinHandle();
        Debug.Log("<color=green>!!! VICTORY !!! Payload reached 100%</color>");
    }

    private void LocalWinHandle()
    {
        CurrentPlayingPayload.SetPayloadSpeed(0);
        OnWinTriggered?.Invoke();

        StartCoroutine(DelayDisconnect(5f));
    }
    private IEnumerator DelayDisconnect(float duration)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            // ��ش NavMeshAgent ����������ӹǳ��鹷ҧ���
            var agent = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.ResetPath(); // ��ҧ Path ��駻�ͧ�ѹ Error ResetPath
            }

            // ��ش�ĵԡ����ͧ FSM (NodeCanvas)
            var fsmOwner = enemy.GetComponent<MonsterState>();
            if (fsmOwner != null)
            {
                fsmOwner.FSMOwner.StopBehaviour();
            }
        }
        yield return new WaitForSeconds(duration);
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
        SceneManager.LoadScene(mainMenuName);
    }

    private void OnDrawGizmos()
    {
        if (winPosition != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(winPosition.position, winDistanceThreshold);
        }
    }
}