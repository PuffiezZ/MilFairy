using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform targetEntity; // ตัวละครที่เป็นเจ้าของ Marker นี้
    [SerializeField] private bool rotateWithTarget = true; // ติ๊กถ้าต้องการให้หมุนตามตัวละคร

    // ระยะความสูงที่ Marker จะลอยอยู่เหนือตัวละคร (เพื่อไม่ให้จมดิน)
    [SerializeField] private float yOffsetPOS = 0f; 

    void LateUpdate()
    {
        if (targetEntity == null) return;

        if (rotateWithTarget)
        {
            // 2. หมุน Marker ให้ "นอนลง" (X=90) และหมุนตามทิศทางตัวละคร (Y = target.y)
            // เราจะใช้ eulerAngles ของ targetEntity ในแกน Y มาเป็นตัวกำหนดทิศหัวลูกศร
            transform.rotation = Quaternion.Euler(90f, targetEntity.eulerAngles.y, 0f);
        }
        else
        {
            // กรณีเป็นไอคอนคงที่ (เช่น จุด Save) ให้หันไปทิศเหนือ (0 องศา) เสมอ
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    // ฟังก์ชันสำหรับตั้งค่า Target จากสคริปต์อื่น
    public void SetTarget(Transform target)
    {
        targetEntity = target;
    } 
}
