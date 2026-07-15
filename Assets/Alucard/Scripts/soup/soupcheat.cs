using UnityEngine;
using UnityEngine.InputSystem;

public class SoupCheat : MonoBehaviour
{
    public SoupPuzzleManager puzzleManager;
    public ItemData ratItemData;

    void Update()
    {
        if (Keyboard.current.minusKey.wasPressedThisFrame)
        {
            InventoryManager inv = FindFirstObjectByType<InventoryManager>();
            
            if (inv != null && ratItemData != null)
            {
                inv.inventory.Add(ratItemData);
                
                // Try calling a refresh method if your inventory has one
                // Common names for this include 'UpdateUI', 'RefreshInventory', or 'DisplayItems'
                // Uncomment the line below if you know the method name:
                // inv.UpdateUI(); 
                
                Debug.Log("Rat added to inventory!");
            }

            if (puzzleManager != null)
            {
                // Unlocking the cursor so you can actually click
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                
                puzzleManager.OpenSoupPuzzle();
            }
        }
    }
}