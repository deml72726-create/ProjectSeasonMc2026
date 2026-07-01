using UnityEngine;
using System.Collections;

public class GameManagerPiano : MonoBehaviour
{
    public static GameManagerPiano Instance { get; private set; }

    [Header("Player & Movement")]
    public PlayerMovement playerMovementScript;

    [Header("Cameras")]
    public GameObject mainCamera;
    public GameObject xylophoneCamera;

    [Header("Transition Settings")]
    public float fadeDuration = 1f;
    private float currentAlpha = 0f;
    private Texture2D fadeTexture;

    [Header("UI & Content")]
    public GameObject goBackCanvas; 
    public GameObject xylophoneMinigameRoom; 

    [Header("State")]
    public bool isInMinigame = false;
    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        fadeTexture = new Texture2D(1, 1);
        fadeTexture.SetPixel(0, 0, Color.black);
        fadeTexture.Apply();
    }

    private void Start()
    {
        xylophoneCamera.SetActive(false);
        mainCamera.SetActive(true);
        xylophoneMinigameRoom.SetActive(false);
        goBackCanvas.SetActive(false);
        currentAlpha = 0f;
        isInMinigame = false; // Ensure this is false at start
    }

    private void Update()
    {
        // Changed KeyCode.E to KeyCode.Escape here
        if (isInMinigame && !isTransitioning)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TriggerExit();
            }
        }
    }

    private void OnGUI()
    {
        if (currentAlpha > 0)
        {
            GUI.color = new Color(0, 0, 0, currentAlpha);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture);
        }
    }

    public void TriggerEnter()
    {
        if (!isTransitioning && !isInMinigame) 
            StartCoroutine(EnterSequence());
    }

    public void TriggerExit()
    {
        if (!isTransitioning && isInMinigame) 
            StartCoroutine(ExitSequence());
    }

    private IEnumerator EnterSequence()
    {
        isTransitioning = true;
        isInMinigame = true; // MOVE THIS TO THE TOP

        if (playerMovementScript != null) 
        {
            // FIX FOR SLIDING: Reset the Rigidbody velocity before disabling the script
            Rigidbody2D rb = playerMovementScript.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;

            playerMovementScript.enabled = false;
        }

        yield return StartCoroutine(Fade(0, 1));

        mainCamera.SetActive(false);
        xylophoneCamera.SetActive(true);
        xylophoneMinigameRoom.SetActive(true);
        goBackCanvas.SetActive(true);

        yield return new WaitForSeconds(0.3f); // Small buffer to prevent instant exit
        yield return StartCoroutine(Fade(1, 0));

        isTransitioning = false;
    }

    private IEnumerator ExitSequence()
    {
        isTransitioning = true;
        isInMinigame = false; // MOVE THIS TO THE TOP

        yield return StartCoroutine(Fade(0, 1));

        xylophoneMinigameRoom.SetActive(false);
        goBackCanvas.SetActive(false);
        xylophoneCamera.SetActive(false);
        mainCamera.SetActive(true);

        yield return new WaitForSeconds(0.3f); // Small buffer to prevent instant re-entry
        yield return StartCoroutine(Fade(1, 0));

        if (playerMovementScript != null) playerMovementScript.enabled = true;

        isTransitioning = false;
    }

    private IEnumerator Fade(float startAlpha, float targetAlpha)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            yield return null; 
        }
        currentAlpha = targetAlpha;
    }
}