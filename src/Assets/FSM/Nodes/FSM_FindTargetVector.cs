using System;
using UnityEngine;
using NodeCanvas.Framework; // เพื่อใช้ BBParameter
using UnityEngine.AI;
using System.ComponentModel;

[Category("Movement")]
[Description("ดึงตำแหน่ง Vector3 จาก GameObject เป้าหมายลงใน Blackboard")]
public class FSM_FindTargetVector : ActionTask
{
    // ตัวแปรรับค่า GameObject เป้าหมาย
    public BBParameter<GameObject> targetObject;

    // ตัวแปรสำหรับส่งค่า Vector3 กลับไปเก็บที่ Blackboard
    public BBParameter<Vector3> saveToVector;


    protected override void OnUpdate()
    {
        UpdateVector();
    }
    private void UpdateVector()
    {
        // ตรวจสอบว่ามี GameObject ในตัวแปรหรือไม่
        if (targetObject.value != null)
        {
            // ตั้งค่าตำแหน่งปัจจุบันของวัตถุลงใน BBParameter
            saveToVector.value = targetObject.value.transform.position;
            EndAction();
        }
        else
        {
            Debug.LogWarning("Target Object is Null!");
            EndAction(); 
        }
    }
}
