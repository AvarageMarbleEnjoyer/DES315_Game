using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//Place this on the win condition prefab in the final room//
//Player clicks the object to trigger the win screen -EM//
public class WinCondition : MonoBehaviour
{
    [Header("Interaction")]
    [Tooltip("How close the player must be to click and win")]
    public float interactionRange = 3f;

    [Tooltip("Layer mask for the player")]
    public LayerMask playerLayer;

    [Header("Win Screen")]
    [Tooltip("Name of the scene to load on win (leave blank to show UI panel instead")]
    public string winSceneName = "";

    [Tooltip("Optional UI panel to activate on win (used if winSceneName is blank")]
    public GameObject winScreenPanel;

    [Tooltip("Optional prompt shown above the object when the player is in range")]
    public GameObject interactPrompt;

    [Header("Room Marker")]
    [Tooltip("Optional decorative prefab to spawn at the centre of the final room, Leave blank to use the WinCondition object's own model.")]
    public GameObject roomMarkerPrefab;

    [Tooltip("Offset from the win conditions position to spawn the marker")]
    public Vector3 markerOffset = Vector3.zero;

    [Header("Pulse Visual")]
    [Tooltip("If true, the object bobs up abd down to draw attention")]
    public bool doPulse = true;
    public float pulseSpeed = 1.5f;
    public float pulseHeight = 0.15f;

    [Header("Input")]
    public InputActionAsset inputActions;

    [Header("Debug")]
    public bool debugMode = true;

    private InputAction interactionAction;
    private Transform playerTransform;
    private Vector3 startPosition;
    private bool gameWon = false;
    private bool playerInRange = false;

    private void Awake()
    {
        //Get the interaction action from the Player map//
        if(inputActions != null)
        {
            var playerMap = inputActions.FindActionMap("Player");
            interactionAction = playerMap?.FindAction("Interact");
        }
    }
    private void Start()
    {
        startPosition = transform.position;

        //Find player//
        PlayerController pc = FindFirstObjectByType<PlayerController>();
        if (pc != null) playerTransform = pc.transform;

        if (interactPrompt != null) interactPrompt.SetActive(false);

        //Spawn the decorative room marker if one is assigned//
        if(roomMarkerPrefab != null)
        {
            GameObject marker = Instantiate(roomMarkerPrefab, transform.position + markerOffset, Quaternion.identity, transform);
            marker.name = "FinalRoomMarker";
        }
    }

    private void OnEnable()
    {
        if(interactionAction != null)
        {
            interactionAction.performed += OnInteract;
            interactionAction.Disable();
        }
    }

    private void OnDisable()
    {
        if(interactionAction != null)
        {
            interactionAction.performed -= OnInteract;
            interactionAction.Disable();
        }
    }

    private void Update()
    {
        if (gameWon) return;

        //Pulse animation//
        if(doPulse)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * pulseSpeed) * pulseHeight;
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        }

        //Show or hide ineract prompt based on player proximity//
        bool wasInRange = playerInRange;
        playerInRange = IsPlayerInRange();

        if(playerInRange != wasInRange && interactPrompt != null)
        {

            interactPrompt.SetActive(playerInRange);

            if (debugMode) Debug.Log($"[WinCondition] Player {(playerInRange ? "entered" : "left")} interaction range");
        }
    }

    //Called when the playert presses E//
    private void OnInteract(InputAction.CallbackContext context)
    {
        if (gameWon) return;
        if (!playerInRange) return;

        TriggerWin();
    }

    public void TriggerWin()
    {
        if (gameWon) return;
        gameWon = true;

        if (debugMode) Debug.Log("[WinCondition] Player won the game!");

        MessageUI.Instance?.EnqueueMessage("You escaped the dungeon! You Win");

        if(!string.IsNullOrEmpty(winSceneName))
        {
            SceneManager.LoadScene(winSceneName);
        }
        else if (winScreenPanel != null) 
        {
            winScreenPanel.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogWarning("[WinCondition] No win scene or panel set! Add one in the Inspector.");
        }
    }

    private bool IsPlayerInRange()
    {
        if (playerTransform == null) return false;
        return Vector3.Distance(transform.position, playerTransform.position) <= interactionRange;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
