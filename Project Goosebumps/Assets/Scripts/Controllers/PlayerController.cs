using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ItemSystem;
using Unity.Netcode;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Inventory))]
public class PlayerController : Controller
{
    /* Most of the variables are associated with a specific method,
     * so I declare variables above the methods they are
     * associated with, instead of all of them at the top.
     * This prevents clutter at the top of the script and makes it
     * more readable. */

    public GameObject EscapeMenu;

    [Header("Input Settings")]
    public float sensitivity = 15f;

    private Inventory inventory;
    private GameObject firstPersonCamera;
    
    private NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>();
    private NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>();

    new void Start()
    {
        // Call parent start method
        base.Start();
        inventory = GetComponent<Inventory>();
        firstPersonCamera = transform.Find("Vertical Rotate").Find("First Person Camera").gameObject;
        currentSpeed = walkingSpeed;

        Cursor.lockState = CursorLockMode.Locked;

        // Sets the camera and input to active on the player object that this network instance owns
        if (IsOwner)
        {
            transform.Find("Vertical Rotate").Find("First Person Camera").gameObject.SetActive(true);
            GetComponent<PlayerInput>().enabled = true;
        }
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

        // Rotate player horizontally
        Quaternion newRotation = Quaternion.Euler(0, lookEulers.x, 0);
        transform.rotation = newRotation;
        // Rotate vertical rotation object vertically and horizontally
        transform.Find("Vertical Rotate").rotation = Quaternion.Euler(-lookEulers.y, lookEulers.x, 0);

        // Full auto firing
        GameObject w = inventory.getEquippedWeapon();
        if (w != null)
        {
            if (attackHeld)
            {
                if (w.GetComponent<Weapon>().fullAuto)
                {
                    //Debug.Log("REached");
                    w.GetComponent<Weapon>().attack();
                }
            }
        }        
    }

    void FixedUpdate()
    {
        oldPosition = transform.position;
        // Updating player position from WASD input
        newPosition = transform.position + rb.rotation * new Vector3(moveInput.x, 0, moveInput.y) * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);

        // Falling Gravity velocity increase
        if (rb.velocity.y < 0)
        {
            rb.AddForce(new Vector3(0, (fallingGravityScale * -1), 0), ForceMode.VelocityChange);
        }

