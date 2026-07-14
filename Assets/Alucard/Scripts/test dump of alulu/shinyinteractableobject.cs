using System.Collections;
using System.Collections.Generic;
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

    public bool isBubbleEnabled = true;
    public CanvasGroup bubbleCanvasGroup; 
    public TMP_Text bubbleText; 
    public float fadeDuration = 0.3f;
    
    public string interactionMessage = "Press E to investigate";
    public GameObject puzzleCanvas;

    private Material defaultMaterial;
    private bool playerInRange = false;
    private Coroutine fadeCoroutine;

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
            if (bubbleCanvasGroup != null)
            {
                bubbleCanvasGroup.alpha = 0f;
                bubbleCanvasGroup.blocksRaycasts = false;
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

            if (bubbleText != null)
            {
                bubbleText.text = interactionMessage;
            }

            if (isBubbleEnabled && bubbleCanvasGroup != null)
            {
                StopFade();
                fadeCoroutine = StartCoroutine(FadeCanvasGroup(bubbleCanvasGroup, 1.0f));
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

            if (isBubbleEnabled && bubbleCanvasGroup != null)
            {
                StopFade();
                fadeCoroutine = StartCoroutine(FadeCanvasGroup(bubbleCanvasGroup, 0.0f));
            }
        }
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float targetAlpha)
    {
        float startAlpha = cg.alpha;
        float time = 0.0f;

        if (targetAlpha > 0)
        {
            cg.blocksRaycasts = true;
        }

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        cg.alpha = targetAlpha;
        if (targetAlpha == 0)
        {
            cg.blocksRaycasts = false;
        }
    }

    void StopFade()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
    }
}