using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RopeGenerator : MonoBehaviour
{
    public RopeLink RopeLink;

    public abstract GameObject GenerateRope(Vector3 position, int length);

}
