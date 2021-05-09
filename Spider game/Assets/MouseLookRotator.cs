using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLookRotator : MonoBehaviour
{
    [SerializeField]
    float mouseSensititvity;

    [SerializeField]
    Transform transformToRotateHorizontally;

    float localXRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        localXRotation = transform.localRotation.x;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensititvity * Time.deltaTime;
        transformToRotateHorizontally.rotation = transformToRotateHorizontally.rotation * Quaternion.Euler(0, mouseX, 0);

        float mouseY = Input.GetAxis("Mouse Y") * mouseSensititvity * Time.deltaTime;
        localXRotation = Mathf.Clamp(localXRotation - mouseY, -90, 90);
        transform.localRotation = Quaternion.Euler(localXRotation, 0, 0);

        //transform.Rotate(Vector3.right * -mouseY, Space.Self);
    }
}
