using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D playerRb;
    public float speed = 5f;
    private SpriteRenderer spriteRenderer; 
    public Transform handSlot;
    public InventoryManager invManager;

    private ItemPickup itemInRange;
    private int selectedIndex = 0;
    private bool isBusy = false; // Prevents multiple pickup triggers

    void Awake() => spriteRenderer = GetComponentInChildren<SpriteRenderer>();

    void Update()
    {
        float input = Input.GetAxisRaw("Horizontal");
        if (input != 0 && spriteRenderer != null) spriteRenderer.flipX = (input < 0);
        playerRb.linearVelocity = new Vector2(input * speed, playerRb.linearVelocity.y);

        // Pickup (E) - Now blocked if isBusy is true
        if (Input.GetKeyDown(KeyCode.E) && itemInRange != null && invManager.inventory.Count < invManager.maxSlots && !isBusy)
        {
            StartCoroutine(PickupRoutine(itemInRange));
        }

        // Drop (Q)
        if (Input.GetKeyDown(KeyCode.Q) && invManager.inventory.Count > 0 && !isBusy)
        {
            DropItem(selectedIndex);
        }

        // Switching Slots
        if (!isBusy)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) { selectedIndex = 0; RefreshHand(); }
            if (Input.GetKeyDown(KeyCode.Alpha2)) { selectedIndex = 1; RefreshHand(); }
            if (Input.GetKeyDown(KeyCode.Alpha3)) { selectedIndex = 2; RefreshHand(); }
            if (Input.GetKeyDown(KeyCode.Alpha4)) { selectedIndex = 3; RefreshHand(); }
        }
    }

    IEnumerator PickupRoutine(ItemPickup item)
    {
        isBusy = true; // LOCK: No more inputs allowed
        
        // 1. Instant Cleanup
        foreach (Transform child in handSlot) DestroyImmediate(child.gameObject);
        
        // 2. Add to inventory and hide real world object immediately
        invManager.AddItem(item.itemData);
        item.gameObject.SetActive(false); 
        selectedIndex = invManager.inventory.Count - 1;
        
        // 3. Animation: visual copy travels to hand
        GameObject visualCopy = Instantiate(item.itemData.prefab, item.transform.position, Quaternion.identity);
        yield return StartCoroutine(MoveToHeadSmoothly(visualCopy));
        
        // 4. Cleanup
        Destroy(visualCopy);
        RefreshHand();
        
        isBusy = false; // UNLOCK: Ready for next interaction
        itemInRange = null;
    }

    void RefreshHand()
    {
        foreach (Transform child in handSlot) DestroyImmediate(child.gameObject);
        if (selectedIndex < invManager.inventory.Count)
        {
            GameObject go = Instantiate(invManager.inventory[selectedIndex].prefab, handSlot.position, Quaternion.identity);
            go.transform.SetParent(handSlot);
            go.transform.localPosition = Vector3.zero;
            if (!go.GetComponent<FloatEffect>()) go.AddComponent<FloatEffect>();
        }
    }

    IEnumerator MoveToHeadSmoothly(GameObject itemObj)
    {
        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 startPos = itemObj.transform.position;
        while (elapsed < duration)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            itemObj.transform.position = Vector3.Lerp(startPos, handSlot.position, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    void DropItem(int index)
    {
        ItemData itemToDrop = invManager.inventory[index];
        invManager.RemoveItem(itemToDrop);
        GameObject droppedObj = Instantiate(itemToDrop.prefab, transform.position, Quaternion.identity);
        if (droppedObj.GetComponent<Collider2D>()) droppedObj.GetComponent<Collider2D>().enabled = true;
        selectedIndex = 0; 
        RefreshHand();
    }

    private void OnTriggerEnter2D(Collider2D col) { if (col.TryGetComponent(out ItemPickup item)) itemInRange = item; }
    private void OnTriggerExit2D(Collider2D col) { if (col.TryGetComponent(out ItemPickup item) && itemInRange == item) itemInRange = null; }
}