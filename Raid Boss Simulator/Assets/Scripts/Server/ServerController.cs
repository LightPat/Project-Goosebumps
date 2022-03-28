using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

namespace LightPat.Server
{
    public class ServerController : NetworkBehaviour
    {
        public GameObject EscapeMenuPrefab;

        [Header("Camera Settings")]
        public int moveSpeed;
        public int sprintSpeed;
        public float sensitivity;

        [Header("Network Objects to Spawn")]
        public GameObject[] objectsToSpawn;
        public Vector3[] spawnPositions;

        private int currentSpeed;

        private void Start()
        {
            currentSpeed = moveSpeed;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            transform.Translate(new Vector3(moveInput.x, 0, moveInput.y) * currentSpeed * Time.deltaTime);

            transform.Rotate(new Vector3(-lookInput.y, lookInput.x, 0) * sensitivity * Time.deltaTime);

            Quaternion q = transform.rotation;
            q.eulerAngles = new Vector3(q.eulerAngles.x, q.eulerAngles.y, 0);
            transform.rotation = q;
        }

        public void SpawnObjects()
        {
            if (!IsServer) { return; }

            if (spawnPositions.Length != objectsToSpawn.Length)
            {
                Debug.Log("You must associate each object with a spawn position. Check your server gameObject.");
                return;
            }

            for (int i = 0; i < objectsToSpawn.Length; i++)
            {
                GameObject spawned = Instantiate(objectsToSpawn[i], spawnPositions[i], Quaternion.identity);
                spawned.GetComponent<NetworkObject>().Spawn();
            }
        }

        private Vector2 moveInput;
        void OnMove(InputValue value)
        {
            moveInput = value.Get<Vector2>();
        }

        private Vector2 lookInput;
        void OnLook(InputValue value)
        {
            lookInput = value.Get<Vector2>();
        }

        void OnSprint(InputValue value)
        {
            if (value.isPressed)
            {
                currentSpeed = sprintSpeed;
            }
            else
            {
                currentSpeed = moveSpeed;
            }
        }

        private GameObject canvas;
        void OnEscapeToggle()
        {
            PlayerInput playerInput = GetComponent<PlayerInput>();

            if (playerInput.currentActionMap.name == "Server")
            {
                canvas = GameObject.Instantiate(EscapeMenuPrefab);

                Cursor.lockState = CursorLockMode.None;
                playerInput.SwitchCurrentActionMap("Escape");
            }
            else if (playerInput.currentActionMap.name == "Escape")
            {
                Destroy(canvas);

                Cursor.lockState = CursorLockMode.Locked;
                playerInput.SwitchCurrentActionMap("Server");
            }
        }
    }
}