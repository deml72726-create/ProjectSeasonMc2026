using UnityEngine;

public class XylophoneTrigger : MonoBehaviour
{
    public SpriteRenderer highlight;
    private bool canInteract = false;

    void Update()
    {
        // Only trigger if player is near and NOT already in the minigame
        if (canInteract && !GameManagerPiano.Instance.isInMinigame)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                GameManagerPiano.Instance.TriggerEnter();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = true;
            if (highlight) highlight.color = Color.yellow;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = false;
            if (highlight) highlight.color = Color.white;
        }
    }
}