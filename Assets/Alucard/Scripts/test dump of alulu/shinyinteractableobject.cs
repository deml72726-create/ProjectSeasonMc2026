using UnityEngine;
using TMPro;
using System.Collections;

public class ShinyInteractable : MonoBehaviour
{
    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public Material outlineMaterial;
    private Material defaultMaterial;

    [Header("Bubble Settings")]
    public bool isBubbleEnabled = true;
    public CanvasGroup bubbleCanvasGroup; 
    public TMP_Text bubbleText; 
    public float fadeDuration = 0.3f;
    
    [TextArea]
    public string interactionMessage = "Press E to investigate";

    private Coroutine fadeCoroutine;

    void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer != null)
        {
            defaultMaterial = spriteRenderer.material;
        }

        if (bubbleCanvasGroup != null)
        {
            bubbleCanvasGroup.alpha = 0f;
            bubbleCanvasGroup.blocksRaycasts = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (bubbleText != null) 
            {
                bubbleText.text = interactionMessage;
            }

            if (spriteRenderer != null && outlineMaterial != null)
            {
                spriteRenderer.material = outlineMaterial;
            }

            if (isBubbleEnabled && bubbleCanvasGroup != null)
            {
                StopFade();
                fadeCoroutine = StartCoroutine(FadeCanvasGroup(bubbleCanvasGroup, 1f));
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (spriteRenderer != null && defaultMaterial != null)
            {
                spriteRenderer.material = defaultMaterial;
            }

            if (isBubbleEnabled && bubbleCanvasGroup != null)
            {
                StopFade();
                fadeCoroutine = StartCoroutine(FadeCanvasGroup(bubbleCanvasGroup, 0f));
            }
        }
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float targetAlpha)
    {
        float startAlpha = cg.alpha;
        float time = 0f;

        if (targetAlpha > 0) cg.blocksRaycasts = true;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        cg.alpha = targetAlpha;
        if (targetAlpha == 0) cg.blocksRaycasts = false;
    }

    void StopFade()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
    }
}