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
        float distanceToConnectedAnchorAfterPreviousFixedUpdate;

        private SpringJoint GetSpringJoint()
        {
            if (joint == null)
                joint = GetComponent<SpringJoint>();
            return joint;
        }

        public Rigidbody GetRigidbody()
        {
            if (rigidbody == null)
                rigidbody = GetComponent<Rigidbody>();
            return rigidbody;
        }

        private void OnEnable()
        {
            positionAfterPreviousFixedUpdate = transform.position;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            // Only adjust the rope if there is something to connect it to.
            if (hookToConnectChainLinkTo)
            {
                if (0 < maximumTakeUpSpeed || 0 < maximumExpellSpeed)
                {
                    if (firstChainLink)
                        firstChainLink.OrientHookPositionTowards(transform.position);

                    // Cheating here a bit...
                    if (firstChainLink && firstChainLink.CurrentEffectiveLength < 0.005)
                        RemoveFirstChainLink();

                    if (ChainShouldBeLengthened())
                    {
                        float lengthToAddToChain = CalculateLengthToAddToChain();

                        AddLengthToChain(lengthToAddToChain);
                    }
                    else if (ChainShouldBeShortened())
                    {
                        float lengthToRemoveFromChain = CalculateAmountToShortenChain();

                        ShortenChainBy(lengthToRemoveFromChain);
                    }
                }

                ApplyPushOutForce();
                ApplyFrictionToHookToConnectChainLinkTo();

                AdjustSpringJoint();
            }

            UpdatePositionAfterPreviousFixedUpdate();
            UpdateDistanceToConnectedAnchorAfterPreviousFixedUpdate();
        }

        bool ChainShouldBeLengthened()
        {
            return 0 < maximumExpellSpeed && hookToConnectChainLinkTo && (FirstChainLinkPointsTowardsSource() || (!firstChainLink && HookToConnectChainLinkToIsFurtherAwayThan(0)));
        }

        float CalculateLengthToAddToChain()
        {
            // Do not add more length to the chain than is theoretically possible under the current maximumExpellSpeed and the passed time.
            float maximumLengthToAdd = Time.fixedDeltaTime * maximumExpellSpeed;
            float gapSize = Vector3.Distance(hookToConnectChainLinkTo.transform.position, transform.position);

            return Mathf.Min(maximumLengthToAdd, gapSize);
        }

        void AddLengthToChain(float amount)
        {
            amount = TryLengtheningFirstChainLinkBy(amount);

            firstChainLink?.OrientHookPositionTowards(transform.position);

            while (0 < amount)
            {
                // Add a ChainLink with the appropriate length
                AddChainLink();
                firstChainLink.SetEffectiveLength(0);
                amount = TryLengtheningFirstChainLinkBy(amount);

                // Orient it towards the source since a new ChainLink must have come from the source
                firstChainLink.OrientHookPositionTowards(transform.position);

                // And copy the velocity. You can play around with this one and see what gives the best results.
                //firstChainLink.ApplyForcesOfAttachedToHook();
                firstChainLink.CopyVelocityOfAttachedToHook();
            }
        }

        bool ChainShouldBeShortened()
        {
            return 0 < maximumTakeUpSpeed && firstChainLink && !FirstChainLinkPointsTowardsSource();
        }

        float CalculateAmountToShortenChain()
        {
            // Likewise to calculating the length to add to the chain, the chain should not be shortened more than what is possible under the
            // current maximumTakeUpSpeed and the passed time.
            float maximumLengthToRemove = Time.fixedDeltaTime * maximumTakeUpSpeed;
            float lengthInside = Vector3.Distance(hookToConnectChainLinkTo.transform.position, transform.position);

            return Mathf.Min(maximumLengthToRemove, lengthInside);
        }

        void ShortenChainBy(float amount)
        {
            while (0 < amount && firstChainLink)
            {
                if (firstChainLink.CurrentEffectiveLength <= amount)
                {
                    amount -= firstChainLink.CurrentEffectiveLength;
                    RemoveFirstChainLink();
                }
                else {
                    firstChainLink.SubstractEffectiveLength(amount);
                    // There is no amount to remove anymore and therefore exit the while loop
                    break;
                }
            }
        }

        void ApplyPushOutForce()
        {
            if (hookToConnectChainLinkTo) {

                Vector3 pushOutForceDirection;

                if (firstChainLink)
                    pushOutForceDirection = firstChainLink.transform.up;
                else
                    pushOutForceDirection = transform.forward;

                hookToConnectChainLinkTo.GetRigidbody().AddForce(pushOutForceDirection * pushOutForceAmount);
                
                // There is a counter impulse in the opposite direction.
                // COMMENTED OUT FOR DEBUGGING
                //GetRigidbody().AddForce(-pushOutForceDirection * pushOutForceAmount);
            }
        }

        void ApplyFrictionToHookToConnectChainLinkTo()
        {
            // Only apply a friction if there is any
            if (hookToConnectChainLinkTo && friction != 0)
            {
                // This function has a problem: it shows creeping behavour, meaning that even if the friction is 1, the hookToConnectChainLinkTo still moves slightly.
                Vector3 currentHookVelocity = hookToConnectChainLinkTo.GetRigidbody().velocity;

                hookToConnectChainLinkTo.GetRigidbody().AddForce(-currentHookVelocity * friction, ForceMode.VelocityChange);

                // Because there is a friction, the chainLinkHook that this source is connected to should also move some amount when this source is moved.
                hookToConnectChainLinkTo.GetRigidbody().AddForce(GetVelocity() * friction, ForceMode.VelocityChange);
            }
        }

        void AdjustSpringJoint()
        {
            float chainLengthAtWhichToPositionAnchor = maximumTakeUpSpeed * Time.fixedDeltaTime;

            ConnectSpringJointToChainAtLength(chainLengthAtWhichToPositionAnchor);
            UpdateSpringJointDistanceValuesBasedOnAnchorPositionInChain(chainLengthAtWhichToPositionAnchor);
        }

        void UpdatePositionAfterPreviousFixedUpdate() => positionAfterPreviousFixedUpdate = transform.position;

        void UpdateDistanceToConnectedAnchorAfterPreviousFixedUpdate() => distanceToConnectedAnchorAfterPreviousFixedUpdate = CalculateDistanceToConnectedAnchor();


        float CalculateDistanceToConnectedAnchor()
        {
            float distanceToConnectedAnchor = 0;

            if (GetSpringJoint().connectedBody)
            {
                Vector3 positionOfConnectedAnchor = GetSpringJoint().connectedBody.transform.TransformPoint(GetSpringJoint().connectedAnchor);

                distanceToConnectedAnchor = Vector3.Distance(transform.position, positionOfConnectedAnchor);
            }

            return distanceToConnectedAnchor;
        }

        // <<<<< ABSTRACTTION LEVEL DOWN >>>>>

        bool FirstChainLinkPointsTowardsSource()
        {
            bool firstChainLinkPointsTowardsSource = false;

            if (firstChainLink)
            {
                    Plane plane = new Plane(-firstChainLink.transform.up, firstChainLink.GetPositionToLinkChainLinkTo());

                    firstChainLinkPointsTowardsSource = plane.GetSide(transform.position);
            }

            return firstChainLinkPointsTowardsSource;
        }

        bool HookToConnectChainLinkToIsFurtherAwayThan(float distance)
        {
            return hookToConnectChainLinkTo && Utility.PointsAreFurtherApartThanDistance(transform.position, hookToConnectChainLinkTo.GetPositionToLinkChainLinkTo(), distance);
        }

        /// <summary>
        /// Returns wether or not a given point is located at the forward side of this <see cref="ChainLinkSource"/> or not.
        /// </summary>
        /// <returns></returns>
        bool PointLiesInForwardDirection(Vector3 position)
        {
            Plane frontPlane = new Plane(transform.forward, transform.position);

            return frontPlane.GetSide(position);
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
                float growPotential = maximumEffectiveChainLinkLength - firstChainLink.CurrentEffectiveLength;

                float effectiveLengthToAdd = Mathf.Min(growPotential, amount);

                firstChainLink.AddEffectiveLength(effectiveLengthToAdd);

                amount -= effectiveLengthToAdd;
            }

            return amount;
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
        }

        void RemoveFirstChainLink()
        {
            GameObject toBeDestroyed = firstChainLink.gameObject;

            hookToConnectChainLinkTo = firstChainLink.AttachedToHook;
            firstChainLink = hookToConnectChainLinkTo.GetComponent<VariableLengthChainLink>();
            
            Destroy(toBeDestroyed);
        }

        float ConnectSpringJointToChainAtLength(float length)
        {
            float springJointWasConnectedAtLength = 0;

            if (firstChainLink)
            {
                float currentChainLinkDistance = 0;
                VariableLengthChainLink currentChainLink = firstChainLink;
                VariableLengthChainLink nextChainLink = currentChainLink.AttachedToHook.GetComponent<VariableLengthChainLink>();

                while (currentChainLinkDistance + currentChainLink.CurrentEffectiveLength < length && nextChainLink)
                {
                    currentChainLinkDistance += currentChainLink.CurrentEffectiveLength;
                    currentChainLink = nextChainLink;
                    nextChainLink = currentChainLink.AttachedToHook.GetComponent<VariableLengthChainLink>();
                }

                if (currentChainLinkDistance + currentChainLink.CurrentEffectiveLength < length)
                {
                    GetSpringJoint().connectedBody = currentChainLink.AttachedToHook.GetRigidbody();
                    GetSpringJoint().connectedAnchor = currentChainLink.AttachedToHook.GetPositionToLinkChainLinkToLocal();
                    springJointWasConnectedAtLength = currentChainLinkDistance + currentChainLink.CurrentEffectiveLength;
                } else
                {
                    GetSpringJoint().connectedBody = currentChainLink.GetRigidbody();
                    GetSpringJoint().connectedAnchor = currentChainLink.GetPositionAtLengthLocal(length - currentChainLinkDistance);
                    springJointWasConnectedAtLength = length;
                }
            }
            else if (hookToConnectChainLinkTo)
            {
                GetSpringJoint().connectedBody = hookToConnectChainLinkTo.GetRigidbody();
                GetSpringJoint().connectedAnchor = hookToConnectChainLinkTo.GetPositionToLinkChainLinkToLocal();
            }

            return springJointWasConnectedAtLength;
        }

        void UpdateSpringJointDistanceValuesBasedOnAnchorPositionInChain(float distanceOfAnchorFromChainBeginning)
        {
            GetSpringJoint().minDistance = distanceOfAnchorFromChainBeginning - maximumTakeUpSpeed * Time.fixedDeltaTime;
            GetSpringJoint().maxDistance = distanceOfAnchorFromChainBeginning + maximumExpellSpeed * Time.fixedDeltaTime;
        }


        Vector3 GetVelocity() => GetMovementSincePreviousFixedUpdate() / Time.fixedDeltaTime;

        Vector3 GetMovementSincePreviousFixedUpdate() => transform.position - positionAfterPreviousFixedUpdate;


        // <<<<< REFERENCED OUTSIDE >>>>>

        public void DestroyChainAndResetHookPosition()
        {
            if (hookToConnectChainLinkTo)
            {
                while (firstChainLink)
                    RemoveFirstChainLink();

                hookToConnectChainLinkTo.transform.position = transform.position;
            }
        }

        public void SetHookToConnectChainLinkTo(ChainLinkHook hook)
        {
            hookToConnectChainLinkTo = hook;
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

        /// <summary>
        /// This method shows some problems when played. Ignore it for now since it is not needed for the GrapplingGun.
        /// </summary>
        public void UnlockRopeLength()
        {
            Debug.LogWarning("UnlockRopeLength has been called, but it is error-prone!");

            maximumExpellSpeed = Mathf.Infinity;
            maximumTakeUpSpeed = Mathf.Infinity;
        }

        // <<<<< VALIDATION >>>>>

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

