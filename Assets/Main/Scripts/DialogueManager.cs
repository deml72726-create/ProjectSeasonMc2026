using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    
    public CanvasGroup bubbleCanvasGroup;
    public TMP_Text bubbleText;
    public float textSpeed = 0.05f; // Adjust for typing speed
    
    private string[] currentLines;
    private int index;
    private bool isTyping;

    void Awake() { Instance = this; }

    void Start() { bubbleCanvasGroup.alpha = 0; }

    void Update()
    {
        // Skip current line if X is pressed
        if (Input.GetKeyDown(KeyCode.X) && isTyping)
        {
            StopAllCoroutines();
            bubbleText.text = currentLines[index];
            isTyping = false;
        }
        // Move to next line if X is pressed and typing is finished
        else if (Input.GetKeyDown(KeyCode.X) && !isTyping && bubbleCanvasGroup.alpha > 0)
        {
            NextLine();
        }
    }

    public void StartDialogue(string[] lines)
    {
        currentLines = lines;
        index = 0;
        bubbleCanvasGroup.alpha = 1;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        bubbleText.text = ""; // Clear existing text
        
        // Typewriter effect
        foreach (char c in currentLines[index].ToCharArray())
        {
            bubbleText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        
        isTyping = false;
    }

    void NextLine()
    {
        if (index < currentLines.Length - 1)
        {
            index++;
            StartCoroutine(TypeLine());
        }
        else
        {
            bubbleCanvasGroup.alpha = 0; // End of dialogue
        }
    }
}