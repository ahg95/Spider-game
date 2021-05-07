using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeGeneratorDefault : RopeGenerator
{
    public override GameObject GenerateRopeWithLength(int length)
    {
        GameObject rope = null;

        if (length != 0)
        {
            for (int i = 0; i < length; i++)
            {
                //GameObject.Instantiate
            }
        }

        return rope;
    }
}
