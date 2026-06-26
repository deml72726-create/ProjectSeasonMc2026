using UnityEngine;

public class InteractionTrigger : MonoBehaviour
{
    public BirdController bird;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) bird.SetHighlight(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) bird.SetHighlight(false);
    }
}