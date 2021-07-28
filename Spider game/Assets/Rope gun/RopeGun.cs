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
    public Sticky projectile;


    [Header("Rope attaching")]
    public LayerMask raycastObstacles;
    public LayerMask possibleObjectsToAttachRopeTo;
    public float maximumDistanceToSurfaceToAttachRopeTo;

    RopeGunState gunState;

    enum RopeGunState
    {
        loaded,
        grappleInAir,
        grappleConnected
    }

    private void OnEnable()
    {
        SwitchToState(RopeGunState.loaded);
        PressReloadButton(); // Makes sure that there is a grapple to shoot
    }

    private void Update()
    {
        if (gunState == RopeGunState.loaded)
        {
            projectile.transform.position = muzzle.position;
            projectile.transform.rotation = muzzle.rotation;
        }

    }

    // Buttons of the actual ropeGun

    public void StartPressingTrigger()
    {
        if (gunState == RopeGunState.loaded)
        {
            SwitchToState(RopeGunState.grappleInAir);
            ShootGrappler();
        }
    }

    public void StopPressingTrigger()
    {

    }

    public void PressReloadButton()
    {
        DestroyRope();

        projectile = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
        chainLinkSource.SetHookToConnectChainLinkTo(projectile.GetComponent<ChainLinkHook>());

        SwitchToState(RopeGunState.loaded);
        grappleDisconnected.Raise();
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

    private void ShootGrappler()
    {
        projectile.GetRigidbody()?.AddForce(muzzle.forward * shootingForce, ForceMode.VelocityChange);
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

    void SwitchToState(RopeGunState state)
    {
        switch (state)
        {
            case RopeGunState.loaded:
                SwitchToLoadedState();
                break;
            case RopeGunState.grappleInAir:
                SwitchToGrappleInAirState();
                break;
            case RopeGunState.grappleConnected:
                SwitchToGrappleConnectedState();
                break;
        }
    }

    void SwitchToLoadedState()
    {
        gunState = RopeGunState.loaded;
        chainLinkSource.maximumPullInSpeed = 0;
        chainLinkSource.maximumPushOutSpeed = 0;
        projectile.DisableStickiness();
    }

    void SwitchToGrappleInAirState()
    {
        gunState = RopeGunState.grappleInAir;
        chainLinkSource.maximumPullInSpeed = Mathf.Infinity;
        chainLinkSource.maximumPushOutSpeed = Mathf.Infinity;
        projectile.EnableStickiness();
    }

    void SwitchToGrappleConnectedState()
    {
        gunState = RopeGunState.grappleConnected;
        chainLinkSource.maximumPullInSpeed = 0;
        chainLinkSource.maximumPushOutSpeed = 0;
    }



    public void OnGrappleConnected()
    {
        SwitchToState(RopeGunState.grappleConnected);
    }
}
