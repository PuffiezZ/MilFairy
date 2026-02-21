using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

// --- Class ย่อยสำหรับเก็บ Reference ---
[System.Serializable]
public class RagdollLimb
{
    [Tooltip("ตั้งชื่อส่วนต่างๆ (เช่น Left Arm, Hips)")]
    public string limbName;

    [Tooltip("ลาก Joint มาใส่")]
    public ConfigurableJoint joint;

    [Tooltip("เปิด - ปิด การซิ้งค์ Animation กับ Ragdoll")]
    public bool syncAnimation = true;

    [Tooltip("ความแข็งแรงเฉพาะส่วน (1 = ปกติ, 2 = แข็งมาก)")]
    [Range(0f, 10f)] public float strengthMultiplier = 1f;
}

// --- Script หลักคุม Slerp Drive ---
public class RagdollBalanceController : MonoBehaviour
{
    [BoxGroup("Slerp Settings")]
    [Tooltip("ค่า Spring หลัก (ความพยายามในการคืนตัว)")]
    [SerializeField] private float masterSpring = 1000f;

    [BoxGroup("Slerp Settings")]
    [Tooltip("ค่า Damper หลัก (ความหนืดกันสั่น)")]
    [SerializeField] private float masterDamper = 50f;

    [BoxGroup("Slerp Settings")]
    [Tooltip("แรงสูงสุดที่ยอมให้ใช้")]
    [SerializeField] private float maxForce = 10000f;

    [BoxGroup("References")]
    [Tooltip("ลาก Capsule Collider ตัวหลักของ Player มาใส่")]
    [SerializeField] private SphereCollider mainCollider;

    [BoxGroup("References")]
    [Tooltip("Joint ของส่วนสะโพก (Spine/Hips) ที่เชื่อมกับตัว Root")]
    [SerializeField] private ConfigurableJoint hipsRootJoint; // เพิ่มตัวนี้เข้ามา!

    [Space(10)]
    [BoxGroup("Limbs Setup")]
    [SerializeField] private RagdollLimb[] ragdollLimbs;

    // เก็บค่าเดิมไว้ตอน Reset
    private float defaultSpring;
    private float defaultDamper;

    private float defaultRadius; // เก็บรัศมีเดิม
    private Vector3 defaultCenter;
    private Vector3 defaultConnectedAnchor; // เก็บจุดยึดเดิม

    private void Awake()
    {
        defaultSpring = masterSpring;
        defaultDamper = masterDamper;

        if (mainCollider != null)
        {
            defaultRadius = mainCollider.radius;
            defaultCenter = mainCollider.center;
        }
        if (hipsRootJoint != null)
        {
            // ต้องปิด Auto Configure ก่อน ไม่งั้นแก้ค่าไม่ได้
            hipsRootJoint.autoConfigureConnectedAnchor = false;
            defaultConnectedAnchor = hipsRootJoint.connectedAnchor;
        }
    }

    private void Start()
    {
        UpdateJoints();
    }

    private void OnValidate()
    {
        // ปรับค่าใน Inspector แล้วเห็นผลทันที
        UpdateJoints();
    }

    [Button("Apply Slerp Settings")]
    public void UpdateJoints()
    {
        if (ragdollLimbs == null) return;

        foreach (var limb in ragdollLimbs)
        {
            if (limb.joint == null) continue;

            if (limb.joint.transform.GetComponent<SyncJoint>())
            {
                limb.joint.transform.GetComponent<SyncJoint>().enableSync = limb.syncAnimation;
            }

            // 1. สำคัญ: ต้องเปลี่ยนโหมดเป็น Slerp ก่อน
            limb.joint.rotationDriveMode = RotationDriveMode.Slerp;

            // 2. คำนวณค่า Spring/Damper
            float finalSpring = masterSpring * limb.strengthMultiplier;
            float finalDamper = masterDamper; // Damper ควรเท่ากันทั้งตัวเพื่อความเสถียร

            JointDrive drive = new JointDrive
            {
                positionSpring = finalSpring,
                positionDamper = finalDamper,
                maximumForce = maxForce
            };

            // 3. ตั้งค่าใส่ Slerp Drive
            limb.joint.slerpDrive = drive;
        }
    }

    // --- ฟังก์ชันสำหรับ Gameplay ---

    [Button("Knockout (Limp)")]
    public void GoLimp()
    {
        masterSpring = 1f; // อ่อนปวกเปียก
        masterDamper = 0f;
        UpdateJoints();

        if (mainCollider != null)
        {
            mainCollider.radius = 0.05f;
            mainCollider.center = new Vector3(defaultCenter.x, 0.05f, defaultCenter.z);
        }

        // 3. *แก้จุดนี้* ย้ายจุดยึดลงมาที่พื้น (0,0,0 ของ Root)
        if (hipsRootJoint != null)
        {
            hipsRootJoint.connectedAnchor = Vector3.zero;
        }
    }

    [Button("Recover (Stand)")]
    public void Recover()
    {
        masterSpring = defaultSpring; // กลับมาแข็งแรง
        masterDamper = defaultDamper;
        UpdateJoints();

        if (mainCollider != null)
        {
            mainCollider.radius = defaultRadius;
            mainCollider.center = defaultCenter;
        }

        if (hipsRootJoint != null)
        {
            hipsRootJoint.connectedAnchor = defaultConnectedAnchor;
        }
    }
}