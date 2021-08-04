using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnsgarsAssets
{
    [RequireComponent(typeof(Rigidbody))]
    public class Sticky : MonoBehaviour
    {
        [Tooltip("This event is raised when this object collided with an object it can stick to.")]
        public GameEvent stickedToAnObject;

        public LayerMask layersToStickTo;

        [Tooltip("Wether or not the grapple should connect with the next stickable object it collides with.")]
        public bool stickOnTouch = false;

        bool isStickingToSomething = false;

        new Rigidbody rigidbody;

        FixedJoint fixedJoint;

        public Rigidbody GetRigidbody()
        {
            if (!rigidbody)
                rigidbody = GetComponent<Rigidbody>();

            return rigidbody;
        }

        public FixedJoint GetFixedJoint()
        {
            if (!fixedJoint)
                fixedJoint = GetComponent<FixedJoint>();

            return fixedJoint;
        }

        public void EnableStickiness() => stickOnTouch = true;

        public void DisableStickiness()
        {
            stickOnTouch = false;
            DestroyFixedJointIfExistent();
            isStickingToSomething = false;
        }

        public void StickTo(GameObject gameObjectToStickTo)
        {
            Rigidbody rigidbodyToStickTo = gameObjectToStickTo.GetComponent<Rigidbody>(); // The gameObject might not have a Rigidbody attached. If it doesn't, the connectedBody of the FixedJoint will be null, which means that this object will stick in space.

            CreateFixedJointIfNotExistent();

            GetFixedJoint().connectedBody = rigidbodyToStickTo;

            isStickingToSomething = true;
            stickedToAnObject?.Raise();
        }

        void DestroyFixedJointIfExistent()
        {
            if (GetFixedJoint())
                Destroy(GetFixedJoint());
        }

        private void OnCollisionStay(Collision collision)
        {
            if (StickOnTouchIsEnabled() && !IsStickingToSomething() && Utility.LayerMaskContainsLayer(layersToStickTo, collision.gameObject.layer))
                StickTo(collision.gameObject);
        }

        bool StickOnTouchIsEnabled() => stickOnTouch;

        bool IsStickingToSomething() => isStickingToSomething;

        public void CreateFixedJointIfNotExistent()
        {
            if (!GetFixedJoint())
                fixedJoint = gameObject.AddComponent<FixedJoint>();
        }
    }
}