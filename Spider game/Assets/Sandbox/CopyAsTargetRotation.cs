using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyAsTargetRotation : MonoBehaviour
{
    public Transform targetRotation;

    public ConfigurableJoint jointToSetRotationFor;

    private void FixedUpdate()
    {
        jointToSetRotationFor.targetRotation = targetRotation.rotation;
    }
}
