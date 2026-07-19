using UnityEngine;

public class XylophonePlacer : MonoBehaviour
{
    public ItemData xylophoneItemData;
    public GameObject groundXylophoneVisual; 
    public GameObject xylophoneOverlayCanvas; 
    public PlayerMovement playerMovement;

    private static bool isPlaced = false;

    void Start()
    {
        if (groundXylophoneVisual != null && groundXylophoneVisual != gameObject)
        {
            groundXylophoneVisual.SetActive(isPlaced);
        }
    }

    void Update()
    {
        if (xylophoneOverlayCanvas != null && xylophoneOverlayCanvas.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseXylophoneOverlay();
            }
        }
    }

    public void InteractWithGround()
    {
        if (!isPlaced)
        {
            TryPlaceXylophone();
        }
        else
        {
            OpenXylophoneOverlay();
        }
    }

    private void TryPlaceXylophone()
    {
        InventoryManager inv = FindFirstObjectByType<InventoryManager>();
        bool hasXylo = false;

        if (inv != null && xylophoneItemData != null)
        {
            if (inv.inventory.Contains(xylophoneItemData))
            {
                hasXylo = true;
            }
        }

        if (hasXylo)
        {
            inv.RemoveItem(xylophoneItemData);
            
            InventoryUI invUI = FindFirstObjectByType<InventoryUI>();
            if (invUI != null) invUI.UpdateUI();
            
            if (groundXylophoneVisual != null)
            {
                groundXylophoneVisual.SetActive(true);
                SpriteRenderer spriteRenderer = groundXylophoneVisual.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.enabled = true;
                }
            }

            isPlaced = true;
        }
    }

    public void DisableInteractionPermanently()
    {
        CloseXylophoneOverlay();
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }
    }

    public void OpenXylophoneOverlay()
    {
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        if (xylophoneOverlayCanvas != null)
        {
            xylophoneOverlayCanvas.SetActive(true);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseXylophoneOverlay()
    {
        if (xylophoneOverlayCanvas != null)
        {
            xylophoneOverlayCanvas.SetActive(false);
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}