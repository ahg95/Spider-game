using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public RopeGun ropeGun;
    public PhysicsCharacterBody characterBody;

    // Update is called once per frame
    void Update()
    {
        // Rope gun input

        if (Input.GetKeyDown(KeyCode.Mouse0))
            ropeGun?.StartPressingTrigger();
        else if (Input.GetKeyUp(KeyCode.Mouse0))
            ropeGun?.StopPressingTrigger();

        if (Input.GetKeyDown(KeyCode.R))
            ropeGun?.PressReloadButton();

        if (Input.GetKeyDown(KeyCode.E))
            ropeGun?.PressAttachButton();

        // Character movement input

        if (Input.GetKeyDown(KeyCode.Space))
            characterBody?.AttemptJump();

        characterBody?.SetMovementInput(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
    }
}
