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
        VariableLengthChainLink firstChainLink;

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
            ConnectSpringJointTohookToConnectChainLinkTo();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            // Only adjust the rope if there is something to connect it to.
            if (hookToConnectChainLinkTo)
            {
                if (ChainShouldBeLengthened())
                {
                    float lengthToAddToChain = CalculateLengthToAddToChain();

                    LengthenChainBy(lengthToAddToChain);
                }
                else if (ChainShouldBeShortened())
                {
                    float lengthToRemoveFromChain = CalculateLengthToRemoveFromChain();

                    ShortenChainBy(lengthToRemoveFromChain);
                }

                ApplyPushOutForce();

                ApplyFrictionToHookToConnectChainLinkTo();



                // Before the physics engine does calculations, the SpringJoint values should be updated to limit the movement of the hookToConnectChainLinkTo
                UpdateSpringJointValues();

                RotatePreviouslySpawnedChainLinkTowardsSourceIfPossible();
            }

            UpdatePositionAfterPreviousFixedUpdate();
        }

        void ShortenChainBy(float amount)
        {
            while (0 < amount)
            {
                if (firstChainLink.CurrentEffectiveLength < amount)
                {
                    RemoveFirstChainLink();
                }
            }

        }

        void RemoveFirstChainLink()
        {
            GameObject toBeDestroyed = firstChainLink.gameObject;

            firstChainLink = firstChainLink.AttachedToHook.GetComponent<VariableLengthChainLink>();

            Destroy(toBeDestroyed);
        }

        bool ChainShouldBeLengthened()
        {
            bool chainShouldBeLengthened = false;

            if (0 < maximumExpellSpeed && hookToConnectChainLinkTo)
            {
                // Make sure you mean the forward direction!
                Plane frontPlane = new Plane(transform.forward, transform.position);

                // If the hookToConnectChainLinkTo lies in front of the source, then the chain should be lengthened.
                chainShouldBeLengthened = frontPlane.GetSide(hookToConnectChainLinkTo.GetPositionToLinkChainLinkTo());
            }

            return chainShouldBeLengthened;
        }

        bool ChainShouldBeShortened()
        {
            bool chainShouldBeShortened = false;

            // If there was no chainLink in the chain but just the hook then there would be nothing to shorten
            if (0 < maximumTakeUpSpeed && firstChainLink)
            {
                Plane frontPlane = new Plane(transform.forward, transform.position);

                // The chain should only be shortened if the hook lies behind the source.
                chainShouldBeShortened = !frontPlane.GetSide(hookToConnectChainLinkTo.GetPositionToLinkChainLinkTo());
            }

            return chainShouldBeShortened;
        }

        float CalculateLengthToAddToChain()
        {
            float maximumLengthToAdd = Time.fixedDeltaTime * maximumExpellSpeed;
            float gapSize = Vector3.Distance(hookToConnectChainLinkTo.transform.position, transform.position);

            return Mathf.Min(maximumLengthToAdd, gapSize);
        }

        float CalculateLengthToRemoveFromChain()
        {
            float maximumLengthToRemove = Time.fixedDeltaTime * maximumTakeUpSpeed;
            float lengthInside = Vector3.Distance(hookToConnectChainLinkTo.transform.position, transform.position);

            return Mathf.Min(maximumLengthToRemove, lengthInside);
        }

        void LengthenChainBy(float amount)
        {
            amount = TryLengtheningFirstChainLinkBy(amount);

            while (0 < amount)
            {
                // Add a ChainLink with the appropriate length
                AddChainLink();
                firstChainLink.SetEffectiveLength(0);
                amount = TryLengtheningFirstChainLinkBy(amount);

                // Orient it towards the source since a new CHainLink must have come from the source
                firstChainLink.OrientHookPositionTowards(transform.position);

                // And copy the velocity. You can play around with this one and see what gives the best results.
                firstChainLink.ApplyForcesOfAttachedToHook();
                //firstChainLink.CopyVelocityOfAttachedToHook();
            }
        }


        /// <summary>
        /// Attempts to lengthen the first chainLink by the specified amount under the constraint of the maximumEffectiveChainLinkLength.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>The amount by which the firstChainLink could not be lengthened</returns>
        float TryLengtheningFirstChainLinkBy(float amount)
        {
            if (firstChainLink)
            {
                float effectiveLengthToAdd = Mathf.Min(maximumEffectiveChainLinkLength, amount);

                firstChainLink.AddEffectiveLength(effectiveLengthToAdd);

                amount -= effectiveLengthToAdd;
            }

            return amount;
        }

        void RotatePreviouslySpawnedChainLinkTowardsSourceIfPossible()
        {
            // It is only possible to rotate it if there is one
            if (firstChainLink)
            {
                Vector3 alignmentWith = transform.position - firstChainLink.GetPositionToLinkToHook();

                firstChainLink.GetRigidbody().MoveRotation(Quaternion.FromToRotation(Vector3.down, alignmentWith));

                // Move the chainLink to the connection point again since the previous rotation operation rotated around the origin and not the connection point
                Vector3 positionToMoveTo = firstChainLink.AttachedToHook.GetPositionToLinkChainLinkTo() - firstChainLink.GetPositionToLinkToHookOffset();

                hookToConnectChainLinkTo.transform.position = positionToMoveTo;
            }
        }

        public void SetHookToConnectChainLinkTo(ChainLinkHook hook)
        {
            hookToConnectChainLinkTo = hook;
            ConnectSpringJointTohookToConnectChainLinkTo();
        }

        public void DestroyChainAndResetHookPosition()
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

        void AddChainLink()
        {
            // Spawn a new ChainLink
            GameObject spawnedChainLink = Instantiate(chainLinkPrefab.gameObject, chainLinkParent);

            // Retrieve the component
            firstChainLink = spawnedChainLink.GetComponent<VariableLengthChainLink>();

            // Connect the spawned ChainLink to the chain.
            firstChainLink.AttachToChainLinkHook(hookToConnectChainLinkTo);

            // New ChainLinks should now connect to the just spawned ChainLink.
            hookToConnectChainLinkTo = spawnedChainLink.GetComponent<ChainLinkHook>();

            // Now the joints of this source have to be connected to the newly spawned ChainLink
            ConnectSpringJointTohookToConnectChainLinkTo();
        }

        void ConnectSpringJointTohookToConnectChainLinkTo()
        {
            if (hookToConnectChainLinkTo)
            {
                GetSpringJoint().connectedBody = hookToConnectChainLinkTo.GetRigidbody();
            }
        }

        void ShortenRopeByOneLink()
        {
            GameObject objectToDestroy = hookToConnectChainLinkTo.gameObject;

            ChainLink attachedChainLink = objectToDestroy.GetComponent<ChainLink>();

            if (attachedChainLink != null)
            {
                hookToConnectChainLinkTo = attachedChainLink.AttachedToHook;

                Destroy(objectToDestroy);

                ConnectSpringJointTohookToConnectChainLinkTo();
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

