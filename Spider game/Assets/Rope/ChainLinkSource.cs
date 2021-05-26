using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SpringJoint))]
public class ChainLinkSource : MonoBehaviour
{
    public ChainLinkHook hookToConnectChainLinkTo;

    public ChainLink chainLinkPrefab;

    [Range(0, 1)]
    public float frictionForceAmount;

    public float pushOutForceAmount;

    public float maximumPushOutSpeedForForce;
    public float maximumPullInSpeedForForce;

    private Joint joint;
    private Vector3 positionAfterPreviousFixedUpdate;

    private void OnEnable()
    {
        positionAfterPreviousFixedUpdate = transform.position;
    }

    private Joint GetJoint()
    {
        if (joint == null)
            joint = GetComponent<Joint>();
        return joint;
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

            float currentPushOutSpeed = CalculateCurrentPushOutSpeed();

            if ((0 < pushOutForceAmount && CalculateCurrentPushOutSpeed() < maximumPushOutSpeedForForce)
                || (pushOutForceAmount < 0 && -CalculateCurrentPushOutSpeed() < maximumPullInSpeedForForce))
                ApplyPushOutForce();
        }

        UpdatePositionAfterPreviousFixedUpdate();
    }

    private Vector3 GetMovementSincePreviousFixedUpdate() => transform.position - positionAfterPreviousFixedUpdate;

    private void UpdatePositionAfterPreviousFixedUpdate() => positionAfterPreviousFixedUpdate = transform.position;

    public void DisconnectRope()
    {
        hookToConnectChainLinkTo = null;
    }

    public void LockRopePullOut()
    {

    }

    public void UnlockRopePullOut()
    {

    }

    public void LockRopePullIn()
    {

    }

    private void ShortenRopeByOneLink()
    {
        GameObject objectToDestroy = hookToConnectChainLinkTo.gameObject;

        ChainLink attachedChainLink = objectToDestroy.GetComponent<ChainLink>();

        if (attachedChainLink != null)
        {
            hookToConnectChainLinkTo = attachedChainLink.AttachedToHook;

            Destroy(objectToDestroy);
        }
    }

    private void ApplyFrictionToHookToConnectChainLinkTo()
    {
        // This function has a problem: it shows creeping behavour, meaning that even if the friction is 1, the hookToConnectChainLinkTo still moves slightly.

        Vector3 currentHookVelocity = hookToConnectChainLinkTo.GetRigidbody().velocity;

        hookToConnectChainLinkTo.GetRigidbody().AddForce(-currentHookVelocity * frictionForceAmount, ForceMode.VelocityChange);

        // Because there is a friction, the chainLinkHook that this source is connected to should also move some amount when this source is moved.
        hookToConnectChainLinkTo.GetRigidbody().AddForce(GetVelocity() * frictionForceAmount, ForceMode.VelocityChange);
    }

    private void ApplyPushOutForce()
    {
        Vector3 pushOutForceDirection = (hookToConnectChainLinkTo.transform.position - transform.position).normalized;
        hookToConnectChainLinkTo.GetRigidbody().AddForce(pushOutForceDirection * pushOutForceAmount);
    }

    private Vector3 GetVelocity() => GetMovementSincePreviousFixedUpdate() / Time.fixedDeltaTime;

    private float CalculateCurrentPushOutSpeed()
    {
        float pushOutSpeed;

        Vector3 hookDirection = (hookToConnectChainLinkTo.transform.position - transform.position).normalized;

        pushOutSpeed = Vector3Utility.GetProjectionFactor(hookDirection, hookToConnectChainLinkTo.GetRigidbody().velocity - GetVelocity());

        return pushOutSpeed;
    }

    private void SpawnAndAttachChainLinkToHook()
    {
        GameObject spawnedChainLink = Instantiate(chainLinkPrefab.gameObject);

        spawnedChainLink.GetComponent<ChainLink>().AttachToChainLinkHookAndRotateTowards(hookToConnectChainLinkTo, transform.position);

        hookToConnectChainLinkTo = spawnedChainLink.GetComponent<ChainLinkHook>();
    }
}
