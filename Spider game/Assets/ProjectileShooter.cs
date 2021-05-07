using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    [Tooltip("The projectile gameObject to spawn, which is accelerated if it has a rigidbody attached.")]
    public GameObject projectilePrefab;
    [Tooltip("The amount of force with which the projectile is accelerated when spawned.")]
    public float projectileAccelerationForce;
    [Tooltip("This amount of time has to pass after each shot before a new shot can be made.")]
    public float shootCooldownTime;

    private float timeSinceLastShot;
    private bool isTriggered;

    // Update is called once per frame
    void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        if (isTriggered)
        {
            ShootIfCooledDown();
        }
    }

    public void StartShooting()
    {
        isTriggered = true;
    }

    public void StopShooting()
    {
        isTriggered = false;
    }

    public void ShootIfCooledDown()
    {
        if (shootCooldownTime <= timeSinceLastShot)
            Shoot();
    }

    private void Shoot()
    {
        GameObject projectileToShoot = Instantiate(projectilePrefab, transform.position, transform.rotation);
        AccelerateProjectile(projectileToShoot);
        timeSinceLastShot = 0;
    }

    private void AccelerateProjectile(GameObject projectile)
    {
        Rigidbody rigidbody = projectile.GetComponent<Rigidbody>();

        rigidbody?.AddForce(transform.forward * projectileAccelerationForce);
    }
}
