using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using UnityEngine.Events;

public class EnvironmentPlate : MonoBehaviourPunCallbacks
{
    public UnityEvent OnPlateEntered;
    public UnityEvent OnPlateExited;

    [Header("Detection Settings")]
    [SerializeField] private LayerMask detectionLayer;
    [SerializeField] private string playerTag = "Player";

    private HashSet<int> _activeColliders = new HashSet<int>();
    private bool _isLocked = false;
    private bool _isPressed = false;

    public bool IsPressed => _isPressed;
    public bool IsLocked => _isLocked;

    /// <summary>
    /// ล็อคสถานะของ Plate ไม่ให้ทำงานต่อหลังจากเงื่อนไขเสร็จสิ้น
    /// รองรับทั้ง Solo และ P2P (Sync ผ่าน RPC)
    /// </summary>
    public void SetLock(bool lockState)
    {
        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
        {
            photonView.RPC(nameof(RPC_SetLock), RpcTarget.AllBuffered, lockState);
        }
        else
        {
            InternalSetLock(lockState);
        }
    }

    [PunRPC]
    private void RPC_SetLock(bool lockState)
    {
        InternalSetLock(lockState);
    }

    private void InternalSetLock(bool lockState)
    {
        _isLocked = lockState;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (_isLocked) return;

        if (IsValidObject(other))
        {
            if (_activeColliders.Add(other.GetInstanceID()))
            {
                EvaluateState();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_isLocked) return;

        if (IsValidObject(other))
        {
            if (_activeColliders.Remove(other.GetInstanceID()))
            {
                EvaluateState();
            }
        }
    }

    private bool IsValidObject(Collider other)
    {
        // ตรวจสอบว่าเป็น Player หรือมี Component HoldableObject หรือไม่
        // ใช้ GetComponentInParent เผื่อกรณี Collider อยู่ที่ลูกของ Object
        bool hasHoldable = other.GetComponentInParent<HoldableObject>() != null;
        bool isPlayer = other.CompareTag(playerTag);
        
        bool isInLayer = detectionLayer == 0 || (detectionLayer.value & (1 << other.gameObject.layer)) != 0;

        return (isPlayer || hasHoldable) && (detectionLayer.value == 0 || isInLayer);
    }

    private void EvaluateState()
    {
        bool shouldBePressed = _activeColliders.Count > 0;

        if (shouldBePressed && !_isPressed)
        {
            _isPressed = true;
            OnPlateEntered?.Invoke();
            Debug.Log($"[EnvironmentPlate] {gameObject.name} Pressed");
        }
        else if (!shouldBePressed && _isPressed)
        {
            _isPressed = false;
            OnPlateExited?.Invoke();
            Debug.Log($"[EnvironmentPlate] {gameObject.name} Released");
        }
    }
}
