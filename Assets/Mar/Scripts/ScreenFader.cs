using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Added this
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;
    private CanvasGroup canvasGroup;
    public float defaultFadeSpeed = 0.5f;

    private void Awake()
    {
        if (Instance == null) { 
            Instance = this; 
            DontDestroyOnLoad(gameObject); 
        }
        else { 
            Destroy(gameObject); 
            return; 
        }

        CreateUI();
    }

private void OnEnable() {
    // Change this line to use the full address:
    UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
}

private void OnDisable() {
    // Change this line to use the full address:
    UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
}
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        StopAllCoroutines();
        StartCoroutine(FadeIn(defaultFadeSpeed));
    }
    // -----------------------------------------------------------------------

    private void CreateUI()
    {
        GameObject canvasObj = new GameObject("FadeCanvas");
        canvasObj.transform.SetParent(this.transform);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        GameObject imgObj = new GameObject("FadeImage");
        imgObj.transform.SetParent(canvasObj.transform);
        Image img = imgObj.AddComponent<Image>();
        img.color = Color.black;

        RectTransform rt = img.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        canvasGroup = imgObj.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 1; 
        canvasGroup.blocksRaycasts = false;
    }

    public IEnumerator FadeOut(float duration)
    {
        canvasGroup.blocksRaycasts = true;
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = timer / duration;
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    public IEnumerator FadeIn(float duration)
    {
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = 1 - (timer / duration);
            yield return null;
        }
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
}