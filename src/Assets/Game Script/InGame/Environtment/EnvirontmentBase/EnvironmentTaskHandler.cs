using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

public class EnvironmentTaskHandler : MonoBehaviour
{
    [Header("Goal Settings")]
    [Tooltip("จำนวนครั้งที่ต้องได้รับรายงานเพื่อให้งานสำเร็จ")]
    [SerializeField] private int requiredCount = 3;

    [Header("Progress Events")]
    public UnityEvent<int, int> OnProgressUpdated; // ส่งค่า (ปัจจุบัน, เป้าหมาย) ออกไปทำ UI ได้
    public UnityEvent OnTaskCompleted;

    [Header("Debug Info")]
    [ReadOnly][SerializeField] private int currentCount = 0;
    private bool _isCompleted = false;

    // ฟังก์ชันนี้คือ "จุดรับเรื่อง" ที่วัตถุอื่นๆ จะต้องมาเรียกใช้
    public void ReportTaskProgress()
    {
        if (_isCompleted) return;

        currentCount++;

        // ส่ง Event บอกว่าคืบหน้าไปเท่าไหร่ (เช่น เอาไปโชว์เลข 1/3 บนหน้าจอ)
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