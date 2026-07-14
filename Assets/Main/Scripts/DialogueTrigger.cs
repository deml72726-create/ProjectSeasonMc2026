using System.Collections;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [TextArea(3, 10)]
    public string[] dialogueLines;
    public float dialogueDuration = 7.0f;
    private Coroutine closeCoroutine;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartDialogue(dialogueLines);

                if (closeCoroutine != null)
                {
                    StopCoroutine(closeCoroutine);
                }
                closeCoroutine = StartCoroutine(AutoCloseDialogue());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (closeCoroutine != null)
            {
                StopCoroutine(closeCoroutine);
            }

            if (DialogueManager.Instance != null && DialogueManager.Instance.bubbleCanvasGroup != null)
            {
                DialogueManager.Instance.bubbleCanvasGroup.alpha = 0.0f;
            }
        }
    }

    IEnumerator AutoCloseDialogue()
    {
        yield return new WaitForSeconds(dialogueDuration);

        if (DialogueManager.Instance != null && DialogueManager.Instance.bubbleCanvasGroup != null)
        {
            DialogueManager.Instance.bubbleCanvasGroup.alpha = 0.0f;
        }
    }
}