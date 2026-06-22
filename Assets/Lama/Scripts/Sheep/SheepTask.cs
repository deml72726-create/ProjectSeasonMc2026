using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Simple Singleton pattern to easily access the GameManager
    public static GameManager Instance { get; private set; }

    public PlayerMovement playerMovementScript;

    [Tooltip("Assign the black full-screen UI Image here.")]
    public Image fadeOverlay; 
    public GameObject Rideau; // Assign the curtain image in the inspector
    public GameObject Rideauleft; // Assign the curtain image in the inspector
    
    public GameObject mainCamera;
    public WindowInteractable window;
    
    [Tooltip("Drag your Second Camera GameObject here")]
    public GameObject secondCamera;
    public float fadeDuration = 2f;
    
    // 1. Define the options and assign their integer values
    public enum TaskMode 
    { 
        ModeOne = 1, 
        ModeTwo = 2, 
        ModeThree = 3 
    }

    // 2. Create the visible variable in your script
    public TaskMode currentTask = TaskMode.ModeOne;
    public string targetCombination = "123456789";
    public string currentCombination = "";
    public int lastClickedNumber = -1;
    public bool didwin = false; // Flag to track if the player has won
    public void ReceiveCloneNumber(int incomingNumber)
    {
        if (puzzleCompleted) return;

        // ---> ADD THIS LINE: Remember this number for the next sheep that spawns! <---
        lastClickedNumber = incomingNumber; 

        currentCombination += incomingNumber.ToString();

        if (targetCombination.StartsWith(currentCombination))
        {
            Debug.Log("Correct so far: " + currentCombination);

            if (currentCombination == targetCombination)
            {
                puzzleCompleted = true; 
                didwin = true;
            }
        }
        else
        {
           puzzleCompleted = true;
           didwin=false;
            
            // Optional: If they fail, you can reset the last clicked number so it goes back to pure random
            // lastClickedNumber = -1; 
        }
    }

    public bool puzzleCompleted = false; // This is the master switch for the sequence    

    public GameObject sheepSpawner;

    public bool comblengreached = false;
    public GameObject TaskOneneeds; 

    public void HideMyUI()
    {
        // This instantly makes the empty object and everything inside it disappear
        TaskOneneeds.SetActive(false); 
    }
    public TMP_InputField playerInput;    // Where the player types their guess
    public TextMeshProUGUI feedbackText;
    public int lengthofcombination; // The required length of the combination for the current task
    public bool didntfail = true; // Flag to track if the player has failed
    public bool didntreach9 = true; // Flag to track if the player has reached



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
    }

    // The function you want to trigger
    public void OnWindowClicked()
    {   
        StartCoroutine(WindowClickSequence());
    }



