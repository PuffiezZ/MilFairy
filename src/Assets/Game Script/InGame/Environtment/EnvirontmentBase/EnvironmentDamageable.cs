using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

// ������÷Ѵ���������� Unity ��� Component ����������ѵ��ѵ�
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonView))]
public class EnvironmentDamageable : MonoBehaviourPun, IDamageable
{
    [Header("General Setting")]
    [SerializeField] private bool enableDamage = true;

    [Header("Visual Shake Settings")]
    [Tooltip("�ҡ Model �١�����ç��� �ҡ�������ҧ����蹷�����ͧ")]
    [SerializeField] private Transform visualTransform;
    [SerializeField] private float shakeIntensity = 0.15f;
    [SerializeField] private float shakeDuration = 0.12f;
    [SerializeField] private float recoverySpeed = 8f;

    [Header("Gameplay Events")]
    public UnityEvent OnHit;
    public UnityEvent OnDestroyed;

    [Header("Stats")]
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private bool canBeDestroyed = false;

    private Vector3 _originalLocalPos;
    private float _currentShakeTimer;

    public float CurrentHealth { get; private set; }
    public float MaxHealth => maxHealth;

    public bool EnableDamage { get; set; }

    private void Reset()
    {
        PhotonView pv = GetComponent<PhotonView>();
        pv.Synchronization = ViewSynchronization.Off;
    }

    private void Awake()
    {
        CurrentHealth = maxHealth;
        EnableDamage = enableDamage;

        if (visualTransform == null) 
            visualTransform = transform;

        _originalLocalPos = visualTransform.localPosition;
    }

    private void Update()
    {
        HandleShakeEffect();
    }

    // --- IDamageable Implementation ---
    public void TakeDamage(float damage)
    {
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC(nameof(RPC_HandleHit), RpcTarget.All, damage);
        }
        else
        {
            LocalHandleHit(damage);
        }
    }

    [PunRPC]
    private void RPC_HandleHit(float damage)
    {
        LocalHandleHit(damage);
    }

    private void LocalHandleHit(float damage)
    {
        _currentShakeTimer = shakeDuration;
        CurrentHealth -= damage;

        OnHit?.Invoke();

        if (canBeDestroyed && CurrentHealth <= 0)
        {
            OnDestroyed?.Invoke();
            if (PhotonNetwork.IsMasterClient) PhotonNetwork.Destroy(gameObject);
        }
    }

    private void HandleShakeEffect()
    {
        if (_currentShakeTimer > 0)
        {
            visualTransform.localPosition = _originalLocalPos + Random.insideUnitSphere * shakeIntensity;
            _currentShakeTimer -= Time.deltaTime;
        }
        else if (visualTransform.localPosition != _originalLocalPos)
        {
            visualTransform.localPosition = Vector3.Lerp(visualTransform.localPosition, _originalLocalPos, Time.deltaTime * recoverySpeed);
        }
    }
}