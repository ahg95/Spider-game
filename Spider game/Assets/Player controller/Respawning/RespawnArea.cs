using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RespawnArea : MonoBehaviour
{
    [Tooltip("The position where the respawnable will be moved to when respawned. If not set, the respawn point will be the transform that this component is attached to.")]
    public Transform respawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        RespawnTrigger respawnTrigger = other.GetComponent<RespawnTrigger>();

        Debug.Log("Intruded area.");

        if (respawnTrigger)
        {
            respawnTrigger.SetRespawnPoint(respawnPoint);
            Debug.Log("Activated spawn area.");
        }
    }
}
