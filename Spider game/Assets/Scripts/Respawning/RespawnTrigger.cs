using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RespawnTrigger : MonoBehaviour
{
    [Tooltip("This should be the prefab that should be spawned when this GameObject is destroyed.")]
    public GameObject respawnPrefab;

    Transform respawnPoint;

    bool respawnPointWasInstantiatedByThisComponent;

    public void SetRespawnPoint(Transform point)
    {
        DestroyCurrentRespawnPointIfItWasInstantiated();

        respawnPoint = point;
        respawnPointWasInstantiatedByThisComponent = false;
    }

    public void SetRespawnPosition(Vector3 position)
    {
        DestroyCurrentRespawnPointIfItWasInstantiated();

        respawnPoint = new GameObject("Respawn position of " + gameObject.name).transform;
        respawnPointWasInstantiatedByThisComponent = true;
    }

    public void RespawnIfPossible()
    {
        // Instantiation has to happen before destruction because otherwise all components of the player are disabled, and I don't completely understand why.
        if (respawnPoint)
            Instantiate(respawnPrefab, respawnPoint.position, respawnPoint.rotation);

        Destroy(gameObject);
    }

    void DestroyCurrentRespawnPointIfItWasInstantiated()
    {
        if (respawnPointWasInstantiatedByThisComponent)
            Destroy(respawnPoint.gameObject);
    }
}
