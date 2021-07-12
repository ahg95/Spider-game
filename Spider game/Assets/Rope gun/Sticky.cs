using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class Sticky : MonoBehaviour
{
    public GameEvent stickedToAnObject;

    bool stickinessEnabled = false;

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

    public void EnableStickiness() => stickinessEnabled = true;

    public void DisableStickiness()
    {
        stickinessEnabled = false;
        DestroyFixedJointIfExistent();
    }

    void DestroyFixedJointIfExistent()
    {
        if (GetFixedJoint())
            Destroy(GetFixedJoint());
    }

    bool StickinessIsEnabled() => stickinessEnabled;

    bool IsStickingToSomething() => isStickingToSomething;

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsStickingToSomething() && StickinessIsEnabled())
            StickTo(collision.gameObject);
    }

    void StickTo(GameObject gameObjectToStickTo)
    {
        Rigidbody rigidbodyToStickTo = gameObjectToStickTo.GetComponent<Rigidbody>(); // The gameObject might not have a Rigidbody attached. If it doesn't, the connectedBody of the FixedJoint will be null, which means that this object will stick in space.

        CreateFixedJointIfNotExistent();

        GetFixedJoint().connectedBody = rigidbodyToStickTo;

        isStickingToSomething = true;
        stickedToAnObject.Raise();
    }

    public void CreateFixedJointIfNotExistent()
    {
        if (!GetFixedJoint())
            fixedJoint = gameObject.AddComponent<FixedJoint>();
    }

    private void OnValidate()
    {
        if (gameObject.layer != 10)
            Debug.LogWarning("A gameObject with the script 'Sticky' is not on the layer 'Sticky'.");
    }
}
