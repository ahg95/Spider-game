using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ChainLinkHook : MonoBehaviour
{
    protected new UnityEngine.Rigidbody rigidbody;

    public Transform PositionToLinkChainLinkTo;

    private void Enable()
    {
        if (PositionToLinkChainLinkTo == null)
            PositionToLinkChainLinkTo = transform;
    }

    public UnityEngine.Rigidbody GetRigidbody()
    {
        if (rigidbody == null)
            rigidbody = GetComponent<UnityEngine.Rigidbody>();
        return rigidbody;
    }

    public Vector3 GetPositionToLinkChainLinkTo() => PositionToLinkChainLinkTo.position;

    public Vector3 GetPositionToLinkChainLinkToOffset() => PositionToLinkChainLinkTo.position - transform.position;

}
