using UnityEngine;

public class BirdInteractableInteractions : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("The tag assigned to your player GameObject (usually 'Player')")]
    public string playerTag = "Player";
    public bool isInteractable = true; // Set to false if you want to disable interaction
    private bool playerIsClose = false;
    private Animator myAnimator;

    //----MELODIES---
    public GameObject BirdMelody1;
    public GameObject BirdMelody2;
    public GameObject BirdMelody3;
    public GameObject BirdMelody4;


    void Start()
    {
        // Get the Animator component attached to this GameObject
        myAnimator = GetComponent<Animator>();
    }

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
        Debug.Log("Bird interacted with!");
        
        // Fire your GameManager instance event
        GameManagerBird.Instance.OnBirdClicked();
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
    
    public void Animate()
    {
        myAnimator.SetBool("Sing", true);
    }
}