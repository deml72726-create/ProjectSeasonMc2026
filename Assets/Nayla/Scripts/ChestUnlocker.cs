using UnityEngine;

public class ChestUnlocker : MonoBehaviour
{
    [Header("Settings")]
    public int keyItemID = 67; 
    public GameObject itemToDrop; 
    public Transform dropLocation; 
    
    [Header("Hand Reference")]
    public Transform handSlot; // Drag the Player's "Hand" object here in the Inspector

    [Header("Interaction Settings")]
    public bool isPlayerInRange = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TryUnlock();
        }
    }

    public void TryUnlock()
    {
        InventoryManager manager = FindObjectOfType<InventoryManager>();

        if (manager == null) return;

        ItemData keyItem = manager.inventory.Find(item => item.itemID == keyItemID);

        if (keyItem != null)
        {
            // 1. Remove from Inventory
            manager.RemoveItem(keyItem);

            // 2. Remove from Hand (Destroy the visual object)
            if (handSlot != null && handSlot.childCount > 0)
            {
                // Assuming the item in the hand has the ItemPickup script or is the key object
                Destroy(handSlot.GetChild(0).gameObject);
            }

            // 3. Spawn the reward
            if (itemToDrop != null)
            {
                Instantiate(itemToDrop, dropLocation.position, Quaternion.identity);
            }
            
            Debug.Log("Chest unlocked and key removed from hand!");
        }
        else
        {
            Debug.Log("It's locked! You need a key.");
        }
    }
}