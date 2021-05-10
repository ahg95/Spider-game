using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour
{
    /*
    public ChainLink lastLinkInChain;

    public void SpawnAndAttachRopeLink(ChainLink chainLinkToSpawn)
    {
        GameObject spawnedGameObject = Instantiate(chainLinkToSpawn.gameObject);

        ChainLink spawnedChainLink = spawnedGameObject.GetComponent<ChainLink>();

        AttachRopeLink(spawnedChainLink);
    }

    private void AttachRopeLink(ChainLink chainLinkToAttach)
    {
        chainLinkToAttach.transform.position = lastLinkInChain.transform.position + lastLinkInChain.GetBottomConnectionPointOffset() - chainLinkToAttach.GetTopConnectionPointOffset();
        chainLinkToAttach.transform.rotation = lastLinkInChain.transform.rotation;
        chainLinkToAttach.GetComponent<Rigidbody>().velocity = lastLinkInChain.GetComponent<Rigidbody>().velocity;
    }
    */
}
