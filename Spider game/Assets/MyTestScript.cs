using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTestScript : MonoBehaviour
{
    public Transform target;

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Vector3 targetDirection = target.position - transform.position;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, targetDirection);
        }
    }
}
