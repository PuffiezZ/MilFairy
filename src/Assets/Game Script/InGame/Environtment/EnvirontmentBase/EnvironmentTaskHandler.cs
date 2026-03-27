using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class EnvironmentTaskHandler : MonoBehaviourPun
{
    [Header("Goal Settings")]
    [Tooltip("�ӹǹ���駷���ͧ���Ѻ��§ҹ�������ҹ�����")]
    [SerializeField] private int requiredCount = 3;

    [Header("Progress Events")]
    public UnityEvent<int, int> OnProgressUpdated; 
    public UnityEvent OnTaskCompleted;

    [Header("Debug Info")]
    [ReadOnly][SerializeField] private int currentCount = 0;
    private bool _isCompleted = false;

    public void ReportTaskProgress()
    {
        if (_isCompleted) return;

        if (PhotonNetwork.InRoom)
        {
            // ป้องกัน Client ทำงานซ้ำซ้อน: ให้ Master Client เท่านั้นที่มีสิทธิ์สั่งอัปเดตไปให้ทุกคน
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC(nameof(RPC_UpdateProgress), RpcTarget.AllBuffered);
            }
        }
        else
        {
            // Offline: ทำงานปกติ
            UpdateProgressInternal();
        }
    }

    [PunRPC]
    private void RPC_UpdateProgress()
    {
        UpdateProgressInternal();
    }

    public void ReduceTaskProgress()
    {
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC(nameof(RPC_ReduceProgress), RpcTarget.AllBuffered);
            }
        }
        else
        {
            ReduceProgressInternal();
        }
    }
    [PunRPC]
    private void RPC_ReduceProgress() 
    {
        ReduceProgressInternal();
    }

    private void UpdateProgressInternal()
    {
        if (_isCompleted) return;

        currentCount++;

        OnProgressUpdated?.Invoke(currentCount, requiredCount);

        if (currentCount >= requiredCount)
        {
            CompleteTask();
        }
    }

    private void ReduceProgressInternal() {
         if (_isCompleted) return;

        currentCount--;

        OnProgressUpdated?.Invoke(currentCount, requiredCount);

        currentCount = Mathf.Max(0, currentCount);

        Debug.Log($"<color=yellow>[Task Manager]</color> {gameObject.name} Reduce current Progress!");
    }

    private void CompleteTask()
    {
        _isCompleted = true;
        Debug.Log($"<color=cyan>[Task Manager]</color> {gameObject.name} Completed!");
        OnTaskCompleted?.Invoke();
    }
}