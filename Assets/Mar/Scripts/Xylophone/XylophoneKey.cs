using UnityEngine;
using System.Collections;

public class XylophoneKey : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip noteSFX;
    private AudioSource audioSource;

    [Header("Visuals")]
    public Color goldenColor = new Color(1f, 0.84f, 0f); // A nice gold
    private Color originalColor;
    private SpriteRenderer spriteRenderer;

    [Header("Mallet Reference")]
    public Transform malletObject; // Drag the "Stick" object here

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        // Ensure we have an AudioSource on this object
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnMouseDown()
    {
        // Only allow clicking if we are actually in the minigame
        if (GameManagerPiano.Instance.isInMinigame)
        {
            PlayKey();
        }
    }

    void PlayKey()
    {
        // 1. Play the specific sound
        audioSource.PlayOneShot(noteSFX);

        // 2. Visual Feedback: Change color to Golden
        StopAllCoroutines();
        StartCoroutine(KeyVisualRoutine());

        // 3. Move the stick to the click position
        if (malletObject != null)
        {
            // Get mouse position in world space
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0; // Keep it in 2D plane
            
            malletObject.position = mousePos;
            
            // Show the stick (it will hide itself via its own script)
            malletObject.gameObject.SetActive(true);
        }
    }

    private IEnumerator KeyVisualRoutine()
    {
        spriteRenderer.color = goldenColor;
        yield return new WaitForSeconds(0.15f); // Stay golden for a moment
        spriteRenderer.color = originalColor;
    }
}