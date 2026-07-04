 using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameManagerBird : MonoBehaviour
{
    // Simple Singleton pattern to easily access the GameManager
    public static GameManagerBird Instance { get; private set; }

    public PlayerMovement playerMovementScript;

    [Tooltip("Assign the black full-screen UI Image here.")]
    public Image fadeOverlay; 
    
    public GameObject mainCamera;
    public BirdInteractableInteractions Bird;
    
    [Tooltip("Drag your Second Camera GameObject here")]
    public GameObject secondCamera;
    public float fadeDuration = 2f;
    public bool CanCloseTab = false;
    public bool OnBird = false;
    public bool firsttimeinteract = true;
    public int currentIndex = 0; // To track the current index in the melody list
    public int comblength = 4; // Length of the melody combination
    public PrefabGenerator prefabSpawner; // Reference to the PrefabGenerator

    //MELODYLIST
    List<GameObject> Melodies = new List<GameObject>();
    public List<GameObject> GetRandomPrefabCombination(int n)
    {
        List<GameObject> combination = new List<GameObject>();

        for (int i = 0; i < n; i++)
        {
            // Pick a random index based on how many prefabs are in your master list
            int randomIndex = Random.Range(0, Melodies.Count);
            
            // Add the random prefab to the new combination list
            combination.Add(Melodies[randomIndex]);
        }

        return combination;
    }
    public List<GameObject> Symphony ; // Assuming you want a combination of length 4


    //-------CODE STARTS HERE-------
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        Melodies.Add(Bird.BirdMelody1);
        Melodies.Add(Bird.BirdMelody2);
        Melodies.Add(Bird.BirdMelody3);
        Melodies.Add(Bird.BirdMelody4);
    }

    void Update()
    {
        // Check if the player presses the Escape key and if they can close the tab
        if (Input.GetKeyDown(KeyCode.Escape) && CanCloseTab)
        {
            StartCoroutine(CloseBirdTask());
        }
        if (Input.GetMouseButtonDown(0) && OnBird)
        {
            StartCoroutine(OnMouseClick());
        }
    }

    // The function you want to trigger
    public void OnBirdClicked()
    {   
        StartCoroutine(BirdClickSequence());
    }

    public IEnumerator OnMouseClick()
    {
        GameObject currentItem = Symphony[currentIndex];
        Debug.Log($"Index {currentIndex}: {currentItem}");

        // 2. Move to the next index, wrapping back to 0 if we hit the end
        // Formula: (0 + 1) % 3 = 1 -> (1 + 1) % 3 = 2 -> (2 + 1) % 3 = 0
        currentIndex = (currentIndex + 1) % Symphony.Count;
        Bird.Animate();
        prefabSpawner.SpawnNextFromCombination(Symphony); // Call the spawn function
        yield return new WaitForSeconds(1f); // Wait until the end of the frame to ensure the click is registered 
    }


//---MAJOR FUNCTION: Handles the entire sequence of events when the window is clicked

    private IEnumerator BirdClickSequence()
    {
        //SET-UP: CHANGE OF CAMERA, DISABLE MOVEMENT, FADE OUT, WAIT, FADE IN
        desactivatemovement(false);

        FadeOut();
        yield return new WaitForSeconds(2f); 

        ToggleCameras(); // Switch to the second camera

        FadeIn();

        if (firsttimeinteract)
        {
        firsttimeinteract = false;
        Symphony = GetRandomPrefabCombination(comblength); // Assuming you want a combination of length 4
        }
                // Bird TASK MECHANISM
        OnBird = true;
        CanCloseTab = true;
        Debug.Log("Bird task is now active. Press Escape to close.");; // Wait until the end of the frame to ensure the click is registered
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

    public IEnumerator CloseBirdTask()
    {
        // Logic to close the bird task and return to the main game
        Debug.Log("Closing Bird Task and returning to main game.");
        CanCloseTab = false;
        OnBird = false;
        FadeOut();
        yield return new WaitForSeconds(2f); // Wait for fade out to complete
        ToggleCameras(); // Switch back to the main camera
        FadeIn();
        desactivatemovement(true);

    }
}