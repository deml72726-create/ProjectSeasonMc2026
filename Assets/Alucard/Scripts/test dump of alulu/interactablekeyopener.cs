using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class InteractableKeyOpener : MonoBehaviour
{
    public UnityEvent onInteractEvent;
    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (onInteractEvent != null)
            {
                onInteractEvent.Invoke();
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