using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeLinkDefault : RopeLink
{
    public override Vector3 GetLinkOffsetVector()
    {
        return Vector3.down;
    }

    public override void SetConnectedLink(RopeLink link)
    {
        GetComponent<Joint>().connectedBody = link.GetComponent<Rigidbody>();
    }
}
