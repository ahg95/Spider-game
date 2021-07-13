using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SpringJoint))]
public class ChainLinkSource : MonoBehaviour
{
    [SerializeField]
    ChainLinkHook hookToConnectChainLinkTo;

    public ChainLink chainLinkPrefab;
    public Transform chainLinkParent;

    [Range(0, 1)]
    public float friction;
    public float pushOutForceAmount;
    public float maximumPushOutSpeed;
    public float maximumPullInSpeed;

    SpringJoint joint;
    Vector3 positionAfterPreviousFixedUpdate;

    private SpringJoint GetSpringJoint()
    {
        if (joint == null)
            joint = GetComponent<SpringJoint>();
        return joint;
    }

    private void OnEnable()
    {
        positionAfterPreviousFixedUpdate = transform.position;
        ConnectSpringJointTohookToConnectChainLinkTo();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (hookToConnectChainLinkTo != null) {
            if (2 <= Vector3.Distance(hookToConnectChainLinkTo.GetPositionToLinkChainLinkTo(), transform.position))
                SpawnAndAttachChainLinkToHook();
            else if (Vector3.Distance(hookToConnectChainLinkTo.transform.position, transform.position) <= 1)
                ShortenRopeByOneLink();

            ApplyFrictionToHookToConnectChainLinkTo();

            ApplyPushOutForce();

            UpdateSpringJointValues();
        }

        UpdatePositionAfterPreviousFixedUpdate();
    }

    public void SetHookToConnectChainLinkTo(ChainLinkHook hook)
    {
        hookToConnectChainLinkTo = hook;
        ConnectSpringJointTohookToConnectChainLinkTo();
    }

    void SpawnAndAttachChainLinkToHook()
    {
        GameObject spawnedChainLink = Instantiate(chainLinkPrefab.gameObject, chainLinkParent);

        spawnedChainLink.GetComponent<ChainLink>().AttachToChainLinkHookAndRotateTowards(hookToConnectChainLinkTo, transform.position);

        hookToConnectChainLinkTo = spawnedChainLink.GetComponent<ChainLinkHook>();

        ConnectSpringJointTohookToConnectChainLinkTo();
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
        // This function has a problem: it shows creeping behavour, meaning that even if the friction is 1, the hookToConnectChainLinkTo still moves slightly.
        Vector3 currentHookVelocity = hookToConnectChainLinkTo.GetRigidbody().velocity;

        hookToConnectChainLinkTo.GetRigidbody().AddForce(-currentHookVelocity * friction, ForceMode.VelocityChange);

        // Because there is a friction, the chainLinkHook that this source is connected to should also move some amount when this source is moved.
        hookToConnectChainLinkTo.GetRigidbody().AddForce(GetVelocity() * friction, ForceMode.VelocityChange);
    }

    void ApplyPushOutForce()
    {
        Vector3 pushOutForceDirection = (hookToConnectChainLinkTo.transform.position - transform.position).normalized;
        hookToConnectChainLinkTo.GetRigidbody().AddForce(pushOutForceDirection * pushOutForceAmount);
    }

    void UpdateSpringJointValues()
    {
        float distanceToHook = (hookToConnectChainLinkTo.transform.position - transform.position).magnitude;

        GetSpringJoint().minDistance = distanceToHook - maximumPullInSpeed * Time.fixedDeltaTime;
        GetSpringJoint().maxDistance = distanceToHook + maximumPushOutSpeed * Time.fixedDeltaTime;
    }

    void ConnectSpringJointTohookToConnectChainLinkTo() => GetSpringJoint().connectedBody = hookToConnectChainLinkTo.GetRigidbody();

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
