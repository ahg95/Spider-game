using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsCharacterController : MonoBehaviour
{
    public float movementSpeed;

    public float jumpHeight;
    public float groundCheckRadius;
    public LayerMask groundObjectLayers;
    public Transform groundCheckOrigin;

    // Input
    float vertInput;
    float horiInput;

    new Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        vertInput = Input.GetAxisRaw("Vertical");
        horiInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && IsInContactWithGround())
            Jump();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(groundCheckOrigin.position, groundCheckRadius);
    }

    private void FixedUpdate()
    {
        MoveAccordingToWASDInput();
    }

    private void Jump()
    {
        Vector3 jumpingForce = Vector3.up * CalculateJumpingForceToReachHeight(jumpHeight);

        rigidbody.AddForce(jumpingForce, ForceMode.VelocityChange);
    }

    private void MoveAccordingToWASDInput()
    {
        Vector3 movementVector = transform.forward * vertInput + transform.right * horiInput;
        movementVector *= Time.fixedDeltaTime * movementSpeed;

        rigidbody.MovePosition(transform.position + movementVector);
    }

    private float CalculateJumpingForceToReachHeight(float height) => Mathf.Sqrt(2 * height * -Physics.gravity.y);

    private bool IsInContactWithGround() => Physics.CheckSphere(groundCheckOrigin.transform.position, groundCheckRadius, groundObjectLayers);
}
