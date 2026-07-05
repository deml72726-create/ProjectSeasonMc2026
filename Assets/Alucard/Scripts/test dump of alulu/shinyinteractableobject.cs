using UnityEngine;
using TMPro; // Make sure to include this

public class ShinyInteractable : MonoBehaviour
{
    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public Material outlineMaterial;
    private Material defaultMaterial;

    [Header("UI Prompt")]
    public GameObject promptUI;      // Drag your Panel/UI here
    public TMP_Text promptText;      // Drag your TextMeshPro object here
    public string interactionMessage = "Press E to investigate";

    void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        defaultMaterial = spriteRenderer.material;

        if (promptUI != null) promptUI.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Apply Outline
            if (spriteRenderer != null && outlineMaterial != null)
                spriteRenderer.material = outlineMaterial;

            // Show Text
            if (promptUI != null)
            {
                promptUI.SetActive(true);
                if (promptText != null) promptText.text = interactionMessage;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Revert Outline
            if (spriteRenderer != null)
                spriteRenderer.material = defaultMaterial;

            // Hide Text
            if (promptUI != null) promptUI.SetActive(false);
        }
    }
}