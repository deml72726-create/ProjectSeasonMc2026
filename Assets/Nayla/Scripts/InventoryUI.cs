using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Image[] slotIcons; 
    public InventoryManager manager; 

    public void UpdateUI()
    {
        for (int i = 0; i < slotIcons.Length; i++)
        {
            if (i < manager.inventory.Count)
            {
                slotIcons[i].sprite = manager.inventory[i].icon;
                slotIcons[i].enabled = true;
                // Don't forget to set "Preserve Aspect" to ON in these Images!
            }
            else
            {
                slotIcons[i].enabled = false;
            }
        }
    }
}