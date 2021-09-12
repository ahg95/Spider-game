using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsWeaponHolder : MonoBehaviour
{
    public ConfigurableJoint joint;

    [Header("Configuration for holding weapon lightly")]
    public float lightPositionSpring;
    public float lightPositionDamper;

    [Header("Configuration for holding weapon strongly")]
    public float strongPositionSpring;
    public float strongPositionDamper;

    JointDrive light;
    JointDrive strong;

    void Awake()
    {
        light = new JointDrive();
        light.positionSpring = lightPositionSpring;
        light.positionDamper = lightPositionDamper;
        light.maximumForce = Mathf.Infinity;

        strong = new JointDrive();
        strong.positionSpring = strongPositionSpring;
        strong.positionDamper = strongPositionDamper;
        strong.maximumForce = Mathf.Infinity;
    }

    public void AimForwardsInstantly()
    {
        joint.transform.rotation = joint.connectedBody.transform.rotation;

        Vector3 anchorWorldPosition = joint.transform.TransformPoint(joint.anchor);
        Vector3 connectedAnchorWorldPosition = joint.connectedBody.transform.TransformPoint(joint.connectedAnchor);

        Vector3 targetOffset = anchorWorldPosition - connectedAnchorWorldPosition;

        joint.transform.position += targetOffset;
    }

    public void HoldWeaponLightly() => joint.slerpDrive = light;

    public void HoldWeaponStrongly() => joint.slerpDrive = strong;
}
