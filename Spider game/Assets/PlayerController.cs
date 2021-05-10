using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public ProjectileShooter shooter;

    public ChainLink link;

    public ChainLink linkToAttachTo;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
            shooter.ShootIfCooledDown();

        if (Input.GetKeyDown(KeyCode.E))
        {
            GameObject spawnedLink = Instantiate(link.gameObject, Vector3.zero, Quaternion.identity);

            spawnedLink.GetComponent<ChainLink>().AttachToChainLinkHook(linkToAttachTo, transform.position);

            linkToAttachTo = spawnedLink.GetComponent<ChainLink>();
        }
    }

}
