using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Rotator : MonoBehaviour
{


    [Header("Rotational forces")]

    public float xTorque;
    public float xMaximumSpeed;

    public float yTorque;
    public float yMaximumSpeed;

    public float zTorque;
    public float zMaximumSpeed;

    new Rigidbody rigidbody;

    private void Reset()
    {

    }

    Rigidbody GetRigidbody()
    {
        if (!rigidbody)
            rigidbody = GetComponent<Rigidbody>();

        return rigidbody;
    }

    private void FixedUpdate()
    {

    }
}
