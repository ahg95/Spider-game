using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnsgarsAssets
{
    [RequireComponent(typeof(Rigidbody), typeof(SpringJoint))]
    public class ChainLinkSource : MonoBehaviour
    {
        [SerializeField]
        ChainLinkHook hookToConnectChainLinkTo;

        public VariableLengthChainLink chainLinkPrefab;
        public Transform chainLinkParent;

        [Range(0.5f, 2)]
        public float maximumChainLinkLength = 1;

        [Header("Uptaking and Expelling")]
        [Range(0, 1)]
        public float friction;
        public float pushOutForceAmount;
        public float maximumPushOutSpeed;
        public float maximumPullInSpeed;

        SpringJoint joint;
        new Rigidbody rigidbody;

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
            if (hookToConnectChainLinkTo)
            {

                if (0 < maximumPushOutSpeed && 2 <= Vector3.Distance(hookToConnectChainLinkTo.GetPositionToLinkChainLinkTo(), transform.position))
                    SpawnAndAttachChainLinkToHook();
                else if (0 < maximumPullInSpeed && Vector3.Distance(hookToConnectChainLinkTo.transform.position, transform.position) <= 1)
                    ShortenRopeByOneLink();

                ApplyFrictionToHookToConnectChainLinkTo();

                ApplyPushOutForce();

                // Before the physics engine does calculations, the SpringJoint values should be updated to limit the movement of the hookToConnectChainLinkTo
                UpdateSpringJointValues();
            }

            UpdatePositionAfterPreviousFixedUpdate();
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
            maximumPushOutSpeed = 0;
            maximumPullInSpeed = 0;
        }

        public void UnlockRopeLength()
        {
            maximumPushOutSpeed = Mathf.Infinity;
            maximumPullInSpeed = Mathf.Infinity;
        }

        void SpawnAndAttachChainLinkToHook()
        {
            GameObject spawnedChainLink = Instantiate(chainLinkPrefab.gameObject, chainLinkParent);

            spawnedChainLink.GetComponent<ChainLink>().AttachToChainLinkHookAndRotateTowards(hookToConnectChainLinkTo, transform.position);

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

            GetSpringJoint().minDistance = distanceToHook - maximumPullInSpeed * Time.fixedDeltaTime;
            GetSpringJoint().maxDistance = distanceToHook + maximumPushOutSpeed * Time.fixedDeltaTime;
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

