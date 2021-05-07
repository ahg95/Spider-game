using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RopeLink : MonoBehaviour
{
    public abstract void SetConnectedLink(RopeLink link);

    public abstract Vector3 GetLinkOffsetVector();


}
