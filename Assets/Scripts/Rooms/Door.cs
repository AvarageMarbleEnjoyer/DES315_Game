using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    public Door linkedDoor;

    [Tooltip("Where the player is placed after teleporting through this door. Add a child GameObject and assign it here.")]
    public Transform spawnPoint;

    [Tooltip("How close the player must be to interact")]
    public float interactRange = 2f;

    [Header("Input")]
    [Tooltip("Leave empty to auto-find from PlayerController")]
    public InputActionAsset inputActions;

    private float cameraMoveIncrease = 3f;
    private InputAction interactAction;
    private Transform playerTransform;
    private NavMeshAgent playerAgent;
    private CameraController cameraController;
    private bool playerInRange;

    private void Awake()
    {
        PlayerController pc = FindAnyObjectByType<PlayerController>();
        if (pc != null)
        {
            playerTransform = pc.transform;
            playerAgent = pc.GetComponent<NavMeshAgent>();
            if (inputActions == null)
                inputActions = pc.inputActions;
        }

        cameraController = FindAnyObjectByType<CameraController>();

        if (inputActions != null)
        {
            InputActionMap playerMap = inputActions.FindActionMap("Player");
            if (playerMap != null)
                interactAction = playerMap.FindAction("Interact");

            if (interactAction == null)
                Debug.LogWarning("[Door] No 'Interact' action found in the 'Player' action map. Add it to your Input Action Asset.");
        }
    }

    private void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.performed += OnInteract;
            interactAction.Enable();
        }
    }

    private void OnDisable()
    {
        if (interactAction != null)
            interactAction.performed -= OnInteract;
    }

    private void Update()
    {
        if (playerTransform == null) return;
        playerInRange = Vector3.Distance(transform.position, playerTransform.position) <= interactRange;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (!playerInRange) return;
        if (linkedDoor == null || linkedDoor.spawnPoint == null)
        {
            Debug.LogWarning("[Door] linkedDoor or its spawnPoint is not assigned.");
            return;
        }

        TeleportPlayer();
    }

    // Warps the player to the linked door's spawn point and shifts the camera by the same delta.
    private void TeleportPlayer()
    {
        Vector3 destination = linkedDoor.spawnPoint.position;
        Vector3 delta = destination - playerTransform.position;
        
        

        if (playerAgent != null)
        {
            playerAgent.isStopped = true;
            playerAgent.ResetPath();
            playerAgent.Warp(destination);
        }
        else
        {
            playerTransform.position = destination;
        }

        if (cameraController != null)
            cameraController.transform.position += delta * cameraMoveIncrease;
    }
}