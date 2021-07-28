using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeGun : MonoBehaviour
{
    public GameEvent grappleDisconnected;

    public float shootingForce;
    public Transform muzzle;
    public ChainLinkSource chainLinkSource;
    public Sticky projectilePrefab;


    [Header("Rope attaching")]
    public LayerMask raycastObstacles;
    public LayerMask possibleObjectsToAttachRopeTo;
    public float maximumDistanceToSurfaceToAttachRopeTo;

    Sticky projectile;
    RopeGunState gunState;

    enum RopeGunState
    {
        undefinedState,
        loaded,
        grappleInAir,
        grappleConnected
    }

    private void OnEnable()
    {
        SwitchToLoadedState();
    }

    private void Update()
    {
        if (gunState == RopeGunState.loaded)
            MoveProjectileToMuzzle();
    }

    void MoveProjectileToMuzzle()
    {
        projectile.transform.position = muzzle.position;
        projectile.transform.rotation = muzzle.rotation;
    }

    // Buttons of the actual ropeGun

    public void StartPressingTrigger()
    {
        if (gunState == RopeGunState.loaded)
        {
            SwitchToGrappleInAirState();
        }
    }

    public void StopPressingTrigger()
    {

    }

    public void PressReloadButton()
    {
        SwitchToLoadedState();
    }

    public void PressAttachRopeToSurfaceButton()
    {
        if (gunState == RopeGunState.grappleConnected)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(muzzle.position, muzzle.forward, out hitInfo, maximumDistanceToSurfaceToAttachRopeTo, raycastObstacles))
            {
                GameObject hitObject = hitInfo.collider.gameObject;

                if (LayerMaskContainsLayer(possibleObjectsToAttachRopeTo, hitObject.layer))
                {
                    AttachRopeToObjectAtPositionWithNormal(hitObject, hitInfo.point, hitInfo.normal);
                    InstantiateChainLinkSource();

                    Debug.Log("Attached rope to surface.");
                }
            }
        }
    }

    public void OnGrappleConnected()
    {
        SwitchToGrappleConnectedState();
    }

    private bool LayerMaskContainsLayer(LayerMask mask, int layer) => IntegerHasBitSetAtIndex(mask.value, layer);

    // First, we shift the bit that we want to investigate to the rightmost position, which determines if the integer is even or odd. Then we set every other bit to '0' by doing a bitwise AND operation ('&') with the number one,
    // which has a '0' on all bits except for the rightmost. If this number is equal to one, only then the bit was set.
    private bool IntegerHasBitSetAtIndex(int integer, int index) => (((integer >> index) & 1) == 1);


    private void InstantiateChainLinkSource()
    {
        //throw new NotImplementedException();
    }

    private void AttachRopeToObjectAtPositionWithNormal(GameObject gameObject, Vector3 position, Vector3 direction)
    {
        //throw new NotImplementedException();
    }

    void SwitchToLoadedState()
    {
        if (gunState != RopeGunState.loaded)
        {
            if (gunState == RopeGunState.grappleConnected)
                grappleDisconnected.Raise();

            DestroyRope();

            projectile = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
            chainLinkSource.SetHookToConnectChainLinkTo(projectile.GetComponent<ChainLinkHook>());

            projectile.DisableStickiness();
            

            chainLinkSource.maximumPullInSpeed = 0;
            chainLinkSource.maximumPushOutSpeed = 0;


            gunState = RopeGunState.loaded;
        }
    }

    void DestroyRope()
    {
        for (int i = 0; i < chainLinkSource.chainLinkParent.childCount; i++)
        {
            Destroy(chainLinkSource.chainLinkParent.GetChild(i).gameObject);
        }

        if (projectile)
            Destroy(projectile.gameObject);
    }

    void SwitchToGrappleInAirState()
    {
        if (gunState != RopeGunState.grappleInAir)
        {
            if (gunState == RopeGunState.loaded)
            {
                MoveProjectileToMuzzle();
                ShootGrappler();
            }


            chainLinkSource.maximumPullInSpeed = Mathf.Infinity;
            chainLinkSource.maximumPushOutSpeed = Mathf.Infinity;
            projectile.EnableStickiness();


            gunState = RopeGunState.grappleInAir;
        }
    }

    private void ShootGrappler()
    {
        projectile.GetRigidbody()?.AddForce(muzzle.forward * shootingForce, ForceMode.VelocityChange);
    }

    void SwitchToGrappleConnectedState()
    {
        if (gunState != RopeGunState.grappleConnected)
        {
            chainLinkSource.maximumPullInSpeed = 0;
            chainLinkSource.maximumPushOutSpeed = 0;


            gunState = RopeGunState.grappleConnected;
        }
    }
}
