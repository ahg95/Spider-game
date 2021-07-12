using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeGun : MonoBehaviour
{
    public float GrappleShootingForce;
    public ChainLinkSource chainLinkSource;
    public Rigidbody grappler;
    public Transform shootingDirection;

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
        grappler.DisableStickiness();
    }

    void SwitchToGrappleInAirState()
    {
        gunState = RopeGunState.grappleInAir;
        chainLinkSource.maximumPullInSpeed = Mathf.Infinity;
        chainLinkSource.maximumPushOutSpeed = Mathf.Infinity;
        grappler.EnableStickiness();
    }

    void SwitchToGrappleConnectedState()
    {
        gunState = RopeGunState.grappleConnected;
        chainLinkSource.maximumPullInSpeed = 0;
        chainLinkSource.maximumPushOutSpeed = 0;
    }

    private void ShootGrappler()
    {
        grappler.GetRigidbody()?.AddForce(shootingDirection.forward * GrappleShootingForce, ForceMode.VelocityChange);
    }

    public void OnGrappleConnected()
    {
        SwitchToState(RopeGunState.grappleConnected);
    }
}
