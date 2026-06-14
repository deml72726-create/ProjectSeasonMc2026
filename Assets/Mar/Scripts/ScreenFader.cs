using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        // --- CODE-ONLY UI GENERATION ---
        GameObject canvasObj = new GameObject("FadeCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; 
        
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject imageObj = new GameObject("FadeImage");
        imageObj.transform.SetParent(canvasObj.transform);
        
        Image img = imageObj.AddComponent<Image>();
        img.color = Color.black;
        img.raycastTarget = false; // Important: Don't block mouse yet

        // FULL SCREEN FIX: This forces it out of the corner to cover everything
        RectTransform rt = img.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        canvasGroup = imageObj.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0; // Start transparent (No fade-in at start)
        canvasGroup.blocksRaycasts = false;
    }

    public IEnumerator FadeOut(float duration)
    {
        canvasGroup.blocksRaycasts = true; 
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, timer / duration);
            yield return null;
        }
        canvasGroup.alpha = 1;
    }
}