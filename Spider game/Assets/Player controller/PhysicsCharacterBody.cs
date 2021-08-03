using AnsgarsAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnsgarsAssets
{
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicsCharacterBody : MonoBehaviour
    {
        [Header("Ground movement")]
        public float walkingMaximumSpeed;
        public float walkingAcceleration;

        [Space(10)]
        public float sprintingMaximumSpeed;
        public float sprintingAcceleration;

        [Header("Air movement")]
        public float airMaximumSpeed;
        public float airAcceleration;
        [Tooltip("When activated, the character stops moving in the air when no input is given.")]
        public bool automaticAirMotionStop = true;

        [Header("Jumping")]
        public float jumpHeight;

        [Header("Ground checking")]
        public float groundCheckRadius;
        public LayerMask groundObjectLayers;
        public Transform groundCheckOrigin;

        Vector2 movementInput;
        bool isSprinting = false;

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

        public void ActivateAutomaticAirMotionStop() => automaticAirMotionStop = true;

        public void DeactivateAutomaticAirMotionStop() => automaticAirMotionStop = false;

        public void JumpIfGrounded()
        {
            if (IsInContactWithGround())
                Jump();
        }

        public void SetMovementInput(Vector2 input)
        {
            movementInput = input;
        }

        public void SetAsSprinting()
        {
            isSprinting = true;
        }

        public void SetAsNotSprinting()
        {
            isSprinting = false;
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
                if (isSprinting)
                {
                    maximumSpeed = sprintingMaximumSpeed;
                    maximumDeltaVelocity = sprintingAcceleration * Time.fixedDeltaTime;
                }
                else
                {
                    maximumSpeed = walkingMaximumSpeed;
                    maximumDeltaVelocity = walkingAcceleration * Time.fixedDeltaTime;
                }
            }
            else
            {
                maximumSpeed = airMaximumSpeed;
                maximumDeltaVelocity = airAcceleration * Time.fixedDeltaTime;
            }

            // Scale down the input vector such that its magnitude is capped at 1, which prevents speed strafing
            Vector2 adjustedMovementInput = Utility.TransformSquareDomainVectorToCircleDomain(movementInput);

            Vector3 targetVelocity = (transform.right * adjustedMovementInput.x + transform.forward * adjustedMovementInput.y) * maximumSpeed;

            Vector3 forceToApply = targetVelocity - GetRigidbody().velocity;

            // If the character is in the air, and the force to apply would slow them down in the direction they want to go, then use the force to steer the character in that direction instead.
            if (!IsInContactWithGround() && 90 < Vector3.Angle(targetVelocity, forceToApply))
                forceToApply = Vector3.Project(GetRigidbody().velocity, targetVelocity.normalized) - GetRigidbody().velocity;

            // Cap the amount of force depending on the specified acceleration
            if (maximumDeltaVelocity < forceToApply.magnitude)
                forceToApply = forceToApply.normalized * maximumDeltaVelocity;

            forceToApply.y = 0;

            rigidbody.AddForce(forceToApply, ForceMode.VelocityChange);
        }

        float CalculateJumpingForceToReachHeight(float height) => Mathf.Sqrt(2 * height * -Physics.gravity.y);

        bool IsInContactWithGround() => Physics.CheckSphere(groundCheckOrigin.transform.position, groundCheckRadius, groundObjectLayers);
    }

}