using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ChainLinkHook : MonoBehaviour
{
    private new Rigidbody rigidbody;

    [SerializeField]
    Transform PositionToLinkChainLinkTo;

    public Rigidbody GetRigidbody()
    {
        if (rigidbody == null)
            rigidbody = GetComponent<Rigidbody>();
        return rigidbody;
    }

    public Vector3 GetPositionToLinkChainLinkTo() => PositionToLinkChainLinkTo.position;

    public Vector3 GetPositionToLinkChainLinkToOffset() => PositionToLinkChainLinkTo.position - transform.position;

}
