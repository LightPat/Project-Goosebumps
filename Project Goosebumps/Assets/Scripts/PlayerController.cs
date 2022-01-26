using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    /* Most of the private variables are associated with a method,
     * so I declare private variables above the methods they are
     * associated with, instead of all of them at the top.
     * This prevents clutter at the top of the script and makes it
     * more readable. */

    [Header("Input Settings")]
    public float sensitivity = 15f;

    [Header("Developer Options")]
    public float walkingSpeed = 5f;
    public float verticalLookBound = 90f;

    private Rigidbody rb;
    private GameObject firstPersonCamera;
    private float currentSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        firstPersonCamera = transform.Find("First Person Camera").gameObject;
    }

    void Update()
    {
        // Look with mouse
        // Remember that Rotate() rotates AROUND that axis, so if we want to look right, we rotate along the Y axis
        lookInput *= (sensitivity * Time.deltaTime);
        lookEulers.x += lookInput.x;

        // This prevents the rotation from increasing or decreasing infinitely if the player does a bunch of spins horizontally
        if (lookEulers.x >= 360)
        {
            lookEulers.x = lookEulers.x - 360;
        }
        else if (lookEulers.x <= -360)
        {
            lookEulers.x = lookEulers.x + 360;
        }

        /* First Person Camera Rotation Logic
        Remember that the camera is a child of the player, so we don't need to worry about horizontal rotation, that has already been calculated
        Calculate vertical rotation for the first person camera if you're not looking straight up or down already
        If we reach the top or bottom of our vertical look bound, set the total rotation amount to 1 over or 1 under the bound
        Otherwise, just change the rotation by the lookInput */
        if (lookEulers.y < -verticalLookBound)
        {
            lookEulers.y = -verticalLookBound - 1;

            if (lookInput.y > 0)
            {
                lookEulers.y += lookInput.y;
            }
        }
        else if (lookEulers.y > verticalLookBound)
        {
            lookEulers.y = verticalLookBound + 1;

            if (lookInput.y < 0)
            {
                lookEulers.y += lookInput.y;
            }
        }
        else
        {
            lookEulers.y += lookInput.y;
        }

        // Rotate the camera vertically, but not the player
        firstPersonCamera.transform.rotation = Quaternion.Euler(-lookEulers.y, lookEulers.x, 0);
    }

    void FixedUpdate()
    {
        // Rotation with mouse look
        Quaternion newRotation = Quaternion.Euler(0, lookEulers.x, 0);
        rb.MoveRotation(newRotation);

        // Updating player position from WASD input
        newPosition = transform.position + rb.rotation * new Vector3(moveInput.x, 0, moveInput.y) * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    private Vector3 newPosition;
    private Vector2 moveInput;
    void OnMove(InputValue value)
    {
        if (value.Get() == null) { currentSpeed = 0f; }

        currentSpeed = walkingSpeed;
        moveInput = value.Get<Vector2>();
    }

    private Quaternion newRotation;
    private Vector3 lookEulers;
    private Vector2 lookInput;
    void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }
}
