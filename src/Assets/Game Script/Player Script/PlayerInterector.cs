using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInterector : MonoBehaviourPun
{
    [Header("Settings")]
    [SerializeField] private float interactRadius = 3f;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private Transform cameraTransform;

    [Header("Debug Settings")]
    [SerializeField] private bool showDebug = true;
    [SerializeField] private Color radiusColor = Color.yellow;
    [SerializeField] private Color targetColor = Color.green;
    [SerializeField] private Color rayColorDefault = Color.blue;
    [SerializeField] private Color rayColorHit = Color.green;

    private IInteractable currentTarget;
    private IInteractable lastTarget; // เพิ่มตัวแปรเก็บเป้าหมายก่อนหน้า
    private GameObject currentTargetObject;

    void Update()
    {
        FindBestInteractable();

        // Debug: แสดงรัศมีรอบตัวผู้เล่น
        if (showDebug)
        {
            DrawRaycastLine();
            DrawDebugRadius();
        }


        if (currentTarget != null && Input.GetKeyDown(KeyCode.E))
        {
            IInteractable itemToPick = currentTarget;
            itemToPick.OnBeginIntereact(this.gameObject); //มีปัญหาตรงนี้
            currentTarget = null;
        }
    }

    private void FindBestInteractable()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRadius, interactLayer);

        IInteractable bestTarget = null;
        GameObject bestTargetObj = null;
        float closestAngle = 0.5f; // ประมาณ 60 องศา


        foreach (var col in colliders)
        {
            if (col == null) continue;

            if (col.TryGetComponent(out IInteractable interactable))
            {
                Vector3 directionToTarget = (col.transform.position - cameraTransform.position).normalized;
                float dot = Vector3.Dot(cameraTransform.forward, directionToTarget);

                // Debug: ลากเส้นไปยังวัตถุทั้งหมดในรัศมี (สีแดงคือยังไม่ถูกเลือก)
                if (showDebug) Debug.DrawLine(cameraTransform.position, col.transform.position, Color.red);

                if (dot > closestAngle)
                {
                    if (HasLineOfSight(col.gameObject))
                    {
                        closestAngle = dot;
                        bestTarget = interactable;
                        bestTargetObj = col.gameObject;
                    }
                }
            }
        }

        currentTarget = bestTarget;
        currentTargetObject = bestTargetObj;

        // Debug: ลากเส้นไปยังวัตถุที่เลือกอยู่ (สีเขียว)
        if (showDebug && currentTargetObject != null)
        {
            Debug.DrawLine(cameraTransform.position, currentTargetObject.transform.position, targetColor);
            Debug.Log($"<color=green>Targeting:</color> {currentTargetObject.name} (Dot: {closestAngle:F2})");
        }

        UpdateUI();
    }

    private bool HasLineOfSight(GameObject target)
    {
        RaycastHit hit;
        if (Physics.Linecast(cameraTransform.position, target.transform.position, out hit))
        {
            return hit.collider.gameObject == target;
        }
        return false;
    }
    private void DrawDebugRadius()
    {
        // วาดวงกลมจำลองรัศมี (แบบง่าย)
        float segments = 20;
        float angle = 0f;
        Vector3 lastPoint = Vector3.zero;

        for (int i = 0; i < segments + 1; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * interactRadius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * interactRadius;
            Vector3 nextPoint = transform.position + new Vector3(x, 0, z);

            if (i > 0) Debug.DrawLine(lastPoint, nextPoint, radiusColor);

            lastPoint = nextPoint;
            angle += 360f / segments;
        }
    }

    private void DrawRaycastLine()
    {
        // คำนวณจุดสิ้นสุดของ Raycast ตามระยะ Interact
        Vector3 forwardDirection = cameraTransform.forward * interactRadius;
        Vector3 endPoint = cameraTransform.position + forwardDirection;

        // ถ้ามี Target ที่ถูกเลือก (Hybrid เลือกมาให้) ให้เปลี่ยนสีเส้นเป็นสีเขียว
        Color currentColor = (currentTarget != null) ? rayColorHit : rayColorDefault;

        // วาดเส้น Raycast ออกจากกลางกล้อง
        Debug.DrawRay(cameraTransform.position, forwardDirection, currentColor);

        // วาด Crosshair เล็กๆ ที่ปลายเส้น Raycast (Optional)
        if (currentTarget != null)
        {
            // ลากเส้นเชื่อมจากปลาย Raycast ไปหาวัตถุเพื่อให้เห็นการ Snap
            Debug.DrawLine(endPoint, currentTargetObject.transform.position, Color.white);
        }
    }

    private void UpdateUI()
    {
        if (PhotonNetwork.InRoom && !photonView.IsMine) return;
        // 1. เช็คว่าเป้าหมายเปลี่ยนไปจากเดิมหรือไม่
        if (currentTarget != lastTarget)
        {
            // 2. เช็คชิ้นเก่า: ต้องเช็คทั้ง null และตรวจสอบว่า object ไม่ได้ถูกทำลาย (Missing Reference)
            // การใช้ lastTarget as MonoBehaviour ช่วยให้เราเช็คสถานะการ Destroy ใน Unity ได้แม่นยำขึ้น
            if (lastTarget != null && !lastTarget.Equals(null))
            {
                try
                {
                    lastTarget.HideWorldInterectUI();
                }
                catch (System.NullReferenceException)
                {
                    // ถ้ามันหายไปแล้วจริงๆ ให้ข้ามไปเลย
                }
            }

            // 3. เปิด UI ชิ้นใหม่
            if (currentTarget != null)
            {
                currentTarget.ShowWorldInterectUI();
            }

            lastTarget = currentTarget;
        }
    }
}
