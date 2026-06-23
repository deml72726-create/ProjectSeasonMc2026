using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class shinyinteractableo : MonoBehaviour
{
    public enum ObjectType { Sprite, UI }
    public ObjectType type;

    public SpriteRenderer spriteRenderer;
    public Image uiImage;
    public Material outlineMaterial;
    public string interactionText = "This is a dusty shelf.";

    private Material defaultMaterial;
    private RectTransform rectTransform;
    private bool isHovered = false;

    void Start()
    {
        if (type == ObjectType.Sprite)
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
            if (spriteRenderer != null)
            {
                defaultMaterial = spriteRenderer.material;
            }
        }
        else if (type == ObjectType.UI)
        {
            if (uiImage == null)
            {
                uiImage = GetComponent<Image>();
            }
            if (uiImage != null)
            {
                defaultMaterial = uiImage.material;
            }
            rectTransform = GetComponent<RectTransform>();
        }
    }

    void Update()
    {
        if (Mouse.current == null) return;

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        bool currentlyHovered = false;

        if (type == ObjectType.Sprite)
        {
            Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                currentlyHovered = true;
            }
        }
        else if (type == ObjectType.UI && rectTransform != null)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mouseScreenPosition, null))
            {
                currentlyHovered = true;
            }
        }

        if (currentlyHovered)
        {
            if (!isHovered)
            {
                isHovered = true;
                OnHoverEnter();
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                OnClick();
            }
        }
        else
        {
            if (isHovered)
            {
                isHovered = false;
                OnHoverExit();
            }
        }
    }

    void OnHoverEnter()
    {
        if (outlineMaterial == null) return;

        if (type == ObjectType.Sprite && spriteRenderer != null)
        {
            spriteRenderer.material = outlineMaterial;
        }
        else if (type == ObjectType.UI && uiImage != null)
        {
            uiImage.material = outlineMaterial;
        }
    }

    void OnHoverExit()
    {
        if (type == ObjectType.Sprite && spriteRenderer != null)
        {
            spriteRenderer.material = defaultMaterial;
        }
        else if (type == ObjectType.UI && uiImage != null)
        {
            uiImage.material = defaultMaterial;
        }
    }

    void OnClick()
    {
        Debug.Log(interactionText);
    }
}