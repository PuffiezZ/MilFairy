using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

// เพิ่มบรรทัดนี้เพื่อให้ Unity ใส่ Component ที่จำเป็นให้อัตโนมัติ
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonView))]
public class EnvironmentDamageable : MonoBehaviourPun, IDamageable
{
    [Header("General Setting")]
    [SerializeField] private bool enableDamage = true;

    [Header("Visual Shake Settings")]
    [Tooltip("ลาก Model ลูกมาใส่ตรงนี้ หากปล่อยว่างจะสั่นที่ตัวเอง")]
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


    // ฟังก์ชันนี้จะทำงานอัตโนมัติเมื่อลาก Script นี้ไปแปะใน Inspector
    private void Reset()
    {
        // ตั้งค่า PhotonView เบื้องต้นให้ Designer เลย
        PhotonView pv = GetComponent<PhotonView>();
        pv.Synchronization = ViewSynchronization.Off; // ปิดการส่ง data ต่อเนื่อง (ใช้แค่ RPC)
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
        // เช็คว่าอยู่ใน Room และเป็นเจ้าของ Master หรือไม่ (เพื่อป้องกันการตีเบิ้ลในบางกรณี)
        // แต่สำหรับ Environment ทั่วไป ใช้ RPC Target All ก็เพียงพอแล้ว
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
            // Optional: ถ้าจะทำลายทิ้งในระบบ Network
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