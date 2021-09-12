using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace AnsgarsAssets
{
    [RequireComponent(typeof(Rigidbody), typeof(DeactivatableFixedJoint), typeof(Collider))]
    public class Sticky : MonoBehaviour
    {
        public LayerMask layersToStickToOnTouch;

        [Tooltip("If enabled, the object starts sticking to the next object it collides with.")]
        public bool stickOnTouch = false;

        [Tooltip("This event is raised when this object collided with an object it can stick to.")]
        public GameEvent stickedToAnObject;

        bool isStickingToSomething = false;

        new Rigidbody rigidbody;
        DeactivatableFixedJoint fixedJoint;
        Collider collider;

        public Rigidbody GetRigidbody()
        {
            if (!rigidbody)
                rigidbody = GetComponent<Rigidbody>();

            return rigidbody;
        }

        public DeactivatableFixedJoint GetDeactivatableFixedJoint()
        {
            if (!fixedJoint)
                fixedJoint = GetComponent<DeactivatableFixedJoint>();

            return fixedJoint;
        }

        public Collider GetCollider()
        {
            if (!collider)
                collider = GetComponent<Collider>();

            return collider;
        }

        public void EnableStickiness() => stickOnTouch = true;

        public void DisableStickiness()
        {
            GetDeactivatableFixedJoint().Deactivate();

            stickOnTouch = false;
            isStickingToSomething = false;
        }

        public void StickTo(GameObject gameObjectToStickTo)
        {
            // The gameObjectToStickTo might not have a Rigidbody attached. If it doesn't, the connectedBody of the FixedJoint will be null, which means that this object will stick in space.
            Rigidbody rigidbodyToStickTo = gameObjectToStickTo.GetComponent<Rigidbody>();

            // The following lines are supposed to prevent that the forces of the impact are continuously applied to the objects.
            // Setting these velocity values upon impact does apparently not influence the physics of the initial impact.
            GetRigidbody().velocity = Vector3.zero;
            GetRigidbody().angularVelocity = Vector3.zero;

            GetDeactivatableFixedJoint().Activate();
            GetDeactivatableFixedJoint().GetJoint().connectedBody = rigidbodyToStickTo;

            isStickingToSomething = true;
            stickedToAnObject?.Raise();
        }

        private void OnCollisionStay(Collision collision)
        {
            if (StickOnTouchIsEnabled() && !IsStickingToSomething() && Utility.LayerMaskContainsLayer(layersToStickToOnTouch, collision.gameObject.layer))
                StickTo(collision.gameObject);
        }

        bool StickOnTouchIsEnabled() => stickOnTouch;

        bool IsStickingToSomething() => isStickingToSomething;
    }
}