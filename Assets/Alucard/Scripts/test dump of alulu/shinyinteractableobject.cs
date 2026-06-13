using UnityEngine;
using UnityEngine.InputSystem;

public class shinyinteractableo : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Material outlineMaterial;
    private Material defaultMaterial;
    public string interactionText = "This is a dusty shelf.";
    private bool isHovered = false;

    void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        defaultMaterial = spriteRenderer.material;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
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
        if (spriteRenderer != null && outlineMaterial != null)
        {
            spriteRenderer.material = outlineMaterial;
        }
    }

    void OnHoverExit()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.material = defaultMaterial;
        }
    }

    void OnClick()
    {
        Debug.Log(interactionText);
    }
}