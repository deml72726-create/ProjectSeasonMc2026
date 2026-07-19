using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    
    public CanvasGroup bubbleCanvasGroup;
    public TMP_Text bubbleText;
    public float textSpeed = 0.05f;
    
    private string[] currentLines;
    private int index;
    private bool isTyping;
    private Coroutine typingCoroutine;

    void Awake() 
    { 
        Instance = this; 
    }

    void Start() 
    { 
        bubbleCanvasGroup.alpha = 0; 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && isTyping)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            bubbleText.text = currentLines[index];
            isTyping = false;
        }
        else if (Input.GetKeyDown(KeyCode.X) && !isTyping && bubbleCanvasGroup.alpha > 0)
        {
            NextLine();
        }
    }

    public void StartDialogue(string[] lines)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        
        currentLines = lines;
        index = 0;
        bubbleCanvasGroup.alpha = 1;
        typingCoroutine = StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        bubbleText.text = "";
        
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
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            typingCoroutine = StartCoroutine(TypeLine());
        }
        else
        {
            bubbleCanvasGroup.alpha = 0;
        }
    }
}