using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RopeGenerator : MonoBehaviour
{
    public RopeLink ropeLink;

    public abstract GameObject GenerateRopeWithLength(int length);

}
