using AnsgarsAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnsgarsAssets
{
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicsCharacterController : MonoBehaviour
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

            // Select the correct maximumSpeed and maximumDeltaVelocity numbers based on the current state of the character
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

            Vector3 forceToApply;

            // If the character is in the air, and the character should not automatically stop in the air when no movement input is given, and no movement input is given ...
            if (!IsInContactWithGround() && !automaticAirMotionStop && movementInput == Vector2.zero)
                // ... then don't apply a force, which would stop the player
                forceToApply = Vector3.zero;
            else
            {
                // Scale down the input vector such that its magnitude is capped at 1, which prevents speed strafing
                Vector2 adjustedMovementInput = Utility.TransformSquareDomainVectorToCircleDomain(movementInput);

                // Calculate which velocity the character intends to have
                Vector3 targetVelocity = (transform.right * adjustedMovementInput.x + transform.forward * adjustedMovementInput.y) * maximumSpeed;

                // We should apply a force that pushes the character from the current velocity towards the desired velocity
                forceToApply = targetVelocity - GetRigidbody().velocity;

                // However, if the character is in the air, and the force to apply would slow them down in the direction they want to go, then use the force to steer the character in that direction instead.
                if (!IsInContactWithGround() && 90 < Vector3.Angle(targetVelocity, forceToApply))
                    forceToApply = Vector3.Project(GetRigidbody().velocity, targetVelocity.normalized) - GetRigidbody().velocity;

                // To slowly transition from the current velocity to the desired velocity we cap the amount of force to apply
                if (maximumDeltaVelocity < forceToApply.magnitude)
                    forceToApply = forceToApply.normalized * maximumDeltaVelocity;

                // The movementInput should have no influence on the vertical position of the character
                forceToApply.y = 0;
            }

            rigidbody.AddForce(forceToApply, ForceMode.VelocityChange);
        }

        float CalculateJumpingForceToReachHeight(float height) => Mathf.Sqrt(2 * height * -Physics.gravity.y);

        bool IsInContactWithGround() => Physics.CheckSphere(groundCheckOrigin.transform.position, groundCheckRadius, groundObjectLayers);
    }

}