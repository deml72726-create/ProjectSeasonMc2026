using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class ShinyInteractable : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Material outlineMaterial;
    public GameObject promptUI;
    public TMP_Text promptText;
    public string interactionMessage = "Press E to investigate";
    public GameObject puzzleCanvas;

    private Material defaultMaterial;
    private bool playerInRange = false;

    void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        defaultMaterial = spriteRenderer.material;

        if (promptUI != null)
        {
            promptUI.SetActive(false);
        }
    }

    void Update()
    {
        if (playerInRange && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (puzzleCanvas != null)
            {
                puzzleCanvas.SetActive(true);
            }
            if (promptUI != null)
            {
                promptUI.SetActive(false);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (spriteRenderer != null && outlineMaterial != null)
            {
                spriteRenderer.material = outlineMaterial;
            }

            if (promptUI != null)
            {
                promptUI.SetActive(true);
                if (promptText != null)
                {
                    promptText.text = interactionMessage;
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (spriteRenderer != null)
            {
                spriteRenderer.material = defaultMaterial;
            }

            if (promptUI != null)
            {
                promptUI.SetActive(false);
            }
        }
    }
}