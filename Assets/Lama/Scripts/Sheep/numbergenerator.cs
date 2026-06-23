using UnityEngine;
using TMPro;

public class RandomNumberGenerator : MonoBehaviour
{
    public int minNumber = 1;
    public int maxNumber = 10; 

    private TextMeshPro myText;
    
    // 1. We declare these at the top so ALL functions can access them
    private int storedNumber; 
    private GameManager manager;
    private bool hasBeenClicked = false; // Prevents clicking the same object twice

void Start()
{
    myText = GetComponentInChildren<TextMeshPro>();

    if (myText == null)
    {
        Debug.LogError("Could not find a TextMeshPro component!");
    }

    // 1. Find the GameManager FIRST
    manager = FindAnyObjectByType<GameManager>();
    
    if (manager == null)
    {
        Debug.LogError("The clone couldn't find the Game Manager!");
        // Safety net just in case the manager is missing
        storedNumber = Random.Range(minNumber, maxNumber); 
    }
    else
    {
        // 2. Ask the manager for the 40% weighted number!
        storedNumber = manager.GetNextWeightedNumber();
    }

    // 3. Finally, update the text to show the assigned number
    if (myText != null)
    {
        myText.text = storedNumber.ToString();
    }
}

    // 4. This built-in Unity function automatically triggers when the mouse clicks this object
    void OnMouseDown()
    {
        // 5. Check if we have the manager and haven't clicked this clone yet
        if (manager != null && hasBeenClicked == false)
        {
            // Send the stored number
            manager.ReceiveCloneNumber(storedNumber);
            
            // Lock this clone so the player can't spam click it to get a 4-digit code instantly
            hasBeenClicked = true; 

            // Optional: Change the color to show it was selected!
            myText.color = Color.green; 
            Debug.Log("Clone clicked! Number sent: " + storedNumber);
        }
    }
}