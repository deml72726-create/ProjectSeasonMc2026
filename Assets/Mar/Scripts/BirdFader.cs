using UnityEngine;
using System.Collections;

public class BirdFader : MonoBehaviour
{
    public static BirdFader Instance;
    private SpriteRenderer fadeRenderer;

    void Awake()
    {
        Instance = this;
        CreateFadeOverlay();
    }

    void CreateFadeOverlay()
    {
        GameObject fadeObj = new GameObject("FadeOverlay");
        fadeObj.transform.parent = this.transform;
        fadeRenderer = fadeObj.AddComponent<SpriteRenderer>();
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.black);
        tex.Apply();
        fadeRenderer.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
        fadeRenderer.sortingOrder = 9999;
        fadeObj.transform.localScale = new Vector3(5000, 5000, 1);
        fadeRenderer.color = new Color(0, 0, 0, 0);
    }

    public void MoveToCamera(GameObject activeCamera)
    {
        if (activeCamera == null) return;
        transform.position = new Vector3(activeCamera.transform.position.x, activeCamera.transform.position.y, activeCamera.transform.position.z + 1);
    }

    public IEnumerator FadeRoutine(float targetAlpha, float duration)
    {
        float startAlpha = fadeRenderer.color.a;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fadeRenderer.color = new Color(0, 0, 0, Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration));
            yield return null;
        }
        fadeRenderer.color = new Color(0, 0, 0, targetAlpha);
    }
}