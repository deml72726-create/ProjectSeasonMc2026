using UnityEngine;
using TMPro;

public class RandomLetterGenerator : MonoBehaviour
{
    public char minletter = 'A';
    public char maxletter = 'Z'; 
    public char storedLetter;

    private TextMeshPro myText;
    private GameManagerSoup manager;

    void Start()
    {
        // 1. Find the TextMeshPro component
        myText = GetComponentInChildren<TextMeshPro>();

        if (myText == null)
        {
            Debug.LogError("Could not find a TextMeshPro component!");
        }

        // 2. Find the GameManager
        manager = FindAnyObjectByType<GameManagerSoup>();
        
        if (manager == null)
        {
            Debug.LogError("The clone couldn't find the Game Manager! Using fallback random letter.");
            // Generates a random letter from A-Z (adding +1 because Random.Range with ints/chars is exclusive at the max value)
            storedLetter = (char)Random.Range((int)minletter, (int)maxletter + 1); 
        }
        else
        {
            // If you have a function in your GameManagerSoup to get letters, call it here, like:
            // storedLetter = manager.GetRandomLetter();
            // Otherwise, we fall back to the basic generation:
            storedLetter = (char)Random.Range((int)minletter, (int)maxletter + 1);
        }

        // 3. CRITICAL FIX: Actually display the stored letter on the TextMeshPro component!
        if (myText != null)
        {
            myText.text = storedLetter.ToString();
        }
    }
}
