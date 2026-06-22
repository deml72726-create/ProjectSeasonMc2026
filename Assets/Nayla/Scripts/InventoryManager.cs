using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public List<ItemData> inventory = new List<ItemData>();
    public int maxSlots = 4;

    public void AddItem(ItemData item)
    {
        if (inventory.Count < maxSlots)
        {
            inventory.Add(item);
            FindObjectOfType<InventoryUI>().UpdateUI();
        }
    }

    public void RemoveItem(ItemData item)
    {
        inventory.Remove(item);
        FindObjectOfType<InventoryUI>().UpdateUI();
    }
}