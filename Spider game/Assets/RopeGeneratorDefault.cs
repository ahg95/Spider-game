using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeGeneratorDefault : RopeGenerator
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            GenerateRope(Vector3.up * 10, 5);
    }

    public override GameObject GenerateRope(Vector3 position, int length)
    {
        GameObject ropeLinkParent = null;

        if (length != 0)
        {
            ropeLinkParent = new GameObject();
            ropeLinkParent.transform.position = position;

            GameObject previouslySpawnedLink = null;

            for (int i = 0; i < length; i++)
            {
                Vector3 linkSpawnPosition;
                if (previouslySpawnedLink == null)
                    linkSpawnPosition = ropeLinkParent.transform.position;
                else
                    linkSpawnPosition = previouslySpawnedLink.transform.position + RopeLink.GetLinkOffsetVector();

                GameObject spawnedLink = Instantiate(RopeLink.gameObject, linkSpawnPosition, Quaternion.identity, ropeLinkParent.transform);

                if (previouslySpawnedLink == null)
                    Destroy(spawnedLink.GetComponent<Joint>());
                else
                    spawnedLink.GetComponent<RopeLink>().SetConnectedLink(previouslySpawnedLink.GetComponent<RopeLink>());

                previouslySpawnedLink = spawnedLink;
            }
        }

        return ropeLinkParent;
    }
}
