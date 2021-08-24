using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnsgarsAssets
{
    /// <summary>
    /// Spawns, deletes, and connects <see cref="ChainLink"/>s together to form a continuous chain. This component acts like
    /// the hole in a ship where a chain is pulled in or pushed out to drop or lift an anchor.
    /// </summary>
    [RequireComponent(typeof(Rigidbody), typeof(SpringJoint))]
    public class ChainLinkSource : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The object that the chain should be connected to.")]
        ChainLinkHook hookToConnectChainLinkTo;

        [SerializeField]
        [Tooltip("The type of object that should be used as links in the chain.")]
        VariableLengthChainLink chainLinkPrefab;

        [SerializeField]
        [Tooltip("How long each chain link is supposed to be. The effective length means how much the link adds to the length of the chain, being the distance between the link's connection points.")]
        [Range(0.5f, 2)]
        float maximumEffectiveChainLinkLength = 1;



        [Header("Uptaking and Expelling")]

        [Tooltip("Applies an outward force with the specified strength on the closest chain link. Negative values pull the chain in.")]
        [Range(-100, 100)]
        public float pushOutForceAmount;

        [Tooltip("How quickly the chain may expell, specified in units per second.")]
        [Range(0, 5)]
        public float maximumExpellSpeed;

        [Tooltip("How quickly the chain may be taken up, specified in units per second.")]
        [Range(0, 5)]
        public float maximumTakeUpSpeed;
        [Range(0, 1)]
        public float friction;

        [Header("Bookkeeping")]
        [Tooltip("When links of the chain are spawned, they are spawned as children of the specified object.")]
        public Transform chainLinkParent;

        SpringJoint joint;
        new Rigidbody rigidbody;
        VariableLengthChainLink previouslySpawnedChainLink;

        Vector3 positionAfterPreviousFixedUpdate;

        private SpringJoint GetSpringJoint()
        {
            if (joint == null)
                joint = GetComponent<SpringJoint>();
            return joint;
        }

        private Rigidbody GetRigidbody()
        {
            if (rigidbody == null)
                rigidbody = GetComponent<Rigidbody>();
            return rigidbody;
        }

        private void OnEnable()
        {
            positionAfterPreviousFixedUpdate = transform.position;
            ConnectSpringJointTohookToConnectChainLinkToIfExistent();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            // Only adjust the rope if there is something to connect it to.
            if (hookToConnectChainLinkTo)
            {
                ExtendChainIfNecessary();

                ApplyFrictionToHookToConnectChainLinkTo();

                ApplyPushOutForce();

                // Before the physics engine does calculations, the SpringJoint values should be updated to limit the movement of the hookToConnectChainLinkTo
                UpdateSpringJointValues();

                RotatePreviouslySpawnedChainLinkTowardsSourceIfPossible();
            }

            UpdatePositionAfterPreviousFixedUpdate();
        }

        void RotatePreviouslySpawnedChainLinkTowardsSourceIfPossible()
        {
            // It is only possible to rotate it if there is one
            if (previouslySpawnedChainLink)
            {
                Vector3 alignmentWith = transform.position - previouslySpawnedChainLink.GetPositionToLinkToHook();

                previouslySpawnedChainLink.GetRigidbody().MoveRotation(Quaternion.FromToRotation(Vector3.down, alignmentWith));

                // Move the chainLink to the connection point again since the previous rotation operation rotated around the origin and not the connection point
                Vector3 positionToMoveTo = previouslySpawnedChainLink.AttachedToHook.GetPositionToLinkChainLinkTo() - previouslySpawnedChainLink.GetPositionToLinkToHookOffset();

                hookToConnectChainLinkTo.transform.position = positionToMoveTo;
            }
        }

        private void ShortenChain()
        {
            /*
if (0 < maximumTakeUpSpeed)
{
    // We don't want to remove more rope than is possible to take up in that time
    float maximumRopeToRemove = maximumTakeUpSpeed * Time.deltaTime;

    float ropeLengthThatHasBeenRemoved = 0;

    // Only remove anything if it is not already the end of the rope
    while (hookToConnectChainLinkTo.GetComponent<VariableLengthChainLink>() && ropeLengthThatHasBeenRemoved < maximumRopeToRemove)
    {
        // Only remove anything if the rope is actually further pulled in. This is the case if the hookToConnectChainLinkTo
        // lies behind the ropeDeletionBorder, which means that it doesn't lie on the forward side of the chainLinkSource
        Plane ropeDeletionBorder = new Plane(transform.forward, transform.position);

        if (ropeDeletionBorder.GetSide(hookChainLinkComponent.GetPositionToLinkChainLinkTo()))
        {
            float distanceToPositionToLinkToHook = Vector3.Distance(transform.position, hookChainLinkComponent.GetPositionToLinkToHook());

            float lengthToRemoveFromRope = Mathf.Min(maximumRopeToRemove, distanceToPositionToLinkToHook);

            // While there is still rope to remove ...
            while (0 < lengthToRemoveFromRope)
            {

            }
        }
    }


}
*/
        }



        void ExtendChainIfNecessary()
        {
            // The chain should never be extended more than is theoretically allowed.
            float maximumExtensionAmount = maximumExpellSpeed * Time.fixedDeltaTime;

            // How far away the chain is from the source.
            float gapSize = Vector3.Distance(transform.position, hookToConnectChainLinkTo.GetPositionToLinkChainLinkTo());

            // How much the chain should be extended.
            float extensionAmount = Mathf.Min(maximumExtensionAmount, gapSize);

            // If the chain should be extended, then try to extend the previously spawned chainLink first.
            if (0 < extensionAmount && previouslySpawnedChainLink && previouslySpawnedChainLink.CurrentEffectiveLength < maximumEffectiveChainLinkLength)
            {
                float chainLinkGrowPotential = maximumEffectiveChainLinkLength - previouslySpawnedChainLink.CurrentEffectiveLength;
                float lengthToAdd = Mathf.Min(extensionAmount, chainLinkGrowPotential);

                previouslySpawnedChainLink.AddEffectiveLength(lengthToAdd);

                // The length we added to the chain should not be added again.
                extensionAmount -= lengthToAdd;
            }

            // If the chain should be extended even more, ...
            while (0 < extensionAmount)
            {
                //... then spawn new ChainLinks until it is extended enough.

                SpawnAndAttachChainLinkToHook();
                RotatePreviouslySpawnedChainLinkTowardsSourceIfPossible();

                float spawnedChainLinkEffectiveLength = Mathf.Min(extensionAmount, maximumEffectiveChainLinkLength);

                previouslySpawnedChainLink.SetEffectiveLength(spawnedChainLinkEffectiveLength);

                // Don't forget to take into account the rope that has just been spawned.
                extensionAmount -= spawnedChainLinkEffectiveLength;
            }
        }

        public void SetHookToConnectChainLinkTo(ChainLinkHook hook)
        {
            hookToConnectChainLinkTo = hook;
            ConnectSpringJointTohookToConnectChainLinkToIfExistent();
        }

        public void PullInRopeInstantly()
        {
            while (hookToConnectChainLinkTo.GetComponent<ChainLink>())
                ShortenRopeByOneLink();

            hookToConnectChainLinkTo.transform.position = transform.position;
        }

        public void SetChainLinkParent(Transform parent)
        {
            chainLinkParent = parent;
        }

        public void LockRopeLength()
        {
            maximumExpellSpeed = 0;
            maximumTakeUpSpeed = 0;
        }

        public void UnlockRopeLength()
        {
            maximumExpellSpeed = Mathf.Infinity;
            maximumTakeUpSpeed = Mathf.Infinity;
        }

        void SpawnAndAttachChainLinkToHook()
        {
            GameObject spawnedChainLink = Instantiate(chainLinkPrefab.gameObject, chainLinkParent);
            previouslySpawnedChainLink = spawnedChainLink.GetComponent<VariableLengthChainLink>();

            previouslySpawnedChainLink.AttachToChainLinkHook(hookToConnectChainLinkTo);

            hookToConnectChainLinkTo = spawnedChainLink.GetComponent<ChainLinkHook>();
            ConnectSpringJointTohookToConnectChainLinkToIfExistent();
        }

        void ShortenRopeByOneLink()
        {
            GameObject objectToDestroy = hookToConnectChainLinkTo.gameObject;

            ChainLink attachedChainLink = objectToDestroy.GetComponent<ChainLink>();

            if (attachedChainLink != null)
            {
                hookToConnectChainLinkTo = attachedChainLink.AttachedToHook;

                Destroy(objectToDestroy);

                ConnectSpringJointTohookToConnectChainLinkToIfExistent();
            }
        }

        void ApplyFrictionToHookToConnectChainLinkTo()
        {
            // Only apply a friction if there is any
            if (friction != 0)
            {
                // This function has a problem: it shows creeping behavour, meaning that even if the friction is 1, the hookToConnectChainLinkTo still moves slightly.
                Vector3 currentHookVelocity = hookToConnectChainLinkTo.GetRigidbody().velocity;

                hookToConnectChainLinkTo.GetRigidbody().AddForce(-currentHookVelocity * friction, ForceMode.VelocityChange);

                // Because there is a friction, the chainLinkHook that this source is connected to should also move some amount when this source is moved.
                hookToConnectChainLinkTo.GetRigidbody().AddForce(GetVelocity() * friction, ForceMode.VelocityChange);
            }
        }

        void ApplyPushOutForce()
        {
            Vector3 pushOutForceDirection = (hookToConnectChainLinkTo.transform.position - transform.position).normalized;
            hookToConnectChainLinkTo.GetRigidbody().AddForce(pushOutForceDirection * pushOutForceAmount);
            GetRigidbody().AddForce(-pushOutForceDirection * pushOutForceAmount);

        }

        void UpdateSpringJointValues()
        {
            // The distanceToHook is calculated to its origin instead of to its positionToLinkChainLinkTo because the Springjoint connects with the origin,
            // and not the positionToLinkChainLinkTo.
            float distanceToHook = (hookToConnectChainLinkTo.transform.position - transform.position).magnitude;

            GetSpringJoint().minDistance = distanceToHook - maximumTakeUpSpeed * Time.fixedDeltaTime;
            GetSpringJoint().maxDistance = distanceToHook + maximumExpellSpeed * Time.fixedDeltaTime;
        }

        void ConnectSpringJointTohookToConnectChainLinkToIfExistent()
        {
            if (hookToConnectChainLinkTo)
                GetSpringJoint().connectedBody = hookToConnectChainLinkTo.GetRigidbody();
        }

        Vector3 GetMovementSincePreviousFixedUpdate() => transform.position - positionAfterPreviousFixedUpdate;

        void UpdatePositionAfterPreviousFixedUpdate() => positionAfterPreviousFixedUpdate = transform.position;

        Vector3 GetVelocity() => GetMovementSincePreviousFixedUpdate() / Time.fixedDeltaTime;

        float CalculateCurrentPushOutSpeed()
        {
            float pushOutSpeed;

            Vector3 hookDirection = (hookToConnectChainLinkTo.transform.position - transform.position).normalized;

            pushOutSpeed = Vector3Utility.GetProjectionFactor(hookDirection, hookToConnectChainLinkTo.GetRigidbody().velocity - GetVelocity());

            return pushOutSpeed;
        }

        private void OnValidate()
        {
            ValidateSpringJointValues();
        }

        void ValidateSpringJointValues()
        {
            // The spring value of a SpringJoint determines how strong it is. If the spring is not strong enough, the length of the rope cannot be locked securely.
            if (GetSpringJoint().spring < float.MaxValue * 0.99) // I use a factor of 99% here because else the if clause would still be entered even if the spring value was equal to "float.MaxValue"
                Debug.LogWarning("The SpringJoint of a ChainLinkSource has a spring value that is lower that the maximum possible value. This can lead to unexpected behaviour for the maximumPushOutSpeed and maximumPullInSpeed variables.");

            if (GetSpringJoint().damper != 0)
                Debug.LogWarning("A SpringJoint used by a ChainLinkSource has unexpected values.");

            if (0.00000000000000001 < GetSpringJoint().tolerance)
                Debug.LogWarning("A SpringJoint used by a ChainLinkSource has unexpected values.");

            if (GetSpringJoint().autoConfigureConnectedAnchor != false)
                Debug.LogWarning("A SpringJoint used by a ChainLinkSource has unexpected values.");

            if (GetSpringJoint().anchor != Vector3.zero)
                Debug.LogWarning("A SpringJoint used by a ChainLinkSource has unexpected values.");

            if (GetSpringJoint().connectedAnchor != Vector3.zero)
                Debug.LogWarning("A SpringJoint used by a ChainLinkSource has unexpected values.");
        }

        private void Reset()
        {
            ConfigureSpringJointValues();
        }

        void ConfigureSpringJointValues()
        {
            GetSpringJoint().spring = float.MaxValue;
            GetSpringJoint().damper = 0;
            GetSpringJoint().tolerance = 0;
            GetSpringJoint().autoConfigureConnectedAnchor = false;
            GetSpringJoint().anchor = Vector3.zero;
            GetSpringJoint().connectedAnchor = Vector3.zero;
        }
    }
}

