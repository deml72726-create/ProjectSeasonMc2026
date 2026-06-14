using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuButtonEffects : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Zoom Settings")]
    public float selectedScale = 1.1f; 
    public float pressedScale = 0.9f;  
    public float animationSpeed = 15f;

    private Vector3 targetScale;
    private Vector3 originalScale;
    private bool isMouseOver = false;

    void Awake()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    // --- NEW: Reset scale when the panel is opened/closed ---
    void OnEnable()
    {
        transform.localScale = originalScale;
        targetScale = originalScale;
        isMouseOver = false;
    }

    void OnDisable()
    {
        transform.localScale = originalScale;
        targetScale = originalScale;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
    }

    public void OnSelect(BaseEventData eventData)
    {
        targetScale = originalScale * selectedScale;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (!isMouseOver) targetScale = originalScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
        targetScale = originalScale * selectedScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        if (EventSystem.current.currentSelectedGameObject != gameObject)
            targetScale = originalScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        targetScale = originalScale * pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        targetScale = originalScale * selectedScale;
    }
}