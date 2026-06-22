using UnityEngine;

public class CloudFloat : MonoBehaviour
{
    [Header("Horizontal Movement (Left / Right)")]
    public float horizontalSpeed = 1.5f;
    public float horizontalRange = 25f; // How many pixels/units it drifts left and right

    [Header("Vertical Movement (Up / Down)")]
    public float verticalSpeed = 0.8f;   // Usually looks best when slower than horizontal
    public float verticalRange = 8f;     // "Really slight" alteration up and down

    [Header("Randomization")]
    [Tooltip("Check this if you have multiple clouds so they don't move in perfect sync!")]
    public bool randomizeStartingOffset = true;

    private Vector2 startPosition;
    private float randomXOffset;
    private float randomYOffset;
    private RectTransform rectTransform;
    private bool isUIElement;

    void Start()
    {
        // Check if this is a UI element (Canvas) or a standard 2D Sprite game object
        rectTransform = GetComponent<RectTransform>();
        isUIElement = rectTransform != null;

        if (isUIElement)
        {
            startPosition = rectTransform.anchoredPosition;
        }
        else
        {
            startPosition = transform.position;
        }

        // Generate offsets so cloning this object creates separate, unique movement patterns
        if (randomizeStartingOffset)
        {
            randomXOffset = Random.Range(0f, 100f);
            randomYOffset = Random.Range(0f, 100f);
        }
    }

    void Update()
    {
        // 1. Calculate the smooth math wave positions using Time.time
        float newX = Mathf.Sin((Time.time * horizontalSpeed) + randomXOffset) * horizontalRange;
        float newY = Mathf.Sin((Time.time * verticalSpeed) + randomYOffset) * verticalRange;

        // 2. Apply the drift relative to the object's original starting point
        if (isUIElement)
        {
            rectTransform.anchoredPosition = startPosition + new Vector2(newX, newY);
        }
        else
        {
            transform.position = startPosition + new Vector2(newX, newY);
        }
    }
}