using System.Collections;
using UnityEngine;

public class TeleportWarpEffect : MonoBehaviour
{
    public static TeleportWarpEffect Instance;
    public Transform playerSpriteTransform; 
    
    public AudioSource sfxSource;
    public AudioClip warpSFX;

    private Vector3 originalScale;
    private Coroutine activeWarpCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (playerSpriteTransform != null)
        {
            originalScale = playerSpriteTransform.localScale;
        }

        if (sfxSource == null)
        {
            sfxSource = GetComponent<AudioSource>();
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
        sfxSource.spatialBlend = 0f;

        TriggerWarp(); 
    }

    public void TriggerWarp()
    {
        if (playerSpriteTransform == null) return;

        if (sfxSource != null && warpSFX != null)
        {
            sfxSource.PlayOneShot(warpSFX);
        }

        if (activeWarpCoroutine != null) StopCoroutine(activeWarpCoroutine);
        activeWarpCoroutine = StartCoroutine(WarpRoutine());
    }

    IEnumerator WarpRoutine()
    {
        PlayerMovement playerMove = FindFirstObjectByType<PlayerMovement>();
        if (playerMove != null)
        {
            playerMove.enabled = false;
        }

        float elapsed = 0f;
        float duration = 0.95f;

        Vector3 squishScale = new Vector3(originalScale.x * 1.15f, originalScale.y * 0.8f, originalScale.z);
        while (elapsed < duration * 0.3f)
        {
            elapsed += Time.deltaTime;
            playerSpriteTransform.localScale = Vector3.Lerp(originalScale, squishScale, elapsed / (duration * 0.3f));
            yield return null;
        }

        elapsed = 0f;
        Vector3 stretchScale = new Vector3(originalScale.x * 0.88f, originalScale.y * 1.18f, originalScale.z);
        while (elapsed < duration * 0.3f)
        {
            elapsed += Time.deltaTime;
            playerSpriteTransform.localScale = Vector3.Lerp(squishScale, stretchScale, elapsed / (duration * 0.3f));
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < duration * 0.4f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration * 0.4f);
            float bounce = Mathf.Sin(t * Mathf.PI * 2f) * (1f - t) * 0.08f;
            playerSpriteTransform.localScale = new Vector3(originalScale.x + bounce, originalScale.y - bounce, originalScale.z);
            yield return null;
        }

        playerSpriteTransform.localScale = originalScale;

        if (playerMove != null)
        {
            playerMove.enabled = true;
        }
    }
}