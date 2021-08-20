using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnsgarsAssets
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class VariableLengthChainLink : ChainLink
    {
        public float EffectiveLength { get; private set; }

        CapsuleCollider capsuleCollider;

        CapsuleCollider GetCapsuleCollider()
        {
            if (!capsuleCollider)
                capsuleCollider = GetComponent<CapsuleCollider>();

            return capsuleCollider;
        }


        /// <summary>
        /// Sets the effective length of the <see cref="VariableLengthChainLink"/>. The effective length is the distance between the connection points of
        /// the <see cref="ChainLink"/>.
        /// </summary>
        /// <param name="length"></param>
        public void SetEffectiveLength(float length)
        {
            // The effectiveLength is useful to know for other scripts, so make it accessible for them:
            EffectiveLength = length;

            // The effective length is the distance between the connection points of the ChainLink, and therefore the radius has to be
            // added twice to calculate the correct height.
            GetCapsuleCollider().height = length + GetCapsuleCollider().radius * 2;

            // Since the collider is scaled, we should adjust the positions of the connection points.
            PositionToLinkToHook.position = transform.position + transform.up * length / 2;
            PositionToLinkChainLinkTo.position = transform.position - transform.up * length / 2;

            // We might as well move the chainLink manually instead of letting the physics engine do the work. If "Auto Configure Connected Anchor"
            // is enabled for the ConfigurableJoint, then doing this step before setting the anchor is important because the connected anchor
            // will have the same global position as the anchor after setting it.
            transform.position = AttachedToHook.GetPositionToLinkChainLinkTo() - GetPositionToLinkToHookOffset();

            // Lastly, we have to move the anchor position of the joint to the new positionToLinkToHook so that the physics engine
            // connects the correct position.
            GetJoint().anchor = PositionToLinkToHook.localPosition;
        }
    }
}