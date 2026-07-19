using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManagerSheep : MonoBehaviour
{
    public static GameManagerSheep Instance { get; private set; }

    [Header("References")]
    public PlayerMovement playerMovementScript;
    public GameObject mainCamera;
    public GameObject secondCamera;
    public GameObject sheepSpawner;
    public WindowInteractable window;
    public GameObject itemToDrop; 
    public Transform dropLocation; 

    [Header("Settings")]
    public int correctTargetNumber = 5; 

    [Header("UI")]
    public Image fadeOverlay;
    public GameObject Rideau;
    public GameObject Rideauleft;
    public float fadeDuration = 0.5f;

    [Header("Game State")]
    public bool isGameActive = false;
    public bool hasWon = false;
    public bool hasLost = false;
    public bool isPermanentlyWon = false; 
    public AudioSource wind;

    [Header("Dialogue Content")]
    public string[] introDialogueLines;
    public string[] winDialogueLines;
    public string[] loseDialogueLines;

    private int sequenceCounter = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (secondCamera != null && secondCamera.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            ExitGameToMain();
        }
    }

    public int GetNextWeightedNumber()
    {
        sequenceCounter++;
        if (sequenceCounter > 9) sequenceCounter = 1;
        return sequenceCounter;
    }

    public void ReceiveCloneNumber(int incomingNumber)
    {
        if (isPermanentlyWon) return; 

        if (incomingNumber == correctTargetNumber)
            hasWon = true;
        else
            hasLost = true;
    }

    public void OnWindowClicked()
    { 
        if (isPermanentlyWon) return; 

        hasWon = false;
        hasLost = false;
        StartCoroutine(WindowClickSequence());
    }

    private IEnumerator WindowClickSequence()
    {
        desactivatemovement(false);
        FadeOut();
        yield return new WaitForSeconds(fadeDuration);

        // Turn on Cursor for overlay
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        EnableSecondCamera();
        FadeIn();
        ShowCurtains(); 

        DialogueManager.Instance.StartDialogue(introDialogueLines);
        sheepSpawner.SetActive(true);
        isGameActive = true;
        if (wind != null) wind.Play();

        yield return new WaitUntil(() => hasWon == true || hasLost == true);

        if (hasWon) 
        {
            isPermanentlyWon = true; 
            DialogueManager.Instance.StartDialogue(winDialogueLines);
            if (itemToDrop != null && dropLocation != null)
                Instantiate(itemToDrop, dropLocation.position, Quaternion.identity);
        }
        else 
        {
            DialogueManager.Instance.StartDialogue(loseDialogueLines);
        }

        yield return new WaitForSeconds(2f);
        FadeOut();
        yield return new WaitForSeconds(fadeDuration);
        
        if (wind != null) wind.Stop();
        HideCurtains();
        sheepSpawner.SetActive(false);
        isGameActive = false;
        
        yield return new WaitForSeconds(0.5f); 
        EnableMainCamera();
        desactivatemovement(true);
        FadeIn();

        // Turn off Cursor on exit
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ExitGameToMain()
    {
        StopAllCoroutines();
        if (wind != null) wind.Stop();
        sheepSpawner.SetActive(false);
        isGameActive = false;
        HideCurtains();
        EnableMainCamera();
        desactivatemovement(true);
        FadeIn();

        // Ensure cursor is hidden/locked when hard exiting via ESC
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ShowCurtains() 
    { 
        Rideau.SetActive(true); 
        Rideauleft.SetActive(true); 
    }

    public void HideCurtains() 
    { 
        Rideau.SetActive(false); 
        Rideauleft.SetActive(false); 
    }

    private void desactivatemovement(bool state) 
    { 
        if (playerMovementScript != null) playerMovementScript.enabled = state; 
    }

    public void FadeOut() => StartCoroutine(FadeRoutine(0f, 1f));
    public void FadeIn() => StartCoroutine(FadeRoutine(1f, 0f));

    private IEnumerator FadeRoutine(float startAlpha, float targetAlpha)
    {
        float elapsedTime = 0f;
        Color color = fadeOverlay.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            fadeOverlay.color = color;
            yield return null;
        }
    }

    public void EnableSecondCamera() { mainCamera.SetActive(false); secondCamera.SetActive(true); }
    public void EnableMainCamera() { secondCamera.SetActive(false); mainCamera.SetActive(true); }
}