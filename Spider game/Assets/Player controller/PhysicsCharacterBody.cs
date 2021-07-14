using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsCharacterBody : MonoBehaviour
{
    public float maximumGroundVelocity;
    public float maximumAirVelocity;
    public float groundAcceleration;
    public float airAcceleration;

    public float jumpHeight;
    public float groundCheckRadius;
    public LayerMask groundObjectLayers;
    public Transform groundCheckOrigin;

    float amountToMoveForward;
    float amountToMoveRight;

    new Rigidbody rigidbody;

    Rigidbody GetRigidbody()
    {
        if (!rigidbody)
            rigidbody = GetComponent<Rigidbody>();

        return rigidbody;
    }

    private void FixedUpdate()
    {
        MoveAccordingToSetMovementAmounts();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(groundCheckOrigin.position, groundCheckRadius);
    }

    public void AttemptJump()
    {
        if (IsInContactWithGround())
            Jump();
    }

    public void SetAmountToMoveForward(float amount)
    {
        amountToMoveForward = Mathf.Clamp(amount, -1, 1);
    }

    public void SetAmountToMoveRight(float amount)
    {
        amountToMoveRight = Mathf.Clamp(amount, -1, 1);
    }

    void Jump()
    {
        Vector3 jumpingForce = Vector3.up * CalculateJumpingForceToReachHeight(jumpHeight);

        GetRigidbody().AddForce(jumpingForce, ForceMode.VelocityChange);
    }

    void MoveAccordingToSetMovementAmounts()
    {
        float maximumSpeed;
        float acceleration;

        if (IsInContactWithGround())
        {
            maximumSpeed = maximumGroundVelocity;
            acceleration = groundAcceleration;
        }
        else
        {
            maximumSpeed = maximumAirVelocity;
            acceleration = airAcceleration;
        }

        // Calculate how fast we should be moving in which direction
        Vector3 targetVelocity = (transform.right * amountToMoveRight + transform.forward * amountToMoveForward) * maximumSpeed;
        Vector3 localTargetVelocity = transform.InverseTransformVector(targetVelocity);

        Vector3 localVelocity = transform.InverseTransformVector(GetRigidbody().velocity);

        Vector3 localForceToApply = Vector3.zero;
        Vector3 velocityOffset = (localTargetVelocity - localVelocity);

        // If the body is not moving in the target direction or if it is not moving fast enough, then apply a force in that direction.
        if (Mathf.Sign(localVelocity.x) != Mathf.Sign(localTargetVelocity.x) || Mathf.Abs(localTargetVelocity.x) < Mathf.Abs(localVelocity.x))
        {
            localForceToApply.x = Mathf.Clamp(velocityOffset.x, -acceleration, acceleration);
        }

        // Same calculation is done using the z axis.
        if (Mathf.Sign(localVelocity.z) != Mathf.Sign(localTargetVelocity.z) || Mathf.Abs(localTargetVelocity.z) < Mathf.Abs(localVelocity.z))
        {
            localForceToApply.z = Mathf.Clamp(velocityOffset.z, -acceleration, acceleration);
        }

        Vector3 forceToApply = transform.TransformVector(localForceToApply);

        rigidbody.AddForce(forceToApply, ForceMode.VelocityChange);
    }

    /// <summary>
    /// Imagine a square with the vertices (-1, -1), (-1, 1), (1, -1), and (1, 1), and a vector inside this square. This function returns a factor that, if applied to the vector, projects it onto the unit circle. This can, for example, be used to prevent strafing in first-person games.
    /// </summary>
    /// <param name="boxVector"></param>
    /// <returns></returns>
    float CalculateSquareToCircleProjectionFactorForVector(Vector2 boxVector)
    {
        boxVector = boxVector.normalized;

        float projectionFactor;

        float xAbs = Mathf.Abs(boxVector.x);
        float yAbs = Mathf.Abs(boxVector.y);

        if (xAbs == 0 && yAbs == 0)
            projectionFactor = 0;
        else
        {
            Vector3 vectorProjectedToSquareBounds;

            if (xAbs <= yAbs && yAbs != 0)
                vectorProjectedToSquareBounds = boxVector / yAbs;
            else // Don't need to check here if xAbs != 0 since either zAbs was 0 but they cannot both be zero, or zAbs was smaller than xAbs which means that it has to be greater than zero
                vectorProjectedToSquareBounds = boxVector / xAbs;

            projectionFactor = 1 / vectorProjectedToSquareBounds.magnitude;
        }

        return projectionFactor;
    }

    float CalculateJumpingForceToReachHeight(float height) => Mathf.Sqrt(2 * height * -Physics.gravity.y);

    bool IsInContactWithGround() => Physics.CheckSphere(groundCheckOrigin.transform.position, groundCheckRadius, groundObjectLayers);
}
