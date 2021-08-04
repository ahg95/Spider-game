using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterController : MonoBehaviour
{
    public ProjectileShooter shooter;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && shooter)
            shooter.ShootIfCooledDown();
    }

}
