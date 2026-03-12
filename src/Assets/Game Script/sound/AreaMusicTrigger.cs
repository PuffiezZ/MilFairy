using UnityEngine;
using Photon.Pun;

public class AreaMusicTrigger : MonoBehaviour
{
    [Header("Music Settings")]
    [Tooltip("ลากไฟล์เพลงที่ต้องการให้เล่นเมื่อเข้าโซนนี้")]
    [SerializeField] private AudioClip areaBGM;

    [Header("Trigger Settings")]
    [Tooltip("ถ้าติ๊กถูก จะเปลี่ยนเพลงเมื่อ 'กล้อง' เข้าใกล้ / ถ้าไม่ติ๊ก จะเปลี่ยนเมื่อ 'ตัวละครเรา' เดินเข้าไป")]
    [SerializeField] private bool triggerByCamera = false;

    private void OnTriggerEnter(Collider other)
    {
        if (Main.Instance == null || areaBGM == null) return;

        if (triggerByCamera)
        {
            // ตรวจสอบว่าสิ่งที่เดินเข้ามาคือ Main Camera หรือไม่
            if (other.CompareTag("MainCamera"))
            {
                Main.Instance.PlayBGM(areaBGM);
            }
        }
        else
        {
            // ตรวจสอบว่าเป็นตัวละครของเราเอง (Local Player) เท่านั้น
            // เพื่อไม่ให้เพลงของเราเปลี่ยนตอนเพื่อนเดินเข้าโซน
            PhotonView pv = other.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)
            {
                Main.Instance.PlayBGM(areaBGM);
            }
        }
    }
    
    // แนะนำ: ให้ใส่ Box Collider และติ๊กถูกที่ Is Trigger ใน Object นี้ด้วย
}