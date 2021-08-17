using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class DeactivatableJoint<T> : MonoBehaviour where T : Joint
{
    T joint;

    public T GetJoint()
    {
        return joint;
    }

    public void Activate()
    {
        if (!joint)
            joint = gameObject.AddComponent<T>();
    }

    public void Deactivate()
    {
        if (joint)
            Destroy(joint);
    }
}
