using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RopeGun : MonoBehaviour
{
    public float GrappleShootingForce;
    public ChainLinkSource chainLinkSource;
    public Rigidbody grappler;



    RopeGunState gunState;

    enum RopeGunState
    {
        loaded,
        shot,
    }

    public void StartPressingTrigger()
    {
        if (gunState == RopeGunState.loaded)
        {
            ShootGrappler();
            gunState = RopeGunState.shot;
        }
    }

    private void ShootGrappler()
    {
        grappler.AddForce(Vector3.forward * GrappleShootingForce, ForceMode.VelocityChange);
    }
}
