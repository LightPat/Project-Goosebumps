using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Input Settings")]
    public float sensitivity;

    [Header("Developer Options")]
    public float walkingSpeed;

    private Rigidbody rb;
    private float currentSpeed;
    private Vector3 FPScameraRotationEulers;

    // For OnMove
    private Vector3 moveVector;
    private bool moveHeld;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentSpeed = 5f;
    }

    void FixedUpdate()
    {
        rb.MovePosition(moveVector);
    }

    void OnMove(InputValue value)
    {
        if (value.isPressed)
        {
            Debug.Log("Working");
        }
        else
        {
            Debug.Log("Released");
        }

        Vector2 input = value.Get<Vector2>();
        moveVector = transform.position + new Vector3(input.x, 0, input.y) * currentSpeed * Time.fixedDeltaTime;


    }

    void OnLook(InputValue value)
    {
    }
}
