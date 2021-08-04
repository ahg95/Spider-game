using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsOnButtonPress : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Vector3 targetDirection = target.position - transform.position;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, targetDirection);
        }
    }
}
