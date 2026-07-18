using UnityEngine;

public class XylophonePlacer : MonoBehaviour
{
    public ItemData xylophoneItemData;
    public GameObject groundXylophoneVisual; // The 2D sprite on the ground
    public GameObject xylophoneOverlayCanvas; // Your UI button overlay
    public PlayerMovement playerMovement;

    private bool isPlaced = false;

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
        int xyloIndex = -1;

        if (inv != null && xylophoneItemData != null)
        {
            for (int i = 0; i < inv.inventory.Count; i++)
            {
                if (inv.inventory[i] == xylophoneItemData)
                {
                    hasXylo = true;
                    xyloIndex = i;
                    break;
                }
            }
        }

        if (hasXylo)
        {
            // Remove from inventory
            inv.inventory.RemoveAt(xyloIndex);
            
            // Show on the ground
            if (groundXylophoneVisual != null)
            {
                groundXylophoneVisual.SetActive(true);
            }

            isPlaced = true;
            Debug.Log("Xylophone placed on the ground!");
        }
        else
        {
            Debug.Log("I need to find a musical instrument first.");
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