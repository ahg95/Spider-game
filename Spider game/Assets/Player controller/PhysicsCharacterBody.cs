using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsCharacterBody : MonoBehaviour
{
    public float movementSpeed;

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
        //if (IsInContactWithGround())
        {
            Vector3 movementVector = transform.forward * amountToMoveForward + transform.right * amountToMoveRight;
            movementVector *= Time.fixedDeltaTime * movementSpeed;

            GetRigidbody().MovePosition(transform.position + movementVector);
        }



    }

    float CalculateJumpingForceToReachHeight(float height) => Mathf.Sqrt(2 * height * -Physics.gravity.y);

    bool IsInContactWithGround() => Physics.CheckSphere(groundCheckOrigin.transform.position, groundCheckRadius, groundObjectLayers);
}
