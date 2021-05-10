using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Joint))]
public class ChainLink : ChainLinkHook
{
    public Joint Joint {
        get {
            if (Joint == null)
                Joint = GetComponent<Joint>();
            return Joint;
        }
        private set
        {
            Joint = value;
        }
    }

    [SerializeField]
    Transform PositionToLinkToHook;

    public Vector3 GetPositionToLinkToHook() => PositionToLinkToHook.position;

    public Vector3 GetPositionToLinkToHookOffset() => PositionToLinkToHook.position - transform.position;

    public Vector3 GetLinkingPositionToHookPositionOffset() => GetPositionToLinkChainLinkTo() - GetPositionToLinkToHook();

    public void AttachToChainLinkHook(ChainLinkHook hookToAttachTo)
    {
        transform.rotation = hookToAttachTo.transform.rotation;

        transform.position = hookToAttachTo.GetPositionToLinkChainLinkTo() - GetPositionToLinkToHookOffset();

        Rigidbody.velocity = hookToAttachTo.Rigidbody.velocity;

        Joint.connectedBody = hookToAttachTo.Rigidbody;
    }

    public void AttachToChainLinkHook(ChainLinkHook hookToAttachTo, Vector3 positionToRotateChainLinkTowards)
    {
        Vector3 targetDirection = positionToRotateChainLinkTowards - transform.position;
        transform.rotation = transform.rotation * Quaternion.FromToRotation(GetLinkingPositionToHookPositionOffset(), targetDirection);

        transform.position = hookToAttachTo.GetPositionToLinkChainLinkTo() - GetPositionToLinkToHookOffset();

        Rigidbody.velocity = hookToAttachTo.Rigidbody.velocity;

        Joint.connectedBody = hookToAttachTo.Rigidbody;
    }
}
