using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;

public class EnvirontmentRequestmentHoldableObject : MonoBehaviourPun, IInteractable
{
    [Tooltip("ลาก Prefab ของ HoldableObject ที่ต้องการมาใส่")]
    [SerializeField] private GameObject requiredObjectPrefab;
    public UnityEvent OnTaskSuccess;
    public void OnBeginIntereact(GameObject player, bool getBoolean = false)
    {
        Debug.Log("Interact with EnvirontmentRequestmentHoldableObject");
        //if (player == null) return;
        Player playerComponent = player.GetComponent<Player>();
        if (playerComponent == null) return;
        if (playerComponent.CurrentHoldable == null) 
        {
            Debug.Log("<color=orange>[Interact]</color> Player is not holding any item.");
            return;
        }

        if (requiredObjectPrefab == null)
        {
            Debug.LogError($"Required Object Prefab หรือ ItemID ของมันไม่ได้ถูกตั้งค่าใน Inspector ของ {gameObject.name}!", this);
            return;
        }
        HoldableObject heldObject = playerComponent.CurrentHoldable;
        HoldableObject targetHoldable = requiredObjectPrefab.GetComponent<HoldableObject>();

        if (targetHoldable == null) return;
        // ตรวจสอบว่า ItemID ตรงกันหรือไม่ (ใช้เปรียบเทียบ Instance กับ Prefab)
        if (heldObject.ItemID == targetHoldable.ItemID)
        {
            if (PhotonNetwork.InRoom)
            {
                // Online: ส่ง RPC ให้ทุกคนจัดการเรื่องการลบไอเทมและ Reset ตัวละคร
                photonView.RPC(nameof(RPC_ProcessSuccess), RpcTarget.All, playerComponent.photonView.ViewID, heldObject.photonView.ViewID);
            }
            else
            {
                // Solo: ทำงานทันที
                ExecuteSuccessLogic(playerComponent, heldObject);
                OnTaskSuccess?.Invoke();
            }
        }
        else
        {
            Debug.Log($"Incorrect object. Required: {targetHoldable.ItemID}, Held: {heldObject.ItemID}");
        }
    }
    [PunRPC]
    private void RPC_ProcessSuccess(int playerViewID, int holdableViewID)
    {
        PhotonView pView = PhotonView.Find(playerViewID);
        PhotonView hView = PhotonView.Find(holdableViewID);

        if (pView != null)
        {
            Player p = pView.GetComponent<Player>();
            HoldableObject h = hView != null ? hView.GetComponent<HoldableObject>() : null;
            ExecuteSuccessLogic(p, h);
        }
        OnTaskSuccess?.Invoke();
    }
    private void ExecuteSuccessLogic(Player player, HoldableObject holdable)
    {
        // 1. คืนค่าสถานะให้ผู้เล่น (Reset Animations & Actions)
        player.SetHoldableObject(player.photonView, null);
        player.GetComponent<PlayerAnimation>().SetArmLayerWeight(0f);

        PlayerCombat combat = player.GetComponent<PlayerCombat>();
        Player.SetActionLeftClick(player.photonView, combat.OnInvokeAttack);

        // 2. ทำลายไอเทม
        if (holdable != null)
        {
            if (PhotonNetwork.InRoom)
            {
                if(holdable.photonView.IsMine)
                    PhotonNetwork.Destroy(holdable.gameObject);
            }
            else
            {
                Destroy(holdable.gameObject);
            }

        }

    }



    public void OnCancelInteract() { }

    public void OnHoldInteract(GameObject player, float progress) { }

    public void ShowWorldInterectUI() { /*  E */ }

    public void HideWorldInterectUI() { /* Դ E */ }

}