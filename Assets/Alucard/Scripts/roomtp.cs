using UnityEngine;
using System.Collections;
using TMPro;

public class RoomTeleport : MonoBehaviour
{
    [Header("Transition Settings")]
    public Transform playerDestination;
    public CanvasGroup fadeGroup;
    public float fadeSpeed = 3.0f;
    public string roomName = "Kitchen";

    [Header("UI Settings")]
    public GameObject promptUI;      
    public TMP_Text promptText;      

    [Header("Audio Settings")]
    public AudioSource sfxSource;
    public AudioClip teleportSFX;

    private bool isPlayerInRange = false;
    private bool isTransitioning = false;
    private GameObject playerObject;

    void Start()
    {
        if (fadeGroup != null)
        {
            fadeGroup.alpha = 0;
            fadeGroup.gameObject.SetActive(false);
        }

        if (promptUI != null)
            promptUI.SetActive(false);

        if (sfxSource == null)
        {
            sfxSource = GetComponent<AudioSource>();
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
        sfxSource.spatialBlend = 0f;
    }

    void Update()
    {
        if (isPlayerInRange && !isTransitioning && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(PerformTransition());
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerObject = other.gameObject;

            if (promptUI != null)
            {
                promptUI.SetActive(true);
                if (promptText != null) 
                    promptText.text = "Press E to enter " + roomName;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (promptUI != null)
                promptUI.SetActive(false);
        }
    }

    IEnumerator PerformTransition()
    {
        isTransitioning = true;
        
        if (promptUI != null) promptUI.SetActive(false);

        PlayerMovement movement = playerObject.GetComponent<PlayerMovement>();
        Rigidbody2D rb = playerObject.GetComponent<Rigidbody2D>();

        if (movement != null) movement.enabled = false;
        if (rb != null) rb.linearVelocity = Vector2.zero;

        if (sfxSource != null && teleportSFX != null)
        {
            sfxSource.PlayOneShot(teleportSFX);
        }

        if (fadeGroup != null)
        {
            fadeGroup.gameObject.SetActive(true);
            while (fadeGroup.alpha < 1)
            {
                fadeGroup.alpha += Time.deltaTime * fadeSpeed;
                yield return null;
            }
        }

        playerObject.transform.position = playerDestination.position;

        if (TeleportWarpEffect.Instance != null)
        {
            TeleportWarpEffect.Instance.TriggerWarp();
        }

        yield return new WaitForSeconds(0.2f);

        if (fadeGroup != null)
        {
            while (fadeGroup.alpha > 0)
            {
                fadeGroup.alpha -= Time.deltaTime * fadeSpeed;
                yield return null;
            }
            fadeGroup.alpha = 0;
            fadeGroup.gameObject.SetActive(false);
        }

        if (movement != null) movement.enabled = true;
        isTransitioning = false;
    }
}