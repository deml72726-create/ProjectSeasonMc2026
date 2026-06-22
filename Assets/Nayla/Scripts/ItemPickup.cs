using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData itemData; 

    public void PickUp(Transform handSlot)
    {
        Debug.Log("DEBUG: PickUp triggered for " + itemData.itemName);
        
        // SWAP LOGIC: If the hand is full, drop the current item first
        if (handSlot.childCount > 0)
        {
            ItemPickup currentItem = handSlot.GetChild(0).GetComponent<ItemPickup>();
            if (currentItem != null) currentItem.Drop();
        }

        InventoryManager manager = FindObjectOfType<InventoryManager>();
        manager.AddItem(itemData);

        transform.SetParent(handSlot);
        transform.localPosition = Vector3.zero;
        
        if (GetComponent<Collider2D>()) GetComponent<Collider2D>().enabled = false;
        if (GetComponent<Rigidbody2D>()) GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
    }

    public void Drop()
    {
        Debug.Log("DEBUG: Drop triggered for " + itemData.itemName);
        
        transform.SetParent(null);
        
        if (GetComponent<Collider2D>()) GetComponent<Collider2D>().enabled = true;
        if (GetComponent<Rigidbody2D>())
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocity = Vector2.zero; // Stop it from flying when dropped
            rb.WakeUp();
        }
        
        InventoryManager manager = FindObjectOfType<InventoryManager>();
        if (manager != null)
        {
            manager.RemoveItem(itemData);
            FindObjectOfType<InventoryUI>().UpdateUI();
        }
    }
}