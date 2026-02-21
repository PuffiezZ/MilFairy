using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class SyncJoint : MonoBehaviour
{
    [SerializeField] private Rigidbody boneRBtarget;
    private ConfigurableJoint joint;
    private Quaternion startLocalRotaiton;

    public bool enableSync = false;

    private void Awake()
    {
        joint = GetComponent<ConfigurableJoint>();

        startLocalRotaiton = transform.localRotation;
    }

    public void UpdateJointFromAnimation()
    {
        if (!enableSync) return;

        ConfigurableJointExtensions.SetTargetRotationLocal(joint, boneRBtarget.transform.localRotation, startLocalRotaiton);
    }
}
