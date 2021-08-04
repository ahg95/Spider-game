using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Rotator : MonoBehaviour
{
    public Vector3 rotationSpeed;

    new Rigidbody rigidbody;

    Rigidbody GetRigidbody()
    {
        if (!rigidbody)
            rigidbody = GetComponent<Rigidbody>();

        return rigidbody;
    }

    private void FixedUpdate()
    {
        GetRigidbody().MoveRotation(transform.rotation * Quaternion.Euler(rotationSpeed * Time.fixedDeltaTime));
    }
}
