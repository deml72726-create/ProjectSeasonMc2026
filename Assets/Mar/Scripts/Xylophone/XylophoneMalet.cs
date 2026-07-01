using UnityEngine;
using System.Collections;

public class XylophoneMallet : MonoBehaviour
{
    [Header("Settings")]
    public float yOffset = -0.5f;
    
    [Tooltip("Match this to your transition time: Fade(1s) + Wait(0.5s)")]
    public float appearanceDelay = 1.5f; 

    private SpriteRenderer spriteRenderer;
    private bool isWaitingToShow = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Start with the sprite hidden so it doesn't show in the main room
        spriteRenderer.enabled = false;
    }

    void Update()
    {
        // 1. Check if we are in the minigame
        bool inMinigame = GameManagerPiano.Instance.isInMinigame;

        if (inMinigame)
        {
            // Always follow the mouse while in minigame, even if invisible
            FollowMouse();

            // 2. If we just entered and aren't showing yet, start the timed delay
            if (!spriteRenderer.enabled && !isWaitingToShow)
            {
                StartCoroutine(TimedAppearance());
            }
        }
        else
        {
            // 3. Hide immediately when leaving the minigame
            spriteRenderer.enabled = false;
            isWaitingToShow = false;
            StopAllCoroutines();
        }
    }

    private IEnumerator TimedAppearance()
    {
        isWaitingToShow = true;

        // Wait for the Fade Out (1s) and the Buffer (0.5s) from GameManager
        yield return new WaitForSeconds(appearanceDelay);

        // Only show if the player hasn't already exited during the fade
        if (GameManagerPiano.Instance.isInMinigame)
        {
            spriteRenderer.enabled = true;
        }

        isWaitingToShow = false;
    }

    void FollowMouse()
    {
        if (Camera.main == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        transform.position = mousePos + new Vector3(0, yOffset, 0);
    }
}