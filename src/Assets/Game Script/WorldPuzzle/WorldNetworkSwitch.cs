using UnityEngine;
using UnityEngine.Events;

public class WorldNetworkSwitch : MonoBehaviour, IInteractable
{
    [Header("Links")]
    [SerializeField] private NetworkAction targetAction;
    [SerializeField] private GameObject visualUI;

    [Header("Settings")]
    [SerializeField] private bool canRepeat = true;
    [SerializeField] private float cooldown = 0.5f; // กันการกดรัวๆ

    // เพิ่ม UnityEvent สำหรับสวิตช์โดยเฉพาะ
    [SerializeField] private UnityEvent<GameObject> onInteractWithPlayer;

    private bool _isUsed = false;
    private float _nextCanInteractTime = 0f;

    public void ShowWorldInterectUI()
    {
        // ถ้า Action เป้าหมายกำลังทำงานอยู่ จะไม่ขึ้น UI ให้กด
        if (targetAction != null && targetAction.IsProcessing) return;

        visualUI?.SetActive(true);
    }

    public void HideWorldInterectUI() => visualUI?.SetActive(false);

    public void OnBeginIntereact(GameObject player, bool getBoolean = false)
    {
        if (Time.time < _nextCanInteractTime) return;
        if (_isUsed && !canRepeat) return;
        if (targetAction != null && targetAction.IsProcessing) return; // กันกดซ้ำขณะทำงาน

        if (targetAction != null)
        {
            targetAction.TriggerAction();

            // รัน Logic เสริม เช่น player.GetComponent<PowerUp>().AddPower();
            onInteractWithPlayer?.Invoke(player);

            _isUsed = true;
            _nextCanInteractTime = Time.time + cooldown;
            HideWorldInterectUI();
        }
    }

    public void OnHoldInteract(GameObject player, float progress)
    {
        throw new System.NotImplementedException();
    }

    public void OnCancelInteract()
    {
        throw new System.NotImplementedException();
    }
}