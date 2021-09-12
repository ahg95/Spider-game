using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnsgarsAssets
{
    [RequireComponent(typeof(Joint))]
    public class ChainLink : ChainLinkHook
    {
        private Joint joint;

        public ChainLinkHook AttachedToHook { get; private set; }

        public Transform PositionToLinkToHook;

        public ChainLinkHook InitialHookToConnectTo;

        public void Start()
        {
            if (InitialHookToConnectTo)
                AttachToChainLinkHook(InitialHookToConnectTo);
        }

        public Joint GetJoint()
        {
            if (joint == null)
                joint = GetComponent<Joint>();
            return joint;
        }

        public Vector3 GetPositionToLinkToHookLocal()
        {
            return transform.InverseTransformPoint(GetPositionToLinkToHook());
        }

        public Vector3 GetPositionToLinkToHook() => PositionToLinkToHook.position;

        public Vector3 GetPositionToLinkToHookOffset() => PositionToLinkToHook.position - transform.position;

        /// <summary>
        /// Returns a vector in world space pointing from the hook position to the position to link to hook.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetLinkingPositionToHookPositionOffset() => GetPositionToLinkChainLinkTo() - GetPositionToLinkToHook();

        /// <summary>
        /// Moves the <see cref="ChainLink"/> to the specified hook, copies its rotation, and connects the joint to the hook.
        /// </summary>
        /// <param name="hookToAttachTo">The <see cref="ChainLinkHook"/> that this <see cref="ChainLink"/> should be attached to.</param>
        public void AttachToChainLinkHook(ChainLinkHook hookToAttachTo)
        {
            AttachedToHook = hookToAttachTo;

            // The Configurable joint that is attached to this ChainLink also limits rotation. For this to work correctly, the rotation of this
            // ChainLink has to be the same as the rotation of the hookToAttachTo.
            transform.rotation = hookToAttachTo.transform.rotation;

            // Since the attached ConfigurableJoint has autoConfigureAnchor activated, the position has to be set before setting the connectedBody
            // in order for the connectedAnchor to be in the correct position.
            transform.position = hookToAttachTo.GetPositionToLinkChainLinkTo() - GetPositionToLinkToHookOffset();

            // Finally, we connect the ConfigurableJoint.
            GetJoint().connectedBody = hookToAttachTo.GetRigidbody();
        }

        public void OrientHookPositionTowards(Vector3 position)
        {
            Vector3 initialPositionToLinkToHook = GetPositionToLinkToHook();

            Vector3 targetDirection = position - initialPositionToLinkToHook;
            transform.rotation = Quaternion.FromToRotation(Vector3.down, targetDirection);

            // Since the rotation happened around the ChainLink's origin, the position has to be updated.
            transform.position = initialPositionToLinkToHook - GetPositionToLinkToHookOffset();
        }

        public void AlignHookPositionWith(Vector3 position)
        {
            Vector3 directionOfPosition = position - GetPositionToLinkToHook();

            if (Vector3.Angle(directionOfPosition, -transform.up) < 90)
                OrientHookPositionTowards(position);
            else
                OrientHookPositionTowards(GetPositionToLinkToHook() - directionOfPosition);
        }

        public void CopyVelocityOfAttachedToHook()
        {
            if (AttachedToHook)
            {
                GetRigidbody().velocity = AttachedToHook.GetRigidbody().velocity;
            }
        }

        /// <summary>
        /// Applies the velocity of the hook this <see cref="ChainLink"/> is attached to to the position where it is attached.
        /// </summary>
        public void ApplyForcesOfAttachedToHook()
        {
            if (AttachedToHook)
            {
                Vector3 force = AttachedToHook.GetRigidbody().velocity;
                GetRigidbody().AddForceAtPosition(force, GetPositionToLinkToHook(), ForceMode.VelocityChange);
            }

        }

        public bool PositionToLinkChainLinkToPointsTowards(Vector3 point)
        {
            Plane plane = new Plane(-transform.up, GetPositionToLinkChainLinkTo());

            return plane.GetSide(point);
        }

        [Obsolete("Method AttachToChainLinkHookAndRotateTowards is deprecated, please use the method OrientPositionToHookChainLinkToTowards instead.")]
        public void AttachToChainLinkHookAndRotateTowards(ChainLinkHook hookToAttachTo, Vector3 positionToRotateChainLinkTowards)
        {
            AttachToChainLinkHook(hookToAttachTo);

            Vector3 targetDirection = positionToRotateChainLinkTowards - (hookToAttachTo.transform.position + hookToAttachTo.GetPositionToLinkChainLinkToOffset());
            transform.rotation = Quaternion.FromToRotation(Vector3.down, targetDirection);

            transform.position = hookToAttachTo.GetPositionToLinkChainLinkTo() - GetPositionToLinkToHookOffset();
        }
    }
}