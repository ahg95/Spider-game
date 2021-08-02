using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RespawnTrigger : MonoBehaviour
{
    [Tooltip("The Transform that should be moved to the respawn point when Respawn() is called. If none is specified, this is the transform that this component is attached to.")]
    public Transform transformToRespawn;

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
        if (respawnPoint)
        {
            if (transformToRespawn)
                transformToRespawn.position = respawnPoint.position;
            else
                transform.position = respawnPoint.position;
        }
    }

    void DestroyCurrentRespawnPointIfItWasInstantiated()
    {
        if (respawnPointWasInstantiatedByThisComponent)
            Destroy(respawnPoint.gameObject);
    }
}
