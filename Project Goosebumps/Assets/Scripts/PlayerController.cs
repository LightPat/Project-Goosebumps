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
    public float sensitivity;

    //[Header("Developer Options")]
    //public float walkingSpeed;

    private Rigidbody rb;
    private float currentSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentSpeed = 5f;
    }

    void FixedUpdate()
    {
        // Need to add rotation here maybe?
        newPosition = transform.position + new Vector3(input.x, 0, input.y) * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    private Vector3 newPosition;
    private Vector2 input;
    void OnMove(InputValue value)
    {
        input = value.Get<Vector2>();
    }

    private Vector3 FPScameraRotationEulers;
    void OnLook(InputValue value)
    {
    }
}
