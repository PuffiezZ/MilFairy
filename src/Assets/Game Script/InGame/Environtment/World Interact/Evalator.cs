using UnityEngine;
using Photon.Pun;

public class Evalator : MonoBehaviourPun
{
    [Header("Elevator Settings")]
    [SerializeField] private Transform platform;
    public float minY = 0f;
    public float maxY = 5f;
    public float speed = 2f;

    private Vector3 targetPosition;
    private bool isMoving = false;

    private void Start()
    {
        // ถ้าไม่ได้ลาก Platform ใส่ใน Inspector จะใช้ตัวมันเองเป็น Platform
        if (platform == null)
        {
            platform = this.transform;
        }
        targetPosition = platform.position;
    }

    private void Update()
    {
        if (isMoving)
        {
            // เลื่อน Platform ไปยังเป้าหมายอย่างสมูท
            platform.position = Vector3.MoveTowards(platform.position, targetPosition, speed * Time.deltaTime);

            // เมื่อถึงเป้าหมายแล้วให้หยุดการทำงาน
            if (Vector3.Distance(platform.position, targetPosition) < 0.01f)
            {
                platform.position = targetPosition;
                isMoving = false;
            }
        }
    }

    public void MoveUp()
    {
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC(nameof(RPC_MoveUp), RpcTarget.All);
        }
        else
        {
            RPC_MoveUp();
        }
    }

    [PunRPC]
    public void RPC_MoveUp()
    {
        targetPosition = new Vector3(platform.position.x, maxY, platform.position.z);
        isMoving = true;
    }

    public void MoveDown()
    {
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC(nameof(RPC_MoveDown), RpcTarget.All);
        }
        else
        {
            RPC_MoveDown();
        }
    }

    [PunRPC]
    public void RPC_MoveDown()
    {
        targetPosition = new Vector3(platform.position.x, minY, platform.position.z);
        isMoving = true;
    }

    private void OnDrawGizmos()
    {
        // ใช้ Platform หรือใช้ตัวเองเป็นจุดอ้างอิงหากยังไม่ได้ลาก Platform ใส่ใน Inspector
        Transform refTransform = platform != null ? platform : transform;

        Vector3 minPosition = new Vector3(refTransform.position.x, minY, refTransform.position.z);
        Vector3 maxPosition = new Vector3(refTransform.position.x, maxY, refTransform.position.z);

        // วาดเส้นเชื่อมระหว่างจุดต่ำสุดและสูงสุด (สีฟ้า)
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(minPosition, maxPosition);

        // วาดกล่องจำลองตำแหน่งสูงสุด (สีแดง) และตำแหน่งต่ำสุด (สีน้ำเงิน)
        Vector3 boxSize = new Vector3(2f, 0.2f, 2f);
        Gizmos.color = Color.red;     Gizmos.DrawWireCube(maxPosition, boxSize);
        Gizmos.color = Color.blue;    Gizmos.DrawWireCube(minPosition, boxSize);
    }
}
