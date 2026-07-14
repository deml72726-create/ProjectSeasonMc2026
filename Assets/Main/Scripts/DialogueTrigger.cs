using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [TextArea(3, 10)]
    public string[] dialogueLines; // Put your lines here in the Inspector!

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Tell the manager to start the chat with these lines
            DialogueManager.Instance.StartDialogue(dialogueLines);
        }
    }
}