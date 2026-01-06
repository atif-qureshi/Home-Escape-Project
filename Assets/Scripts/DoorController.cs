using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;  // ADD THIS LINE

public class DoorController : MonoBehaviour
{
    // Door Settings
    public Transform doorObject;
    public float openAngle = 90f;
    public float openSpeed = 2f;

    // UI Text Settings
    public GameObject textDisplay;
    public string openMessage = "Press [E] to Open Door";
    public string closeMessage = "Press [E] to Close Door";

    // Input Action
    private InputAction interactAction;  // FOR NEW INPUT SYSTEM

    // Private variables
    private bool isOpen = false;
    private bool playerIsNear = false;
    private Quaternion startRotation;
    private Quaternion endRotation;

    void Start()
    {
        // Set initial rotations
        startRotation = doorObject.rotation;
        endRotation = startRotation * Quaternion.Euler(0, openAngle, 0);

        // Hide text at start
        if (textDisplay != null)
            textDisplay.SetActive(false);

        // SETUP NEW INPUT SYSTEM
        interactAction = new InputAction(binding: "<Keyboard>/e");
        interactAction.performed += ctx => OnInteractPressed();
        interactAction.Enable();
    }

    void Update()
    {
        // Smooth door rotation
        if (isOpen)
        {
            doorObject.rotation = Quaternion.Slerp(
                doorObject.rotation,
                endRotation,
                Time.deltaTime * openSpeed
            );
        }
        else
        {
            doorObject.rotation = Quaternion.Slerp(
                doorObject.rotation,
                startRotation,
                Time.deltaTime * openSpeed
            );
        }
    }

    // NEW METHOD FOR INPUT SYSTEM
    void OnInteractPressed()
    {
        if (playerIsNear)
        {
            if (!isOpen)
            {
                OpenDoor();
            }
            else
            {
                CloseDoor();
            }
        }
    }

    void OpenDoor()
    {
        isOpen = true;
        // Change text to close message
        if (textDisplay != null)
        {
            Text textComponent = textDisplay.GetComponent<Text>();
            if (textComponent != null)
                textComponent.text = closeMessage;
        }
    }

    void CloseDoor()
    {
        isOpen = false;
        // Change text to open message
        if (textDisplay != null)
        {
            Text textComponent = textDisplay.GetComponent<Text>();
            if (textComponent != null)
                textComponent.text = openMessage;
        }
    }

    // When player enters the trigger zone
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
            // Show text
            if (textDisplay != null)
            {
                textDisplay.SetActive(true);
                Text textComponent = textDisplay.GetComponent<Text>();
                if (textComponent != null)
                    textComponent.text = isOpen ? closeMessage : openMessage;
            }
        }
    }

    // When player leaves the trigger zone
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
            // Hide text
            if (textDisplay != null)
                textDisplay.SetActive(false);
        }
    }

    // Clean up input action
    void OnDestroy()
    {
        if (interactAction != null)
        {
            interactAction.Disable();
            interactAction.Dispose();
        }
    }
}