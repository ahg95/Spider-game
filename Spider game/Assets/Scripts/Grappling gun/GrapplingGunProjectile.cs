using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace AnsgarsAssets
{
    /// <summary>
    /// A simple class that requires the components necessary for a gameObject to be a GrappleGunProjectile and allows easy access to these components.
    /// </summary>
    [RequireComponent(typeof(ChainLinkHook), typeof(Sticky))]
    public class GrapplingGunProjectile : MonoBehaviour
    {
        ChainLinkHook chainLinkHook;

        Sticky sticky;

        ParentConstraint parentConstraint;

        new Rigidbody rigidbody;

        public ChainLinkHook GetChainLinkHook()
        {
            if (!chainLinkHook)
                chainLinkHook = GetComponent<ChainLinkHook>();

            return chainLinkHook;
        }

        public Sticky GetSticky()
        {
            if (!sticky)
                sticky = GetComponent<Sticky>();

            return sticky;
        }

        public ParentConstraint GetParentConstraint()
        {
            if (!parentConstraint)
                parentConstraint = GetComponent<ParentConstraint>();

            return parentConstraint;
        }

        public Rigidbody GetRigidbody()
        {
            if (!rigidbody)
                rigidbody = GetComponent<Rigidbody>();

            return rigidbody;
        }
    }
}

