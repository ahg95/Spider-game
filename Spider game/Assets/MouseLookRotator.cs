using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLookRotator : MonoBehaviour
{
    [SerializeField]
    float mouseSensititvity;

    // Update is called once per frame
    void Update()
    {
        float horizontalRotationAngle = Input.GetAxis("Mouse X") * mouseSensititvity * Time.deltaTime;
        float verticalRotationAngle = Input.GetAxis("Mouse Y") * mouseSensititvity * Time.deltaTime;


        if (transform.localRotation.x + verticalRotationAngle < -90)
            verticalRotationAngle = -90 - transform.localRotation.x;
        else if (90 < transform.localRotation.x + verticalRotationAngle)
            verticalRotationAngle = 90 - transform.localRotation.x;


        transform.Rotate(Vector3.up * horizontalRotationAngle, Space.World);
    }
}
