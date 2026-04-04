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

    private const string mainMenuName = "Mainmenu 1";
    private PayloadSetup payloadSetup;
    private AIDataSetup aiDataSetup;
    public PayloadScript CurrentPlayingPayload { get; set; }
    public static RoomManager Instance { get; private set; }
    public bool isVictoryTriggered = false;
    public bool isDefeatTriggered = false;
    public static UnityAction<float,bool,float> OnEndTriggered;

    private float elapsedTime = 0f;
    private bool isTimerRunning = false;

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

    public void StartGameplayTimer()
    {
        // ให้เฉพาะ Host หรือการเล่น Offline เป็นคนนับเวลา
        if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient) return;
        isTimerRunning = true;
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;
        }

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

    public void RespawnPlayer(Player player, float delay = 3f)
    {
        StartCoroutine(RespawnCoroutine(player, delay));
    }

    private IEnumerator RespawnCoroutine(Player player, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (player != null && (!PhotonNetwork.InRoom || player.photonView.IsMine))
        {
            Transform selectedPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            player.transform.position = selectedPoint.position;
            player.transform.rotation = selectedPoint.rotation;
            
            player.Respawn();
        }
    }

    public void TriggerWinCondition()
    {
        if (isVictoryTriggered) return;
        isVictoryTriggered = true;
        isTimerRunning = false; // หยุดเวลาเมื่อจบเกม

        if (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
        {
            photonView.RPC(nameof(RPC_OnPayloadReachedGoal), RpcTarget.All, elapsedTime);
        }
        else
        {
            LocalWinHandle(elapsedTime);
        }
    }
    
    public void TriggerLoseCondition()
    {
        if(isDefeatTriggered) return;
        isDefeatTriggered = true;
        isTimerRunning = false; // หยุดเวลาเมื่อจบเกม

        if(PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
        {
            photonView.RPC(nameof(RPC_Lose), RpcTarget.All, elapsedTime);
        }
        else
        {
            LocalLoseHandle(elapsedTime);
        }
    }
    [PunRPC]
    public void RPC_Lose(float finalTime)
    {
        LocalLoseHandle(finalTime);
        Debug.Log("<color=red>!!! DEFEAT !!!</color>");
    }
    
    public void LocalLoseHandle(float finalTime)
    {
        float perCentCurrentHP = 0f;
        if (CurrentPlayingPayload != null)
        {
            CurrentPlayingPayload.SetPayloadSpeed(0);
            if (CurrentPlayingPayload.CurrentPlayingToothCart != null)
                perCentCurrentHP = CurrentPlayingPayload.CurrentPlayingToothCart.CurrentHealth / CurrentPlayingPayload.CurrentPlayingToothCart.MaxHealth;
        }

        OnEndTriggered?.Invoke(perCentCurrentHP, false, finalTime);
        
        StartCoroutine(DelayDisconnect(5f));
    }
    
    [PunRPC]
    public void RPC_OnPayloadReachedGoal(float finalTime)
    {
        LocalWinHandle(finalTime);
        Debug.Log("<color=green>!!! VICTORY !!! Payload reached 100%</color>");
    }

    private void LocalWinHandle(float finalTime)
    {
        float perCentCurrentHP = 1f; // Default 100% ถ้าหาไม่เจอในตอนชนะ
        if (CurrentPlayingPayload != null)
        {
            CurrentPlayingPayload.SetPayloadSpeed(0);
            if (CurrentPlayingPayload.CurrentPlayingToothCart != null)
                perCentCurrentHP = CurrentPlayingPayload.CurrentPlayingToothCart.CurrentHealth / CurrentPlayingPayload.CurrentPlayingToothCart.MaxHealth;
        }

        OnEndTriggered?.Invoke(perCentCurrentHP, true, finalTime);

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