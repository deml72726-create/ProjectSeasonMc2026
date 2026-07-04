 using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameManagerSoup : MonoBehaviour
{
    // Simple Singleton pattern to easily access the GameManager
    public static GameManagerSoup Instance { get; private set; }

    public PlayerMovement playerMovementScript;

    [Tooltip("Assign the black full-screen UI Image here.")]
    public Image fadeOverlay; 
    
    public GameObject mainCamera;
    
    [Tooltip("Drag your Second Camera GameObject here")]
    public GameObject secondCamera;
    public float fadeDuration = 2f;
    public bool CanCloseTab = false;
    public bool OnSoup = false;
    public bool firsttimeinteract = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
           // Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player presses the Escape key and if they can close the tab
        if (Input.GetKeyDown(KeyCode.Escape) && CanCloseTab)
        {
            StartCoroutine(CloseSoupTask());
        }
    }


       public void OnSoupClicked()
    {   
        StartCoroutine(SoupClickSequence());
    }

        private IEnumerator SoupClickSequence()
    {
        //SET-UP: CHANGE OF CAMERA, DISABLE MOVEMENT, FADE OUT, WAIT, FADE IN
        desactivatemovement(false);

        FadeOut();
        yield return new WaitForSeconds(2f); 

        ToggleCameras(); // Switch to the second camera

        FadeIn();

                // Bird TASK MECHANISM
        OnSoup = true;
        CanCloseTab = true;
        Debug.Log("Soup task is now active. Press Escape to close.");; // Wait until the end of the frame to ensure the click is registered
    }

       // ----Helper function to set stuff up for the sheep task
    // FADEIN/FADEOUTFUNCTIONS

    private void desactivatemovement(bool state = false)
    {
        // Logic to deactivate player movement
        
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = state; // Turns the script checkbox on/off
            Debug.Log("Player movement enabled: " + state);
        }
        else
        {
            Debug.LogWarning("Player Movement Script is not assigned in the GameManager!");
        }
        
    }

    public void FadeOut()
    {
        StartCoroutine(FadeRoutine(0f, 1f));
    }

    // Call this method to fade back to the game
    public void FadeIn()
    {
        StartCoroutine(FadeRoutine(1f, 0f));
    }
    public void EnableSecondCamera()
    {
        mainCamera.SetActive(false);
        secondCamera.SetActive(true);
    }

     // Call this to switch back to the main camera
    public void EnableMainCamera()
    {
        secondCamera.SetActive(false);
        mainCamera.SetActive(true);
    }

    // Bonus: If you want a single button to just swap back and forth
    public void ToggleCameras()
    {
        // Check if the main camera is currently active
        bool isMainActive = mainCamera.activeInHierarchy;

        // Flip their states
        mainCamera.SetActive(!isMainActive);
        secondCamera.SetActive(isMainActive);
    }

    private IEnumerator FadeRoutine(float startAlpha, float targetAlpha)
    {
        float elapsedTime = 0f;
        Color color = fadeOverlay.color;
        color.a = startAlpha;
        fadeOverlay.color = color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            // Interpolate the alpha value over time
            color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            fadeOverlay.color = color;
            
            // Wait for the next frame
            yield return null; 
        }

        // Ensure the final alpha is set perfectly
        color.a = targetAlpha;
        fadeOverlay.color = color;
    }

    public IEnumerator CloseSoupTask()
    {
        // Logic to close the bird task and return to the main game
        Debug.Log("Closing Bird Task and returning to main game.");
        CanCloseTab = false;
        OnSoup = false;
        FadeOut();
        yield return new WaitForSeconds(2f); // Wait for fade out to complete
        ToggleCameras(); // Switch back to the main camera
        FadeIn();
        desactivatemovement(true);

    }
}
