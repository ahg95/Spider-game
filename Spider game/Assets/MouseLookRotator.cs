using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLookRotator : MonoBehaviour
{
    [SerializeField]
    float mouseSensititvity;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalRotationAngle = Input.GetAxis("Mouse X") * mouseSensititvity * Time.deltaTime;
        float verticalRotationAngle = - Input.GetAxis("Mouse Y") * mouseSensititvity * Time.deltaTime;

        transform.Rotate(Vector3.right * verticalRotationAngle, Space.Self);
        transform.Rotate(Vector3.up * horizontalRotationAngle, Space.World);
    }
}
