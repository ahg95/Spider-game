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
        switch(state)
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
