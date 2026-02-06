using NodeCanvas.Framework; // เพื่อใช้ BBParameter
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class FSM_Delay : ActionTask
{
    public BBParameter<float> duration;
    protected override void OnExecute()
    {
        StartCoroutine(DelayCorutine(duration.value));
    }

    private IEnumerator DelayCorutine(float durationFloat)
    {
        yield return new WaitForSeconds(durationFloat);
        EndAction();
    }
}
