using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class InteractableKeyOpener : MonoBehaviour
{
    public UnityEvent onInteractEvent;
    public GameObject canvasToControl; // Drag your [Soup Canva] object here
    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            // Only open if the canvas is NOT currently active
            if (canvasToControl != null && !canvasToControl.activeSelf)
            {
                if (onInteractEvent != null)
                {
                    onInteractEvent.Invoke();
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}