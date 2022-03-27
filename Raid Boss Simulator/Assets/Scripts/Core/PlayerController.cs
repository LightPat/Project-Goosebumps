using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using TMPro;
using LightPat.Core.WeaponSystem;

namespace LightPat.Core
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(WeaponLoadout))]
    public class PlayerController : NetworkBehaviour
    {
        /* Most of the variables are associated with a specific method,
         * so I declare variables above the methods they are
         * associated with, instead of all of them at the top.
         * This prevents clutter at the top of the script and makes it
         * more readable. */

        public GameObject EscapeMenu;

        [Header("Input Settings")]
        public float sensitivity = 15f;

        private WeaponLoadout weaponLoadout;
        private GameObject firstPersonCamera;
        private Rigidbody rb;
        private Animator animator;

        [HideInInspector]
        public Transform verticalRotate;

        void Start()
        {
            if (IsClient)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            verticalRotate = transform.Find("Vertical Rotate");
            weaponLoadout = GetComponent<WeaponLoadout>();
            firstPersonCamera = verticalRotate.Find("First Person Camera").gameObject;
            currentSpeed = walkingSpeed;

            // Sets the camera and input to active on the player object that this network instance owns
            if (IsOwner)
            {
                verticalRotate.Find("First Person Camera").gameObject.SetActive(true);
                GetComponent<PlayerInput>().enabled = true;
                firstPersonCamera.tag = "MainCamera";
            }
            else
            {
                transform.Find("HUD").gameObject.SetActive(false);
            }

            rb = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
        }

        // This method is called in NetworkUI, only by the server
        public void updateName(string newName)
        {
            name = newName;
            transform.Find("Player Name").gameObject.GetComponent<TextMeshPro>().SetText(newName);

            updateNameClientRpc(newName);
        }

        [ClientRpc]
        void updateNameClientRpc(string newName)
        {
            name = newName;
            transform.Find("Player Name").gameObject.GetComponent<TextMeshPro>().SetText(newName);
        }

        void Update()
        {
            if (!IsOwner) { return; }

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

            Quaternion newRotation = Quaternion.Euler(0, lookEulers.x, 0);
            rb.MoveRotation(newRotation);

            if (oldRotation != newRotation)
            {
                RotateServerRpc(lookEulers);
            }

            verticalRotate.rotation = Quaternion.Euler(-lookEulers.y, lookEulers.x, 0);

            // Full auto firing
            GameObject w = weaponLoadout.getEquippedWeapon();
            if (w != null)
            {
                if (attackHeld)
                {
                    if (w.GetComponent<Weapon>().fullAuto)
                    {
                        w.GetComponent<Weapon>().attack();
                        AttackServerRpc();
                    }
                }
            }

            animator.SetBool("Airborne", !isGrounded());
            UpdateAnimationStateServerRpc("Airborne", !isGrounded());
        }

        void FixedUpdate()
        {
            newPosition = transform.position + rb.rotation * new Vector3(moveInput.x, 0, moveInput.y) * currentSpeed * NetworkManager.Singleton.LocalTime.FixedDeltaTime;

            // Send position update to server
            if (newPosition != transform.position)
            {
                MoveServerRpc(newPosition);
            }

            rb.MovePosition(newPosition);

            // Falling Gravity velocity increase
            if (rb.velocity.y < 0)
            {
                rb.AddForce(new Vector3(0, (fallingGravityScale * -1), 0), ForceMode.VelocityChange);
            }
        }

        [Header("Move Settings")]
        public float walkingSpeed = 5f;
        private Vector2 moveInput;
        private Vector3 newPosition;
        private float currentSpeed;
        void OnMove(InputValue value)
        {
            if (value.Get<Vector2>() != Vector2.zero)
            {
                animator.SetBool("Walk", true);
                UpdateAnimationStateServerRpc("Walk", true);
            }
            else
            {
                animator.SetBool("Walk", false);
                UpdateAnimationStateServerRpc("Walk", false);
            }

            moveInput = value.Get<Vector2>();
        }

        [ServerRpc]
        void MoveServerRpc(Vector3 newPosition)
        {
            transform.position = newPosition;
            MoveClientRpc(newPosition);
        }

        [ClientRpc]
        void MoveClientRpc(Vector3 newPosition)
        {
            if (IsLocalPlayer) { return; }

            transform.position = newPosition;
        }

        [Header("Look Settings")]
        public float verticalLookBound = 90f;
        private Quaternion oldRotation;
        private Vector3 lookEulers;
        private Vector2 lookInput;
        void OnLook(InputValue value)
        {
            lookInput = value.Get<Vector2>();
        }

        [ServerRpc]
        void RotateServerRpc(Vector3 newRotationEulers)
        {
            rb.MoveRotation(Quaternion.Euler(0, newRotationEulers.x, 0));
            verticalRotate.rotation = Quaternion.Euler(-newRotationEulers.y, newRotationEulers.x, 0);
            RotateClientRpc(newRotationEulers);
        }

        [ClientRpc]
        void RotateClientRpc(Vector3 newRotationEulers)
        {
            if (IsLocalPlayer) { return; }

            rb.MoveRotation(Quaternion.Euler(0, newRotationEulers.x, 0));
            verticalRotate.rotation = Quaternion.Euler(-newRotationEulers.y, newRotationEulers.x, 0);
        }

        [Header("Is Grounded")]
        public float checkDistance = 2f;
        bool isGrounded()
        {
            RaycastHit hit;
            // Raycast any gameObject that is beneath the collider
            Vector3 checkPosition = transform.position;
            checkPosition.y += 1;

            bool bHit = Physics.Raycast(checkPosition, transform.up * -1, out hit, checkDistance);

            return bHit;
        }

        [Header("Jump Settings")]
        public float jumpHeight = 3f;
        public float fallingGravityScale = 0.5f;
        void OnJump(InputValue value)
        {
            // TODO this isn't really an elegant solution, if you stand on the edge of something it doesn't realize that you are still grouded
            // If you check for velocity = 0 then you can double jump since the apex of your jump's velocity is 0
            // Check if the player is touching a gameObject under them
            // May need to change 1.5f to be a different number if you switch the asset of the player model

            if (value.isPressed)
            {
                if (isGrounded())
                {
                    float jumpForce = Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y);

                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                    {
                        StartCoroutine(IdleJump(jumpForce));
                    }
                    else
                    {
                        rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.VelocityChange);
                    }

                    animator.SetBool("Jump", true);
                    JumpServerRpc(jumpForce);
                }
            }
            else
            {
                animator.SetBool("Jump", false);
            }
        }

        IEnumerator IdleJump(float jumpForce)
        {
            yield return new WaitForSeconds(0.5f);
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.VelocityChange);
        }

        [ServerRpc]
        void JumpServerRpc(float jumpForce)
        {
            if (!IsHost)
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    StartCoroutine(IdleJump(jumpForce));
                }
                else
                {
                    rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.VelocityChange);
                }
            }

            JumpClientRpc(jumpForce);
        }

        [ClientRpc]
        void JumpClientRpc(float jumpForce)
        {
            if (IsLocalPlayer) { return; }

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                StartCoroutine(IdleJump(jumpForce));
            }
            else
            {
                rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.VelocityChange);
            }
        }

        [Header("Crouch Settings")]
        public float crouchSpeed = 2f;
        void OnCrouch(InputValue value)
        {
            if (value.isPressed)
            {
                currentSpeed = crouchSpeed;
                animator.SetBool("Crouch", true);
                UpdateAnimationStateServerRpc("Crouch", true);
            }
            else
            {
                currentSpeed = walkingSpeed;
                animator.SetBool("Crouch", false);
                UpdateAnimationStateServerRpc("Crouch", false);
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
                if (hit.collider.tag == "Weapon")
                {
                    weaponLoadout.addItem(hit.collider.gameObject);
                }
                else if (hit.collider.tag == "Interactable")
                {
                    // TODO Only call interactable change on the server, may need to change later
                    InteractableServerRpc(hit.transform.gameObject.GetComponent<NetworkObject>().NetworkObjectId);
                }
            }
        }

        [ServerRpc]
        void InteractableServerRpc(ulong targetId)
        {
            GameObject[] interactableObjects = GameObject.FindGameObjectsWithTag("Interactable");

            foreach (GameObject g in interactableObjects)
            {
                if (g.GetComponent<NetworkObject>().NetworkObjectId == targetId)
                {
                    g.GetComponent<Interactable>().Invoke();
                    break;
                }
            }
        }

        private bool attackHeld;
        void OnAttack(InputValue value)
        {
            GameObject equippedWeapon = weaponLoadout.getEquippedWeapon();

            // Fire if the weapon isn't full auto
            // Full auto firing is handled in update()
            if (value.isPressed)
            {
                if (equippedWeapon != null)
                {
                    if (!equippedWeapon.GetComponent<Weapon>().fullAuto)
                    {
                        equippedWeapon.GetComponent<Weapon>().attack();
                        AttackServerRpc();
                    }
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

        [ServerRpc]
        void AttackServerRpc()
        {
            weaponLoadout.getEquippedWeapon().GetComponent<Weapon>().attack();
            AttackClientRpc();
        }

        [ClientRpc]
        void AttackClientRpc()
        {
            if (IsLocalPlayer) { return; }

            weaponLoadout.getEquippedWeapon().GetComponent<Weapon>().attack();
        }

        private GameObject canvas;
        private string lastStateName;
        void OnEscapeToggle()
        {
            PlayerInput playerInput = GetComponent<PlayerInput>();

            switch (playerInput.currentActionMap.name)
            {
                case "First Person":
                    weaponLoadout.HUDCanvas.SetActive(false);

                    canvas = GameObject.Instantiate(EscapeMenu);

                    Cursor.lockState = CursorLockMode.None;

                    lastStateName = playerInput.currentActionMap.name;
                    playerInput.SwitchCurrentActionMap("Escape");
                    break;

                case "Escape":
                    Destroy(canvas);

                    if (lastStateName == "Inventory")
                    {
                        weaponLoadout.GUICanvas.SetActive(true);
                        Cursor.lockState = CursorLockMode.None;
                    }
                    else
                    {
                        weaponLoadout.HUDCanvas.SetActive(true);
                        Cursor.lockState = CursorLockMode.Locked;
                    }

                    playerInput.SwitchCurrentActionMap(lastStateName);
                    break;

                case "Inventory":
                    weaponLoadout.GUICanvas.SetActive(false);

                    canvas = GameObject.Instantiate(EscapeMenu);

                    lastStateName = playerInput.currentActionMap.name;
                    playerInput.SwitchCurrentActionMap("Escape");
                    break;
            }
        }

        [ServerRpc]
        void UpdateAnimationStateServerRpc(string stateName, bool value)
        {
            animator.SetBool(stateName, value);
            UpdateAnimationStateClientRpc(stateName, value);
        }

        [ClientRpc]
        void UpdateAnimationStateClientRpc(string stateName, bool value)
        {
            if (IsLocalPlayer) { return; }

            animator.SetBool(stateName, value);
        }
    }
}