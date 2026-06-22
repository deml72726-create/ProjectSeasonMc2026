using System.Collections;
using UnityEngine;

public class roomtp : MonoBehaviour
{
    public Transform playerDestination;
    public CanvasGroup fadeGroup;
    public float fadeSpeed = 3.0f;

    private bool isTransitioning = false;

    void Start()
    {
        if (fadeGroup != null)
        {
            fadeGroup.alpha = 0;
            fadeGroup.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            StartCoroutine(PerformTransition(other.gameObject));
        }
    }

    IEnumerator PerformTransition(GameObject player)
    {
        isTransitioning = true;

        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (movement != null)
        {
            movement.enabled = false;
        }
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (fadeGroup != null)
        {
            fadeGroup.gameObject.SetActive(true);
            while (fadeGroup.alpha < 1)
            {
                fadeGroup.alpha += Time.deltaTime * fadeSpeed;
                yield return null;
            }
        }

        if (playerDestination != null)
        {
            player.transform.position = playerDestination.position;
        }

        yield return new WaitForSeconds(0.2f);

        if (fadeGroup != null)
        {
            while (fadeGroup.alpha > 0)
            {
                fadeGroup.alpha -= Time.deltaTime * fadeSpeed;
                yield return null;
            }
            fadeGroup.alpha = 0;
            fadeGroup.gameObject.SetActive(false);
        }

        if (movement != null)
        {
            movement.enabled = true;
        }

        isTransitioning = false;
    }
}