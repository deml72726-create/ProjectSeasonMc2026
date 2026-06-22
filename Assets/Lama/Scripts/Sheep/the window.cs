using UnityEngine;

public class WindowInteractable : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("The tag assigned to your player GameObject (usually 'Player')")]
    public string playerTag = "Player";
    public bool isInteractable = true; // Set to false if you want to disable interaction
    private bool playerIsClose = false;

    void Update()
    {
        // If the player is inside the zone AND presses the E key
        if (playerIsClose && Input.GetKeyDown(KeyCode.E) && isInteractable)
        {
            Interact();
        }
    }

    private void Interact()
    {
        Debug.Log("Window interacted with!");
        
        // Fire your GameManager instance event
        GameManager.Instance.OnWindowClicked();
    }

    // Automatically detects when the player walks into the interaction zone
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            playerIsClose = true;
        }
    }

    // Automatically detects when the player walks away
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            playerIsClose = false;
        }
    }

    public void SetInteractable()
    {
        isInteractable = false;
    }
}