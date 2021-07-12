using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public RopeGun ropeGun;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
            ropeGun.StartPressingTrigger();
        else if (Input.GetKeyUp(KeyCode.Mouse1))
            ropeGun.StopPressingTrigger();

        if (Input.GetKeyDown(KeyCode.R))
            ropeGun.PressReloadButton();
    }
}
