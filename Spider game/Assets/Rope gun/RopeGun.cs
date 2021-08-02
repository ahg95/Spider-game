using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeGun : MonoBehaviour
{
    [Header("General")]
    public float shootingForceAmount;

    public Transform muzzle;
    [Tooltip("The specified rigidbody and the source of the rope will be connected by a fixed joint.")]
    public Rigidbody rigidBodyToConnectChainLinkSourceTo;

    [Header("Prefabs")]
    public GameObject ropePrefab;
    public Sticky projectilePrefab;
    public ChainLinkSource chainLinkSourcePrefab;

    [Header("Rope attaching")]
    public LayerMask raycastObstacles;
    public LayerMask possibleObjectsToAttachRopeTo;
    public float maximumDistanceToSurfaceToAttachRopeTo;

    [Header("Optional bookkeeping")]
    public Transform ropeParent;
    public Transform chainLinkSourceParent;
    public Transform projectileParent;

    [Header("Events")]
    public GameEvent grappleDisconnected;

    GameObject rope;
    Sticky projectile;
    ChainLinkSource chainLinkSource;

    RopeGunState gunState = RopeGunState.unloaded;

    enum RopeGunState
    {
        unloaded, // The initial state of the gun. There is no projectile, no chainLinkSource, and no rope.
        loaded, // The gun is ready to fire.
        shot, // The projectile is shot out and flying in the air.
        connected // The projectile has hit something.
    }

    private void OnEnable()
    {
        SwitchToLoadedState();
    }

    private void Update()
    {
        if (gunState == RopeGunState.loaded)
            MoveProjectileToMuzzleIfExistent();
    }

    // --- Buttons of the actual ropeGun that the player can press ---

    public void StartPressingTrigger()
    {
        if (gunState == RopeGunState.loaded)
            SwitchToShotState();
    }

    public void StopPressingTrigger()
    {

    }

    public void PressReloadButton()
    {
        if (gunState == RopeGunState.shot
            || gunState == RopeGunState.connected)
            SwitchToLoadedState();
    }

    public void PressAttachButton()
    {
        if (gunState == RopeGunState.connected)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(muzzle.position, muzzle.forward, out hitInfo, maximumDistanceToSurfaceToAttachRopeTo, raycastObstacles))
            {
                GameObject hitObject = hitInfo.collider.gameObject;

                if (LayerMaskContainsLayer(possibleObjectsToAttachRopeTo, hitObject.layer))
                {
                    AttachRopeToObjectAtPositionWithNormal(hitObject, hitInfo.point, hitInfo.normal);

                    SwitchToUnloadedState();
                }
            }
        }
    }

    // --- State transitions ---

    void SwitchToUnloadedState()
    {
        rope = null;
        projectile = null;
        chainLinkSource = null;

        gunState = RopeGunState.unloaded;

        // Automatic reloading
        SwitchToLoadedState();
    }

    void SwitchToLoadedState()
    {
        if (gunState == RopeGunState.unloaded)
        {
            InstantiateAndConfigureRopeIfNotExistent();
            InstantiateAndConfigureProjectileIfNotExistent();
            InstantiateAndConfigureChainLinkSourceIfNotExistent();
        }
        else if (gunState == RopeGunState.shot)
        {
            chainLinkSource.PullInRopeInstantly();
        }
        else if (gunState == RopeGunState.connected)
        {
            chainLinkSource.PullInRopeInstantly();

            grappleDisconnected.Raise();
        }

        chainLinkSource.LockRopeLength();
        projectile.DisableStickiness();

        gunState = RopeGunState.loaded;
    }

    void SwitchToShotState()
    {
        MoveProjectileToMuzzleIfExistent();
        ShootProjectile();

        chainLinkSource.UnlockRopeLength();
        projectile.EnableStickiness();

        gunState = RopeGunState.shot;
    }

    void SwitchToConnectedState()
    {
        chainLinkSource.LockRopeLength();

        gunState = RopeGunState.connected;
    }

    // --- Other methods ---

    void MoveProjectileToMuzzleIfExistent()
    {
        if (projectile)
        {
            projectile.transform.position = muzzle.position;
            projectile.transform.rotation = muzzle.rotation;
        }
    }

    void InstantiateAndConfigureRopeIfNotExistent()
    {
        if (!rope)
        {
            rope = Instantiate(ropePrefab, ropeParent);

            if (chainLinkSource)
                chainLinkSource.chainLinkParent = rope.transform;
        }
    }

    void InstantiateAndConfigureProjectileIfNotExistent()
    {
        if (!projectile)
        {
            projectile = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
            projectile.DisableStickiness();

            if (chainLinkSource)
                chainLinkSource.SetHookToConnectChainLinkTo(projectile.GetComponent<ChainLinkHook>());
        }

    }

    void InstantiateAndConfigureChainLinkSourceIfNotExistent()
    {
        if (!chainLinkSource)
        {
            chainLinkSource = Instantiate(chainLinkSourcePrefab, rigidBodyToConnectChainLinkSourceTo.transform.position, Quaternion.identity, chainLinkSourceParent);
            chainLinkSource.GetComponent<FixedJoint>().connectedBody = rigidBodyToConnectChainLinkSourceTo;

            if (rope)
                chainLinkSource.chainLinkParent = rope.transform;

            if (projectile)
                chainLinkSource.SetHookToConnectChainLinkTo(projectile.GetComponent<ChainLinkHook>());
        }
    }

    private bool LayerMaskContainsLayer(LayerMask mask, int layer) => IntegerHasBitSetAtIndex(mask.value, layer);

    // First, we shift the bit that we want to investigate to the rightmost position, which determines if the integer is even or odd. Then we set every other bit to '0' by doing a bitwise AND operation ('&') with the number one,
    // which has a '0' on all bits except for the rightmost. If this number is equal to one, only then the bit was set.
    private bool IntegerHasBitSetAtIndex(int integer, int index) => (((integer >> index) & 1) == 1);

    private void AttachRopeToObjectAtPositionWithNormal(GameObject gameObject, Vector3 position, Vector3 direction)
    {
        chainLinkSource.transform.position = position;

        Sticky sticky = chainLinkSource.GetComponent<Sticky>();

        if (!sticky)
            Debug.LogError("The ChainLinkSource has no sticky script attached.");

        sticky.StickTo(gameObject);

        // TODO: rotate it towards the diraction
    }

    void ShootProjectile()
    {
        projectile.GetRigidbody()?.AddForce(muzzle.forward * shootingForceAmount, ForceMode.VelocityChange);
    }

    public void OnGrappleConnected()
    {
        if (gunState == RopeGunState.shot)
            SwitchToConnectedState();
    }
}
