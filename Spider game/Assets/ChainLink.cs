using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLink : MonoBehaviour
{
    public Transform TopConnectionPoint;
    public Transform BottomConnectionPoint;

    public Vector3 GetTopConnectionPointOffset() => TopConnectionPoint.transform.position - transform.position;

    public Vector3 GetBottomConnectionPointOffset() => BottomConnectionPoint.transform.position - transform.position;

    public void AttachTo(ChainLink linkToAttachTo)
    {
        transform.position = linkToAttachTo.transform.position + linkToAttachTo.GetBottomConnectionPointOffset() - linkToAttachTo.GetTopConnectionPointOffset();
        transform.rotation = linkToAttachTo.transform.rotation;

        GetComponent<Rigidbody>().velocity = linkToAttachTo.GetComponent<Rigidbody>().velocity;
        GetComponent<Joint>().connectedBody = linkToAttachTo.GetComponent<Rigidbody>();
    }
}
