using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData itemData;

    [System.Obsolete]
    public void PickUp(Transform handSlot)
    {
        if (handSlot.childCount > 0)
        {
            ItemPickup currentItem = handSlot.GetChild(0).GetComponent<ItemPickup>();
            if (currentItem != null) currentItem.Drop();
        }

        InventoryManager manager = FindObjectOfType<InventoryManager>();
        if (manager != null)
        {
            manager.AddItem(itemData);
            
            InventoryUI invUI = FindObjectOfType<InventoryUI>();
            if (invUI != null)
            {
                invUI.UpdateUI();
            }
        }

        if (NewItemPopup.Instance != null && itemData != null)
        {
            NewItemPopup.Instance.ShowUnlockPopup(itemData.icon, itemData.itemName);
        }

        transform.SetParent(handSlot);
        transform.localPosition = Vector3.zero;
        
        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            col.enabled = false;
        }

        if (GetComponent<Rigidbody2D>()) GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
    }

    [System.Obsolete]
    public void Drop()
    {
        transform.SetParent(null);
        
        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            col.enabled = true;
        }

        if (GetComponent<Rigidbody2D>())
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocity = Vector2.zero;
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