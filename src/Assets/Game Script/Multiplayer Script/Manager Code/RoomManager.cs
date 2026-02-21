using NaughtyAttributes;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerObject; // ต้องอยู่ในโฟลเดอร์ Resources เท่านั้น
    [SerializeField] private Transform[] spawnPoints; // ใช้ array เพื่อสุ่มจุดเกิด
    private const string mainMenuName = "Mainmenu";
    private PayloadSetup payloadSetup;
    private AIDataSetup aiDataSetup;
    public PayloadScript CurrentPlayingPayload { get; set; }
    public static RoomManager Instance { get; private set; }
    private bool isVictoryTriggered = false;
    public static UnityAction OnWinTriggered;


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
    void Start()
    {
        // ถ้าเราเปลี่ยน Scene มาโดยที่ยังอยู่ในห้อง (Joined Room แล้ว)
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
            player.GetComponent<PlayerSetup>().SetPayloadInstance();
        }
    }

    // กรณีที่อาจจะโหลด Scene มาก่อนเข้าห้องสำเร็จ
    public override void OnJoinedRoom()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        // สุ่มจุดเกิดไม่ให้ทับกัน
        Transform selectedPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // สร้างตัวละครผ่าน Network
        GameObject player = PhotonNetwork.Instantiate(playerObject.name, selectedPoint.position, Quaternion.identity);

        // เรียก Setup เพื่อเปิดกล้องเฉพาะเครื่องเรา
        player.GetComponent<PlayerSetup>().IsLocalPlayer();
        player.GetComponent<PlayerSetup>().SetPayloadInstance();
    }

    public void TriggerWinCondition()
    {
        if (isVictoryTriggered) return;
        isVictoryTriggered = true;

        if (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
        {
            // แจ้งเตือนทุกคนในห้อง
            photonView.RPC(nameof(RPC_OnPayloadReachedGoal), RpcTarget.All);
        }
        else
        {
            LocalWinHandle();
        }
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
            // หยุด NavMeshAgent เพื่อไม่ให้คำนวณเส้นทางต่อ
            var agent = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.ResetPath(); // ล้าง Path ทิ้งป้องกัน Error ResetPath
            }

            // หยุดพฤติกรรมของ FSM (NodeCanvas)
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
}