//---MAJOR FUNCTION: Handles the entire sequence of events when the window is clicked

    private IEnumerator WindowClickSequence()
    {
        //SET-UP: CHANGE OF CAMERA, DISABLE MOVEMENT, FADE OUT, WAIT, FADE IN
        desactivatemovement(false);

        FadeOut();
        yield return new WaitForSeconds(2f); 

        EnableSecondCamera();

        FadeIn();
        ToggleImageVisibility();

        // SHEEP TASK MECHANISM

        

     switch (currentTask) 
        {
            case TaskMode.ModeOne:
                yield return StartCoroutine(TaskOne());
                break;

            case TaskMode.ModeTwo:
                yield return StartCoroutine(TaskTwo());
                break;

            case TaskMode.ModeThree:
                yield return StartCoroutine(TaskThree());
                break;

        }

        //GOING BACK TO NORMAL
        FadeOut();
        yield return new WaitForSeconds(2f);
        ToggleImageVisibility();
        yield return new WaitForSeconds(2f); 
        EnableMainCamera();
        desactivatemovement(true);
        FadeIn();

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
    public void ToggleImageVisibility()
    {
        // This gets the current status and flips it
        bool isCurrentlyActive = Rideau.activeSelf;
        Rideau.SetActive(!isCurrentlyActive);
        Rideauleft.SetActive(!isCurrentlyActive);
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

        private void checkcombleng()
    {
        if (currentCombination.Length == lengthofcombination) // Replace with your desired combination length
        {
            Debug.Log("Combination complete! Shutting down spawner.");
            sheepSpawner.SetActive(false);
        } 
    }

    public int GetNextWeightedNumber()
    {
        // If nothing has been clicked yet, just return a normal random number
        if (lastClickedNumber == -1)
        {
            return Random.Range(1, 10);
        }

        // Roll a 40% chance
        float chance = Random.value; 

        if (chance <= 1f) 
        {
            // 40% of the time, pick the clicked number, -1, or +1
            int offset = Random.Range(-1, 2); // Picks -1, 0, or 1
            int result = lastClickedNumber + offset;

            // Clamp prevents the number from becoming 0 or 10
            return Mathf.Clamp(result, 1, 9); 
        }
        else
        {
            // 60% of the time, pick a totally random number
            return Random.Range(1, 10);
        }
    }
    public void TriggerGameWin()
    {
        puzzleCompleted = true; 

        // 1. Find all GameObjects in the scene with this specific tag
        GameObject[] allSheepObjects = GameObject.FindGameObjectsWithTag("Sheep");

        // 2. Loop through them
        foreach (GameObject sheep in allSheepObjects)
        {
            // 3. Because a Tag only finds the GameObject, we have to "reach inside" 
            // it to grab your specific script before we can trigger the win state.
            SheepMovement script = sheep.GetComponent<SheepMovement>();
            
            if (script != null)
            {
                script.TriggerWin(); 
            }
        }
        window.SetInteractable(); // Disable further interaction with the window
    }

  public void TriggerGameLose()
    {
        puzzleCompleted = false; 
        Debug.Log("Game Lost! Triggering lose state for all sheep.");

        // 1. Find all GameObjects in the scene with this specific tag
        GameObject[] allSheepObjects = GameObject.FindGameObjectsWithTag("Sheep");

        // 2. Loop through them
        foreach (GameObject sheep in allSheepObjects)
        {
            // 3. Because a Tag only finds the GameObject, we have to "reach inside" 
            // it to grab your specific script before we can trigger the win state.
            SheepMovement script = sheep.GetComponent<SheepMovement>();
            
            if (script != null)
            {
                script.TriggerLose(); 
            }
        }
    }


    
    //----TASKSFUNCTIONS
    //TASK1-------
    public IEnumerator TaskOne()
    {
        lengthofcombination = 4; // Set the desired combination length
        sheepSpawner.SetActive(true);
        while (!comblengreached)
        {
            checkcombleng();
            yield return null;
        }
        yield return new WaitForSeconds(2f); // Simulate some delay after combination is reached
        TaskOneneeds.SetActive(true); // Show the UI for Task One
        yield return new WaitForSeconds(4f); // Simulate some delay for Task One
        string guessedCode = playerInput.text;
        if (guessedCode == currentCombination)
        {
            Debug.Log("Code Matched!");
            feedbackText.text = "Access Granted!";
            feedbackText.color = Color.green;
            
            // --> Add your win logic here (e.g., open a door, load next level) <--
        }
        else
        {
            Debug.Log("Wrong Code!");
            feedbackText.text = "Error: Incorrect Code";
            feedbackText.color = Color.red;
            
            // Clear the input field so the player can try again
            playerInput.text = "";
        }

        yield return new WaitForSeconds(5f); // Simulate some delay for Task One
    }   


    //TASK2-------
    private IEnumerator TaskTwo()
    {
        lengthofcombination = Random.Range(3, 10); // Set the desired combination length for Task Two
        sheepSpawner.SetActive(true);
        while (!comblengreached)
        {
            checkcombleng();
            yield return null;
        }
        yield return new WaitForSeconds(2f); // Simulate some delay after combination is reached
        Debug.Log("Executing Task Two");
    }


    //TASK3-------
    private IEnumerator TaskThree()
    {
        currentCombination = ""; // Reset the combination for Task Three
        lastClickedNumber = -1; // Reset the last clicked number for Task Three
        sheepSpawner.SetActive(true); // Reset the flag for Task Three
  
        yield return new WaitUntil(() => puzzleCompleted == true);

        sheepSpawner.SetActive(false); // Disable the sheep spawner after the task is completed
        if (didwin==true)
        {
            TriggerGameWin();
        }
        else
        {
        // Find every sheep currently in the scene
        TriggerGameLose();
        }
        yield return new WaitForSeconds(2f); // Simulate some delay after win/lose is triggered
    }



}