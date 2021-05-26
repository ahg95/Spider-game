using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralTestingScript : MonoBehaviour
{
    public ProjectileShooter shooter;



    public Transform rotationTarget;

    public ChainLink chainLinkPrefabToSpawn;

    public ChainLink linkToAttachTo;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && shooter)
            shooter.ShootIfCooledDown();

        if (Input.GetKeyDown(KeyCode.E))
        {
            GameObject spawnedLink = Instantiate(chainLinkPrefabToSpawn.gameObject);

            spawnedLink.GetComponent<ChainLink>().AttachToChainLinkHookAndRotateTowards(linkToAttachTo, rotationTarget.position);
            //spawnedLink.GetComponent<ChainLink>().AttachToChainLinkHook(linkToAttachTo);

            linkToAttachTo = spawnedLink.GetComponent<ChainLink>();
        }
    }

}
