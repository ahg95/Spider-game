using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsCharacterBody : MonoBehaviour
{
    [Header("Ground movement")]
    public float groundMaximumSpeed;
    public float groundAcceleration;

    [Header("Air movement")]
    public float airMaximumSpeed;
    public float airAcceleration;

    [Header("Jumping")]
    public float jumpHeight;

    [Header("Ground checking")]
    public float groundCheckRadius;
    public LayerMask groundObjectLayers;
    public Transform groundCheckOrigin;

    Vector2 movementInput;

    new Rigidbody rigidbody;

    Rigidbody GetRigidbody()
    {
        if (!rigidbody)
            rigidbody = GetComponent<Rigidbody>();

        return rigidbody;
    }

    private void FixedUpdate()
    {
        MoveAccordingToMovementInput();
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

    public void SetMovementInput(Vector2 input)
    {
        movementInput = input;
    }

    void Jump()
    {
        Vector3 jumpingForce = Vector3.up * CalculateJumpingForceToReachHeight(jumpHeight);

        GetRigidbody().AddForce(jumpingForce, ForceMode.VelocityChange);
    }

    /// <summary>
    /// Applies the movement to this character according to the movementInput member.
    /// </summary>
    void MoveAccordingToMovementInput()
    {
        float maximumSpeed;
        float maximumDeltaVelocity;

        if (IsInContactWithGround())
        {
            maximumSpeed = groundMaximumSpeed;
            maximumDeltaVelocity = groundAcceleration * Time.fixedDeltaTime;
        }
        else
        {
            maximumSpeed = airMaximumSpeed;
            maximumDeltaVelocity = airAcceleration * Time.fixedDeltaTime;
        }

        // The following line prevents that the character moves faster diagonally
        Vector2 movement = TransformSquareDomainVectorToCircleDomain(movementInput);


        /* The following vectors are created or transformed to the local space of this transform because it is necessary for the following if clauses. If the vectors were in world space, the individual components of the vectors would
         * not represent the axes on which the player is moving.
         */

        // The following vector represents the desired velocity for this character in local space.
        Vector3 localTargetVelocity = (Vector3.right * movement.x + Vector3.forward * movement.y) * maximumSpeed;

        Vector3 localVelocity = transform.InverseTransformVector(GetRigidbody().velocity);

        Vector3 localForceToApply = Vector3.zero;
        Vector3 localVelocityOffset = (localTargetVelocity - localVelocity);

        if (   (0 < localTargetVelocity.x && localVelocity.x < localTargetVelocity.x) // If we want to move right and we are moving slower than we want
            || (localTargetVelocity.x < 0 && localTargetVelocity.x < localVelocity.x) // or if we want to move left and we are moving slower than we want
            || (localTargetVelocity.x == 0 && localVelocity.x != 0)) // or if we don't want to move right or left, but we are
            localForceToApply.x = Mathf.Clamp(localVelocityOffset.x, -maximumDeltaVelocity, maximumDeltaVelocity); // then we apply a force towards our target velocity.

        if (   (0 < localTargetVelocity.z && localVelocity.z < localTargetVelocity.z) // If we want to move forward and we are moving slower than we want
            || (localTargetVelocity.z < 0 && localTargetVelocity.z < localVelocity.z) // or if we want to move backward and we are moving slower than we want
            || (localTargetVelocity.z == 0 && localVelocity.z != 0)) // or if we don't want to move forward or backward, but we are
            localForceToApply.z = Mathf.Clamp(localVelocityOffset.z, -maximumDeltaVelocity, maximumDeltaVelocity); // then we apply a force towards our target velocity.

        // We have to transform the local force vector to world space or the character would not move according to its own orientation.
        Vector3 forceToApply = transform.TransformVector(localForceToApply);

        rigidbody.AddForce(forceToApply, ForceMode.VelocityChange);
    }

    /// <summary>
    /// Imagine the given vector would lie inside a square with its center at (0,0). Now imagine a circle at the same position that perfectly fits into this square.
    /// This function returns a scaled-down version of the given vector that transforms it from the square domain to the circle domain.
    /// It can be used to transform playor input from a square domain to a circle domain to prevent strafing diagonally to move faster.
    /// </summary>
    /// <param name="squareDomainVector"></param>
    /// <returns></returns>
    Vector2 TransformSquareDomainVectorToCircleDomain(Vector2 squareDomainVector)
    {
        Vector2 circleDomainVector = squareDomainVector;

        float transformationDividend = ProjectVectorOntoUnitSquareBounds(squareDomainVector).magnitude;

        if (transformationDividend != 0)
            circleDomainVector = squareDomainVector / transformationDividend;

        return circleDomainVector;
    }

    /// <summary>
    /// Imagine a square with the bounds (-1, -1), (-1, 1), (1, -1), and (1, 1), and some two dimensional vector. This function returns a vector with the same direction, but which lies on the bounds of this square.
    /// </summary>
    /// <param name="vectorToProject"></param>
    /// <returns></returns>
    Vector2 ProjectVectorOntoUnitSquareBounds(Vector2 vectorToProject)
    {
        Vector2 projectedVector = Vector2.zero;

        if (vectorToProject != Vector2.zero)
        {
            float xAbs = Mathf.Abs(vectorToProject.x);
            float yAbs = Mathf.Abs(vectorToProject.y);

            // First we determine if the vectorToProject is further away from the unit square in the x dimension or in the y dimension. If we divide the vector by the distance in that dimension, we have projected the vector to the square bounds.
            if (xAbs <= yAbs && yAbs != 0)
                projectedVector = vectorToProject / yAbs;
            else // Don't need to check here if xAbs != 0 since either 1) yAbs is 0 but not both are zero, meaning that xAbs is not zero, or 2) zAbs was smaller than xAbs which means that it has to be greater than zero
                projectedVector = vectorToProject / xAbs;
        }

        return projectedVector;
    }

    float CalculateJumpingForceToReachHeight(float height) => Mathf.Sqrt(2 * height * -Physics.gravity.y);

    bool IsInContactWithGround() => Physics.CheckSphere(groundCheckOrigin.transform.position, groundCheckRadius, groundObjectLayers);
}
