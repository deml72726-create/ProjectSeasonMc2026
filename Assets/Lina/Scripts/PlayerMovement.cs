using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D playerRb;
    public float speed = 5f;
    public SpriteRenderer spriteRenderer;
    public Transform handSlot;
    public InventoryManager invManager;

    private float input;
    private ItemPickup itemInRange;
    private int selectedIndex = 0;

    void Update()
    {
        input = Input.GetAxisRaw("Horizontal");
        if (input != 0) spriteRenderer.flipX = (input < 0);

        if (Input.GetKeyDown(KeyCode.E) && itemInRange != null && invManager.inventory.Count < invManager.maxSlots)
        {
            invManager.AddItem(itemInRange.itemData);
            itemInRange.gameObject.SetActive(false);
            selectedIndex = invManager.inventory.Count - 1; // Auto-select the one you just picked up
            EquipHand(selectedIndex);
            itemInRange = null;
        }

        if (Input.GetKeyDown(KeyCode.Q) && invManager.inventory.Count > 0) DropItem(selectedIndex);

        // Switch Slots: Now handles empty slots correctly
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeSelection(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeSelection(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeSelection(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) ChangeSelection(3);
    }

    void FixedUpdate() => playerRb.linearVelocity = new Vector2(input * speed, playerRb.linearVelocity.y);

    void ChangeSelection(int index)
    {
        selectedIndex = index;
        EquipHand(selectedIndex);
    }

    void EquipHand(int index)
    {
        // Always clear the hand first
        foreach (Transform child in handSlot) Destroy(child.gameObject);

        // If the slot is within range AND has an item, spawn it
        if (index < invManager.inventory.Count)
        {
            GameObject go = Instantiate(invManager.inventory[index].prefab, handSlot.position, Quaternion.identity);
            go.transform.SetParent(handSlot);
            if (go.GetComponent<Collider2D>()) go.GetComponent<Collider2D>().enabled = false;
        }
        // If the slot is empty, the loop above already cleared the hand, so it stays empty!
    }

    void DropItem(int index)
    {
        ItemData itemToDrop = invManager.inventory[index];
        invManager.RemoveItem(itemToDrop);
        
        GameObject droppedObj = Instantiate(itemToDrop.prefab, transform.position + Vector3.right, Quaternion.identity);
        if (droppedObj.GetComponent<Collider2D>()) droppedObj.GetComponent<Collider2D>().enabled = true;
        
        // After dropping, refresh the hand
        selectedIndex = 0; 
        EquipHand(selectedIndex);
    }

    private void OnTriggerEnter2D(Collider2D col) { if (col.TryGetComponent(out ItemPickup item)) itemInRange = item; }
    private void OnTriggerExit2D(Collider2D col) { if (col.TryGetComponent(out ItemPickup item) && itemInRange == item) itemInRange = null; }
}