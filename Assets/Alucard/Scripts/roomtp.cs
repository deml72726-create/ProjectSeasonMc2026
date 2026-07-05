using UnityEngine;
using System.Collections;
using TMPro; // Required for TextMeshPro

public class RoomTeleport : MonoBehaviour
{
    [Header("Transition Settings")]
    public Transform playerDestination;
    public CanvasGroup fadeGroup;
    public float fadeSpeed = 3.0f;
    public string roomName = "Kitchen";

    [Header("UI Settings")]
    public GameObject promptUI;      // Drag the Panel or GameObject here
    public TMP_Text promptText;      // Drag the TextMeshPro component here

    private bool isPlayerInRange = false;
    private bool isTransitioning = false;
    private GameObject playerObject;

    void Start()
    {
        if (fadeGroup != null)
        {
            fadeGroup.alpha = 0;
            fadeGroup.gameObject.SetActive(false);
        }

        if (promptUI != null)
            promptUI.SetActive(false);
    }

    void Update()
    {
        // Check for player input when in range
        if (isPlayerInRange && !isTransitioning && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(PerformTransition());
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerObject = other.gameObject;

            if (promptUI != null)
            {
                promptUI.SetActive(true);
                if (promptText != null) 
                    promptText.text = "Press E to enter " + roomName;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (promptUI != null)
                promptUI.SetActive(false);
        }
    }

    IEnumerator PerformTransition()
    {
        isTransitioning = true;
        
        // Hide the prompt while transitioning
        if (promptUI != null) promptUI.SetActive(false);

        PlayerMovement movement = playerObject.GetComponent<PlayerMovement>();
        Rigidbody2D rb = playerObject.GetComponent<Rigidbody2D>();

        if (movement != null) movement.enabled = false;
        if (rb != null) rb.linearVelocity = Vector2.zero;

        if (fadeGroup != null)
        {
            fadeGroup.gameObject.SetActive(true);
            while (fadeGroup.alpha < 1)
            {
                fadeGroup.alpha += Time.deltaTime * fadeSpeed;
                yield return null;
            }
        }

        playerObject.transform.position = playerDestination.position;
        yield return new WaitForSeconds(0.2f);

        if (fadeGroup != null)
        {
            while (fadeGroup.alpha > 0)
            {
                fadeGroup.alpha -= Time.deltaTime * fadeSpeed;
                yield return null;
            }
            fadeGroup.alpha = 0;
            fadeGroup.gameObject.SetActive(false);
        }

        if (movement != null) movement.enabled = true;
        isTransitioning = false;
    }
}