using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class EnvironmentSwitch : MonoBehaviourPun, IDamageable, IInteractable
{
    [Header("Switch Settings")]
    [SerializeField] private bool isOneTimeUse = false;
    [SerializeField] private float cooldown = 0.5f;
    [SerializeField] private bool enableDamageToggle = true;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject activeVisual;
    [SerializeField] private GameObject inactiveVisual;

    [Header("Events")]
    public UnityEvent OnSwitchActivated;
    public UnityEvent OnSwitchDeactivated;

    private bool _isActive = false;
    private float _lastToggleTime;

    public bool EnableDamage { get; set; }

    private void Start()
    {
        EnableDamage = enableDamageToggle;
        UpdateVisuals();
    }

    // --- IDamageable Implementation (�١�����Ƿӧҹ) ---
    public void TakeDamage(float damage)
    {
        ExecuteToggleLogic();
    }

    // --- IInteractable Implementation (������ E ���ͻ��� Interact ���Ƿӧҹ) ---
    public void OnBeginIntereact(GameObject player, bool getBoolean = false)
    {
        ExecuteToggleLogic();
    }

    public void ShowWorldInterectUI() {}
    public void HideWorldInterectUI() {}
    public void OnHoldInteract(GameObject player, float progress) {}
    public void OnCancelInteract() { }

    // --- Logic Core ---
    private void ExecuteToggleLogic()
    {
        if (Time.time < _lastToggleTime + cooldown) return;
        if (isOneTimeUse && _isActive) return;

        if (PhotonNetwork.InRoom)
        {
            photonView.RPC(nameof(RPC_ToggleSwitch), RpcTarget.All);
        }
        else
        {
            LocalToggle();
        }
    }

    [PunRPC]
    private void RPC_ToggleSwitch() => LocalToggle();

    private void LocalToggle()
    {
        _isActive = !_isActive;
        _lastToggleTime = Time.time;

        if (_isActive) OnSwitchActivated?.Invoke();
        else OnSwitchDeactivated?.Invoke();

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (activeVisual) activeVisual.SetActive(_isActive);
        if (inactiveVisual) inactiveVisual.SetActive(!_isActive);
    }
}