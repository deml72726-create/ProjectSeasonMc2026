using TMPro;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
  [Header ("Dialogue Settings")]
  [SerializeField] private GameObject visualCue;

  [Header ("Ink Json")]
  [SerializeField] private TextAsset inkJson;
  private bool playerInRange ;

    private void Awake()
    {
        playerInRange = false;
        visualCue.SetActive(false);
    }
    private void Update()
    {
        if (playerInRange && !DialogueManager.GetInstance().isDialoguePlaying)
        {
            visualCue.SetActive(true);
            if (InputManager.GetInstance().GetInteractPressed())
            {
                DialogueManager.GetInstance().EnterDialogueMode(inkJson);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerInRange = true;
        }
        
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerInRange = false;
            visualCue.SetActive(false);
        }
        
    }

}
