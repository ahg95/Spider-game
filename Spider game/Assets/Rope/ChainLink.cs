using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Joint))]
public class ChainLink : ChainLinkHook
{
    private Joint joint;

    public ChainLinkHook AttachedToHook { get; private set; }

    public Transform PositionToLinkToHook;

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

        AttachedToHook = hookToAttachTo;
    }

    public void AttachToChainLinkHookAndRotateTowards(ChainLinkHook hookToAttachTo, Vector3 positionToRotateChainLinkTowards)
    {
        AttachToChainLinkHook(hookToAttachTo);

        Vector3 targetDirection = positionToRotateChainLinkTowards - (hookToAttachTo.transform.position + hookToAttachTo.GetPositionToLinkChainLinkToOffset());
        transform.rotation = Quaternion.FromToRotation(Vector3.down, targetDirection);

        transform.position = hookToAttachTo.GetPositionToLinkChainLinkTo() - GetPositionToLinkToHookOffset();
    }
}
