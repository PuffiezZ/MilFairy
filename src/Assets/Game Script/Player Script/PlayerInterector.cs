using ParadoxNotion;
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
    private IInteractable lastTarget; // �����������������¡�͹˹��
    private GameObject currentTargetObject;

    void Update()
    {
        if(PhotonNetwork.InRoom)
        {
            if(!photonView.IsMine) return;    
        }
        
        FindBestInteractable();

        // Debug: �ʴ�������ͺ��Ǽ�����
        if (showDebug)
        {
            DrawRaycastLine();
            DrawDebugRadius();
        }


        if (currentTarget != null && Input.GetKeyDown(KeyCode.E))
        {
            IInteractable interactable = currentTarget;
            interactable.OnBeginIntereact(this.gameObject); //�ջѭ�ҵç���
            currentTarget = null;
        }
    }

    private void FindBestInteractable()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRadius, interactLayer);

        IInteractable bestTarget = null;
        GameObject bestTargetObj = null;
        float closestAngle = 0.5f; // ����ҳ 60 ͧ��


        foreach (var col in colliders)
        {
            if (col == null) continue;

            if (col.TryGetComponent(out IInteractable interactable))
            {
                Vector3 directionToTarget = (col.transform.position - cameraTransform.position).normalized;
                float dot = Vector3.Dot(cameraTransform.forward, directionToTarget);

                // Debug: �ҡ�����ѧ�ѵ�ط����������� (��ᴧ����ѧ���١���͡)
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

        // Debug: �ҡ�����ѧ�ѵ�ط�����͡���� (������)
        if (showDebug && currentTargetObject != null)
        {
            Debug.DrawLine(cameraTransform.position, currentTargetObject.transform.position, targetColor);
            //Debug.Log($"<color=green>Targeting:</color> {currentTargetObject.name} (Dot: {closestAngle:F2})");
        }

        UpdateUI();
    }

    private bool HasLineOfSight(GameObject target)
    {
        Vector3 start = cameraTransform.position;
        Vector3 end = target.transform.position;
        Vector3 direction = end - start;
        float distance = direction.magnitude;

        // ใช้ RaycastAll เพื่อหาวัตถุทั้งหมดที่ขวางอยู่ในเส้นทาง
        RaycastHit[] hits = Physics.RaycastAll(start, direction.normalized, distance);
        
        // เรียงลำดับ hits ตามระยะทาง (ใกล้ไปไกล)
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (var hit in hits)
        {
            // 1. ถ้าเจอเป้าหมายก่อน แสดงว่ามี Line of Sight
            if (hit.collider.gameObject == target) return true;

            // 2. ถ้าเจอวัตถุที่มี Tag หรือ Layer ที่ต้องการ Ignore ให้ข้ามไป (Continue)
            if (hit.collider.CompareTag("InvisibleCollision") || hit.collider.gameObject.layer == LayerMask.NameToLayer("InvisibleCollision")) 
                continue;
            if (hit.collider.isTrigger) continue; // ปกติเราจะข้าม Trigger zones ต่างๆ ด้วย

            // 3. ถ้าเจอวัตถุอื่นที่ทึบ (เช่น กำแพงจริง) ขวางอยู่ก่อนถึงเป้าหมาย ให้ถือว่าไม่มี Line of Sight
            return false;
        }

        return false;
    }
    private void DrawDebugRadius()
    {
        // �Ҵǧ������ͧ����� (Ẻ����)
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
        // �ӹǳ�ش����ش�ͧ Raycast ������� Interact
        Vector3 forwardDirection = cameraTransform.forward * interactRadius;
        Vector3 endPoint = cameraTransform.position + forwardDirection;

        // ����� Target ���١���͡ (Hybrid ���͡�����) �������¹�������������
        Color currentColor = (currentTarget != null) ? rayColorHit : rayColorDefault;

        // �Ҵ��� Raycast �͡�ҡ��ҧ���ͧ
        Debug.DrawRay(cameraTransform.position, forwardDirection, currentColor);

        // �Ҵ Crosshair ���� ��������� Raycast (Optional)
        if (currentTarget != null)
        {
            // �ҡ���������ҡ���� Raycast ����ѵ�����������繡�� Snap
            Debug.DrawLine(endPoint, currentTargetObject.transform.position, Color.white);
        }
    }

    private void UpdateUI()
    {
        if (PhotonNetwork.InRoom && !photonView.IsMine) return;
        // 1. ����������������¹仨ҡ����������
        if (currentTarget != lastTarget)
        {
            // 2. �礪�����: ��ͧ�礷�� null ��е�Ǩ�ͺ��� object �����١����� (Missing Reference)
            // ����� lastTarget as MonoBehaviour ������������ʶҹС�� Destroy � Unity ������Ӣ��
            if (lastTarget != null && !lastTarget.Equals(null))
            {
                try
                {
                    lastTarget.HideWorldInterectUI();
                }
                catch (System.NullReferenceException)
                {
                    // ����ѹ�������Ǩ�ԧ� ����������
                }
            }

            // 3. �Դ UI �������
            if (currentTarget != null)
            {
                currentTarget.ShowWorldInterectUI();
            }

            lastTarget = currentTarget;
        }
    }
}
