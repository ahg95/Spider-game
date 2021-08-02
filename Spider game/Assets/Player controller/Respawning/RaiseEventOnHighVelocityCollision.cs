using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RaiseEventOnHighVelocityCollision : MonoBehaviour
{
    [Tooltip("The game event that is to be raised when the ")]
    public GameEvent highVelocityCollisionOccured;

    [Tooltip("A collision has to happen with at least this amount of relative velocity in order for the event to be raised.")]
    public float minimumRelativeVelocity;

    private void OnCollisionEnter(Collision collision)
    {
        if (minimumRelativeVelocity <= collision.relativeVelocity.magnitude)
        {
            if (highVelocityCollisionOccured)
                highVelocityCollisionOccured.Raise();
        }
    }
}
