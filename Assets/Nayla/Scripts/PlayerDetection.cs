using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Trigger the transition to BendingDown
            animator.SetBool("isTalking", true); 
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Revert the transition back to Idle
            animator.SetBool("isTalking", false);
        }
    }
}