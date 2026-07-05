using UnityEngine;
using UnityEngine.UI;

public class UISpriteAnimator : MonoBehaviour
{
    public Image targetImage;
    public Sprite[] animationFrames;
    public float framesPerSecond = 8.0f;
    private int currentFrameIndex = 0;
    private float timer = 0.0f;

    void Start()
    {
        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }
    }

    void Update()
    {
        if (animationFrames == null || animationFrames.Length == 0) return;

        timer += Time.deltaTime;
        float frameDuration = 1.0f / framesPerSecond;

        if (timer >= frameDuration)
        {
            timer = 0.0f;
            currentFrameIndex = (currentFrameIndex + 1) % animationFrames.Length;
            if (targetImage != null)
            {
                targetImage.sprite = animationFrames[currentFrameIndex];
            }
        }
    }
}