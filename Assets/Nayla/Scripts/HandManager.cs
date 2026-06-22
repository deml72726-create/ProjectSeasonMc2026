using UnityEngine;

public class HandManager : MonoBehaviour
{
    public Transform handSlot;
    public ItemPickup itemInHand;
    public InventoryManager inventoryManager;

    public void EquipItem(ItemPickup item)
    {
        itemInHand = item;
        item.transform.SetParent(handSlot);
        item.transform.localPosition = Vector3.zero;
        item.GetComponent<Collider2D>().enabled = false;
        
        // Add to inventory if not already there
        inventoryManager.AddItem(item.itemData);
    }

    public void DropItem()
    {
        if (itemInHand != null)
        {
            itemInHand.transform.SetParent(null);
            itemInHand.GetComponent<Collider2D>().enabled = true;
            itemInHand = null;
        }
    }
}