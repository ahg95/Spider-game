using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnsgarsAssets
{
    [RequireComponent(typeof(Rigidbody))]
    public class ChainLinkHook : MonoBehaviour
    {
        protected new Rigidbody rigidbody;

        public Transform PositionToLinkChainLinkTo;

        private void OnEnable()
        {
            if (PositionToLinkChainLinkTo == null)
                PositionToLinkChainLinkTo = transform;
        }

        public Rigidbody GetRigidbody()
        {
            if (rigidbody == null)
                rigidbody = GetComponent<Rigidbody>();
            return rigidbody;
        }

        public Vector3 GetPositionToLinkChainLinkTo() => PositionToLinkChainLinkTo.position;

        public Vector3 GetPositionToLinkChainLinkToOffset() => PositionToLinkChainLinkTo.position - transform.position;
    }
}