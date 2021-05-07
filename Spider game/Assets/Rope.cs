using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Rope : MonoBehaviour
{
    public RopeLink DefaultRopeLink;

    RopeLink firstLink;
    RopeLink lastLink;





    public void AddLinkToFirstLink(RopeLink linkToAdd = null)
    {
        if (linkToAdd == null)
            linkToAdd = DefaultRopeLink;
    }

    public void AddLinkToLastLink(RopeLink linkToAdd = null)
    {
        if (linkToAdd == null)
            linkToAdd = DefaultRopeLink;
    }

    public void AddLinksToFirstLink(int numberOfLinks, RopeLink linkToAdd = null)
    {
        if (linkToAdd == null)
            linkToAdd = DefaultRopeLink;
    }

    public void AddLinksToLastLink(int numberOfLinks, RopeLink linkToAdd = null)
    {
        if (linkToAdd == null)
            linkToAdd = DefaultRopeLink;

        for (int i = 0; i < numberOfLinks; i++)
        {

        }
    }
}
