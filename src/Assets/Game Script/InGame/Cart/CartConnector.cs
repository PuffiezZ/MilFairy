using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartConnector : MonoBehaviourPun
{
    [Header("Settings")]
    [Tooltip("ลาก Configurable Joint ของตัวรถพ่วงนี้มาใส่")]
    [SerializeField] private ConfigurableJoint hitchJoint;

    [Tooltip("ลาก Transform ของด้ามจับด้านหน้ามาใส่ (เพื่อใช้เป็นจุดหมุน)")]
    [SerializeField] private Transform frontHandleTip;

    [Header("Target Connection (Optional Setup via Inspector)")]
    [Tooltip("ถ้าจะเชื่อมแต่แรก ให้ลาก Rigidbody ของคันหน้ามาใส่ตรงนี้")]
    [SerializeField] private Rigidbody leaderCart;
    [Tooltip("ถ้าจะเชื่อมแต่แรก ให้ลาก RearHitch ของคันหน้ามาใส่ตรงนี้")]
    [SerializeField] private Transform leaderHitchPoint;

    [Header("Tilt Settings")]
    [Tooltip("องศาที่ยอมให้รถกระดกขึ้นลงได้ (แนะนำ 15-25 องศา)")]
    [SerializeField] private float maxTiltAngle = 20f; // <-- เพิ่มตัวแปรนี้

    [Header("Connection Style")]
    [Tooltip("ถ้าติ๊กถูก = ใช้โซ่/เชือก (มีระยะฟรี), ถ้าไม่ติ๊ก = ใช้เหล็กแข็ง (ล็อคตาย)")]
    public bool useRopeStyle = true;

    [Tooltip("ความยาวของสายลาก (ระยะฟรีที่ยอมให้รถห่างจากจุดเกาะได้)")]
    [SerializeField] private float ropeLength = 0.5f;

    /// <summary>
    /// สั่งเชื่อมต่อรถพ่วงเข้ากับคันหน้า
    /// </summary>
    /// <param name="leaderRb">Rigidbody ของคันหน้า</param>
    /// <param name="hitchPoint">จุดเชื่อมต่อท้ายรถคันหน้า</param>
    public void ConnectTo(Rigidbody leaderRb, Transform hitchPoint)
    {
        // 1. จัดตำแหน่ง (เหมือนเดิม)
        Vector3 offsetFromHandle = transform.position - frontHandleTip.position;
        transform.position = hitchPoint.position + offsetFromHandle;
        transform.rotation = leaderRb.rotation;

        // 2. ตั้งค่า Joint พื้นฐาน
        hitchJoint.connectedBody = leaderRb;
        hitchJoint.autoConfigureConnectedAnchor = false;
        hitchJoint.anchor = transform.InverseTransformPoint(frontHandleTip.position);
        hitchJoint.connectedAnchor = leaderRb.transform.InverseTransformPoint(hitchPoint.position);

        // --- 3. ตั้งค่ารูปแบบการดึง (Linear Motion) ---
        if (useRopeStyle)
        {
            // แบบโซ่/เชือก: ยอมให้ขยับได้ภายในระยะที่กำหนด (Limited)
            hitchJoint.xMotion = ConfigurableJointMotion.Limited;
            hitchJoint.yMotion = ConfigurableJointMotion.Limited;
            hitchJoint.zMotion = ConfigurableJointMotion.Limited;

            // กำหนดความยาวเชือก (Limit)
            SoftJointLimit linearLimit = new SoftJointLimit();
            linearLimit.limit = ropeLength; // ระยะห่างสูงสุดที่ยอมได้
            hitchJoint.linearLimit = linearLimit;
        }
        else
        {
            // แบบเหล็กแข็ง: ล็อคตายติดหนึบ (Locked)
            hitchJoint.xMotion = ConfigurableJointMotion.Locked;
            hitchJoint.yMotion = ConfigurableJointMotion.Locked;
            hitchJoint.zMotion = ConfigurableJointMotion.Locked;
        }

        // --- 4. ตั้งค่าการหมุน/กระดก (Angular Motion) ---
        hitchJoint.angularXMotion = ConfigurableJointMotion.Limited; // กระดกได้

        SoftJointLimit tiltLimit = new SoftJointLimit();
        tiltLimit.limit = maxTiltAngle;
        hitchJoint.lowAngularXLimit = tiltLimit;
        hitchJoint.highAngularXLimit = tiltLimit;

        hitchJoint.angularYMotion = ConfigurableJointMotion.Free;   // เลี้ยวได้อิสระ
        hitchJoint.angularZMotion = ConfigurableJointMotion.Locked; // ห้ามเอียงตะแคง

        Physics.IgnoreCollision(leaderRb.GetComponent<Collider>(), GetComponent<Collider>(), true);
    }
}
