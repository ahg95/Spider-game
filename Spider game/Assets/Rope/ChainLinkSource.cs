using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Joint))]
public class ChainLinkSource : MonoBehaviour
{
    public ChainLinkHook hookToConnectChainLinkTo;

    public ChainLink chainLinkPrefab;

    [Range(0, 1)]
    public float frictionForceAmount;

    public float pushOutForceAmount;

    private Joint joint;

    private Joint GetJoint()
    {
        if (joint == null)
            joint = GetComponent<Joint>();
        return joint;
    }

    private void OnEnable()
    {
        joint = GetComponent<Joint>();
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
        }
    }

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
        Vector3 currentHookVelocity = hookToConnectChainLinkTo.GetRigidbody().velocity;

        hookToConnectChainLinkTo.GetRigidbody().AddForce(-currentHookVelocity * frictionForceAmount, ForceMode.VelocityChange);
    }

    private void ApplyPushOutForce()
    {
        Vector3 pushOutForceDirection = (hookToConnectChainLinkTo.transform.position - transform.position).normalized;

        hookToConnectChainLinkTo.GetRigidbody().AddForce(pushOutForceDirection * pushOutForceAmount);
    }



    private void SpawnAndAttachChainLinkToHook()
    {
        GameObject spawnedChainLink = Instantiate(chainLinkPrefab.gameObject);

        spawnedChainLink.GetComponent<ChainLink>().AttachToChainLinkHookAndRotateTowards(hookToConnectChainLinkTo, transform.position);

        hookToConnectChainLinkTo = spawnedChainLink.GetComponent<ChainLinkHook>();
    }
}
