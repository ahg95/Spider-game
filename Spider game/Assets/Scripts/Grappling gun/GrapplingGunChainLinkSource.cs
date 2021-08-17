using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace AnsgarsAssets
{
    /// <summary>
    /// A ChainLinkSource with extra components that can be used by the grapplingGun.
    /// The <see cref="ChainLinkSource"/> is needed for the rope functionality, the <see cref="DeactivatableFixedJoint"/> is needed to attach the <see cref="ChainLinkSource"/> to a surface, and the <see cref="ParentConstraint"/> is needed to keep the <see cref="ChainLinkSource"/> at the muzzle of the <see cref="GrapplingGun"/>.
    /// </summary>
    [RequireComponent(typeof(ChainLinkSource), typeof(DeactivatableFixedJoint), typeof(ParentConstraint))]
    public class GrapplingGunChainLinkSource : MonoBehaviour
    {
        ChainLinkSource chainLinkSource;

        DeactivatableFixedJoint deactivatableFixedJoint;

        ParentConstraint parentConstraint;

        public ChainLinkSource GetChainLinkSource()
        {
            if (!chainLinkSource)
                chainLinkSource = GetComponent<ChainLinkSource>();

            return chainLinkSource;
        }

        public DeactivatableFixedJoint GetDeactivatableFixedJoint()
        {
            if (!deactivatableFixedJoint)
                deactivatableFixedJoint = GetComponent<DeactivatableFixedJoint>();

            return deactivatableFixedJoint;
        }

        public ParentConstraint GetParentConstraint()
        {
            if (!parentConstraint)
                parentConstraint = GetComponent<ParentConstraint>();

            return parentConstraint;
        }
    }
}