using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace AnsgarsAssets
{
    [RequireComponent(typeof(DeactivatableFixedJoint), typeof(ParentConstraint))]
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

        ParentConstraint parentConstraint;

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

        public ParentConstraint GetParentConstraint()
        {
            if (!parentConstraint)
                parentConstraint = GetComponent<ParentConstraint>();

            return parentConstraint;
        }

        public void EnableStickiness() => stickOnTouch = true;

        public void DisableStickiness()
        {
            GetDeactivatableFixedJoint().Deactivate();
            GetParentConstraint().constraintActive = false;

            stickOnTouch = false;
            isStickingToSomething = false;
        }

        public void StickTo(GameObject gameObjectToStickTo)
        {
            // The gameObjectToStickTo might not have a Rigidbody attached. If it doesn't, the connectedBody of the FixedJoint will be null, which means that this object will stick in space.
            Rigidbody rigidbodyToStickTo = gameObjectToStickTo.GetComponent<Rigidbody>();

            if (rigidbodyToStickTo)
            {
                GetDeactivatableFixedJoint().Activate();
                GetDeactivatableFixedJoint().GetJoint().connectedBody = rigidbodyToStickTo;
            } else
            {
                // Reset the ParentConstraint by removing all previous sources
                while (0 < GetParentConstraint().sourceCount)
                    GetParentConstraint().RemoveSource(0);

                GetParentConstraint().constraintActive = true;

                ConstraintSource sourceToAdd = new ConstraintSource();
                sourceToAdd.sourceTransform = gameObjectToStickTo.transform;
                sourceToAdd.weight = 1;

                GetParentConstraint().AddSource(sourceToAdd);

                GetParentConstraint().SetTranslationOffset(0, transform.position - gameObjectToStickTo.transform.position);
            }

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