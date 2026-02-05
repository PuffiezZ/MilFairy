using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.BehaviorDesigner.Runtime.Tasks.Actions;
using Opsive.GraphDesigner.Runtime.Variables;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions.Must;

public class SetPayloadTatget : Action
{
    public SharedVariable<GameObject> payloadPOS;
    public override void OnStart()
    {
        GameObject foundObject = GameObject.FindGameObjectWithTag("Payload");
        ////BoxCollider boxCollider = foundObject.GetComponent<BoxCollider>();
        ////Vector3 getPOS = boxCollider.ClosestPoint(transform.position);
        if (foundObject != null)
        {
            payloadPOS.Value = foundObject;
            Debug.Log($"<color=cyan>[AI]</color> Successfully locked on: {foundObject.name}");
        }
        else
        {
            Debug.LogError("<color=red>[AI]</color> Cannot find object with Tag: Payload!");
        }
    }
    public override TaskStatus OnUpdate()
    {
        if(payloadPOS.Value != null)
        {
            Debug.Log("Payload Target Set to: " + payloadPOS.Value);
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }
}