        // Send the position change to the server if there was a position update
        if (newPosition != oldPosition)
        {
            UpdatePlayerPositionServerRpc(transform.position);
        }
    }

    [Header("Is Grounded")]
    public float checkDistance = 2f;
    bool isGrounded()
    {
        RaycastHit hit;
        // Raycast any gameObject that is beneath the collider
        bool bHit = Physics.Raycast(transform.position, transform.up * -1, out hit, checkDistance);

        return bHit;

        // Sweep test or capsule collide later maybe as ideas to fix this
        //bHit = rb.SweepTest(-transform.up, out hit, 1.5f);

        //Debug.Log(hit.collider);
        //return bHit;
    }

    [ServerRpc]
    void UpdatePlayerPositionServerRpc(Vector3 newPosition)
    {
        // Update this player's position on the server
        networkPosition.Value = newPosition;

        if (IsServer)
        {
            UpdatePlayerPositionClientRpc();
        }
    }

    [ClientRpc]
    void UpdatePlayerPositionClientRpc()
    {
        // This code is executed on each client
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            // Update the player object if they aren't the local player, since the local player moves itself
            if (!player.GetComponent<NetworkObject>().IsLocalPlayer)
            {
                Vector3 newClientPosition = player.GetComponent<PlayerController>().networkPosition.Value;

                player.transform.position = newClientPosition;
            }
        }
    }

    [Header("Move Settings")]
    public float walkingSpeed = 5f;
    private Vector3 newPosition;
    private Vector3 oldPosition;
    private Vector2 moveInput;
    private float currentSpeed;
    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    [Header("Look Settings")]
    public float verticalLookBound = 90f;
    private Quaternion newRotation;
    private Vector3 lookEulers;
    private Vector2 lookInput;
    void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }
    
    [Header("Jump Settings")]
    public float jumpHeight = 3f;
    public float fallingGravityScale = 0.5f;
    void OnJump()
    {
        // TODO this isn't really an elegant solution, if you stand on the edge of something it doesn't realize that you are still grouded
        // If you check for velocity = 0 then you can double jump since the apex of your jump's velocity is 0
        // Check if the player is touching a gameObject under them
        // May need to change 1.5f to be a different number if you switch the asset of the player model
        

        // Jump logic
        if (isGrounded())
        {
            float jumpForce = Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y);
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.VelocityChange);

            // Send AddForce call here?
        }
    }

    [Header("Crouch Settings")]
    public float crouchSpeed = 2f;
    public float crouchHeight = 1.5f;
    void OnCrouch(InputValue value)
    {
        // Half the collider's height
        // GetComponent<Collider>().bounds.size.y/2

        if (value.isPressed)
        {
            // If crouch is pressed, shrink the player and change the speed
            // TODO Change this to animation later
            transform.Find("Model").localScale -= new Vector3(0, crouchHeight, 0);
            currentSpeed = crouchSpeed;
        }
        else
        {
            // If crouch is released, grow the player and revert the speed change
            transform.Find("Model").localScale += new Vector3(0, crouchHeight, 0);
            currentSpeed = walkingSpeed;
        }
    }

    [Header("Interact Settings")]
    public float reach = 4f;
    void OnInteract()
    {
        RaycastHit hit;
        // Raycast gameObject that we are looking at if it is in the range of our reach
        bool bHit = Physics.Raycast(firstPersonCamera.transform.position, firstPersonCamera.transform.forward, out hit, reach);

        // Have to check bHit in order to prevent a null reference exception when checking tags
        if (bHit)
        {
            if (hit.collider.tag == "InventoryItem")
            {
                inventory.addItem(hit.collider.gameObject);
            }
            else if (hit.collider.tag == "Interactable")
            {
                // TODO
                Debug.Log("Haven't implemented interactable logic yet in PlayerController.cs");
            }
        }
    }

    private bool attackHeld;
    void OnAttack(InputValue value)
    {
        GameObject equippedWeapon = inventory.getEquippedWeapon();

        // Fire if the weapon isn't full auto
        // Full auto firing is handled in update()
        if (equippedWeapon != null)
        {
            if (!equippedWeapon.GetComponent<Weapon>().fullAuto)
            {
                equippedWeapon.GetComponent<Weapon>().attack();
            }
        }

        if (value.isPressed)
        {
            attackHeld = true;
        }
        else
        {
            attackHeld = false;
        }
    }

    private GameObject canvas;
    private string lastStateName;
    void OnEscapeToggle()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();

        switch (playerInput.currentActionMap.name)
        {
            case "First Person":
                inventory.HUDCanvas.SetActive(false);

                canvas = GameObject.Instantiate(EscapeMenu);

                Cursor.lockState = CursorLockMode.None;

                lastStateName = playerInput.currentActionMap.name;
                playerInput.SwitchCurrentActionMap("Escape");
                break;

            case "Escape":
                Destroy(canvas);

                if (lastStateName == "Inventory")
                {
                    inventory.GUICanvas.SetActive(true);
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    inventory.HUDCanvas.SetActive(true);
                    Cursor.lockState = CursorLockMode.Locked;
                }

                playerInput.SwitchCurrentActionMap(lastStateName);
                break;

            case "Inventory":
                inventory.GUICanvas.SetActive(false);

                canvas = GameObject.Instantiate(EscapeMenu);

                lastStateName = playerInput.currentActionMap.name;
                playerInput.SwitchCurrentActionMap("Escape");
                break;
        }
    }
}