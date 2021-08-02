using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnsgarsAssets;

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

        if (Input.GetKeyDown(KeyCode.Mouse0))
            ropeGun?.StartPressingExpellRopeButton();
        else if (Input.GetKeyUp(KeyCode.Mouse0))
            ropeGun?.StopPressingExpellRopeButton();

        if (Input.GetKeyDown(KeyCode.Mouse1))
            ropeGun?.StartPressingTakeUpRopeButton();
        else if (Input.GetKeyUp(KeyCode.Mouse1))
            ropeGun?.StopPressingTakeUpRopeButton();

        // Character movement input

        if (Input.GetKeyDown(KeyCode.Space))
            characterBody?.JumpIfGrounded();

        characterBody?.SetMovementInput(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));

        if (Input.GetKeyDown(KeyCode.LeftShift))
            characterBody?.SetAsSprinting();
        else if (Input.GetKeyUp(KeyCode.LeftShift))
            characterBody?.SetAsNotSprinting();
    }
}
