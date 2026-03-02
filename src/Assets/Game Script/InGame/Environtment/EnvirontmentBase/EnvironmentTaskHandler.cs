using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

public class EnvironmentTaskHandler : MonoBehaviour
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

        currentCount++;

        OnProgressUpdated?.Invoke(currentCount, requiredCount);

        if (currentCount >= requiredCount)
        {
            CompleteTask();
        }
    }

    private void CompleteTask()
    {
        _isCompleted = true;
        Debug.Log($"<color=cyan>[Task Manager]</color> {gameObject.name} Completed!");
        OnTaskCompleted?.Invoke();
    }
}