using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class RopeGunGrappler : MonoBehaviour
{
    public GameEvent grapplerConnectedWithSomething;

    private void OnValidate()
    {
        if (gameObject.layer != 10)
            Debug.LogWarning("A gameObject with the script RopeGunGrappler is not on the layer 'Grapple'");
    }

    bool isConnectedToSomeGameObject = false;

    private void OnCollisionEnter(Collision collision)
    {
        ConnectToGameObject(collision.gameObject);
        //Debug.Log("collided");
    }

    void ConnectToGameObject(GameObject gameObjectToConnectTo)
    {
        FixedJoint joint = GetComponent<FixedJoint>();

        if (!joint)
            joint = gameObject.AddComponent<FixedJoint>();

        Rigidbody rigidbodyToConnectWith = gameObjectToConnectTo.GetComponent<Rigidbody>();

        if (rigidbodyToConnectWith)
            joint.connectedBody = rigidbodyToConnectWith;

        isConnectedToSomeGameObject = true;
        grapplerConnectedWithSomething.Raise();
    }

    bool IsConnectedToSomeGameObject()
    {
        return isConnectedToSomeGameObject;
    }
}
