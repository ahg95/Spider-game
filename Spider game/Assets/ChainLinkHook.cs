using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ChainLinkHook : MonoBehaviour
{
    public Rigidbody Rigidbody {
        get {
            if (Rigidbody == null)
                Rigidbody = GetComponent<Rigidbody>();
            return Rigidbody;
        }
        private set
        {
            Rigidbody = value;
        }
    }

    [SerializeField]
    Transform PositionToLinkChainLinkTo;

    public Vector3 GetPositionToLinkChainLinkTo() => PositionToLinkChainLinkTo.position;

    public Vector3 GetPositionToLinkChainLinkToOffset() => PositionToLinkChainLinkTo.position - transform.position;

}
