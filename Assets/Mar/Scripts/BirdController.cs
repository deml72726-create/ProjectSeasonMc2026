using UnityEngine;
// REMOVED: using UnityEngine.SceneManagement; (This was causing the conflict)

public class BirdController : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color originalColor;
    private bool canInteract = false; // Changed from isPlayerNearby to match SetHighlight logic

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;
    }

    void Update()
    {
        // When the wall tells us we can interact AND we press E
        if (canInteract && Input.GetKeyDown(KeyCode.E))
        {
            // Use global:: to make sure it talks to YOUR SceneManager
            global::SceneManager.Instance.StartBirdMinigame();
        }
    }

    // --- THIS IS THE FUNCTION YOUR InteractionTrigger WAS LOOKING FOR ---
    public void SetHighlight(bool highlighted)
    {
        canInteract = highlighted;
        
        if (sr != null)
        {
            sr.color = highlighted ? Color.yellow : originalColor;
        }
    }

    // We keep these as a backup, but the InteractionTrigger script 
    // you showed in the screenshot will now handle the highlighting properly.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SetHighlight(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SetHighlight(false);
        }
    }
}