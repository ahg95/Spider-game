using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Joint))]
public class ChainLink : ChainLinkHook
{
    private Joint joint;

    [SerializeField]
    Transform PositionToLinkToHook;

    public Joint GetJoint()
    {
        if (joint == null)
            joint = GetComponent<Joint>();
        return joint;
    }

    public Vector3 GetPositionToLinkToHook() => PositionToLinkToHook.position;

    public Vector3 GetPositionToLinkToHookOffset() => PositionToLinkToHook.position - transform.position;

    public Vector3 GetLinkingPositionToHookPositionOffset() => GetPositionToLinkChainLinkTo() - GetPositionToLinkToHook();

    public void AttachToChainLinkHook(ChainLinkHook hookToAttachTo)
    {
        transform.rotation = hookToAttachTo.transform.rotation;

        transform.position = hookToAttachTo.GetPositionToLinkChainLinkTo() - GetPositionToLinkToHookOffset();

        GetRigidbody().velocity = hookToAttachTo.GetRigidbody().velocity;

        GetJoint().connectedBody = hookToAttachTo.GetRigidbody();
    }

    public void AttachToChainLinkHook(ChainLinkHook hookToAttachTo, Vector3 positionToRotateChainLinkTowards)
    {
        Vector3 targetDirection = positionToRotateChainLinkTowards - transform.position;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, targetDirection); // TODO: Generalize from Vector3.up to any configuration of ChainLink

        transform.position = hookToAttachTo.GetPositionToLinkChainLinkTo() - GetPositionToLinkToHookOffset();

        GetRigidbody().velocity = hookToAttachTo.GetRigidbody().velocity;

        GetJoint().connectedBody = hookToAttachTo.GetRigidbody();
    }
}
