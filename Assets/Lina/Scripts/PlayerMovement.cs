using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D playerRb;
    public float speed = 5f;
    private SpriteRenderer spriteRenderer; 
    private Animator animator; 
    public Transform handSlot;
    public InventoryManager invManager;

    // --- DIRECT FOOTSTEP FIELDS ---
    public AudioSource audioSource;
    public AudioClip floorClip;
    public AudioClip grassClip;
    public LayerMask groundLayer;
    public float rayDistance = 5.0f;
    private float stepTimer;
    public float timeBetweenSteps = 0.5f;

    private ItemPickup itemInRange;
    private int selectedIndex = 0;
    private bool isBusy = false; 

    void Awake() 
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>(); 
    }

    void Update()
    {
        float input = Input.GetAxisRaw("Horizontal");
        
        // 1. Handle Flip
        if (input != 0 && spriteRenderer != null) 
            spriteRenderer.flipX = (input < 0);
        
        // 2. Handle Physics Movement
        playerRb.linearVelocity = new Vector2(input * speed, playerRb.linearVelocity.y);

        // 3. Direct Footstep Logic
        if (Mathf.Abs(input) > 0.1f)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0)
            {
                PlayFootstepDirectly();
                stepTimer = timeBetweenSteps;
            }
        }

        // 4. Update Animator
        if (animator != null)
            animator.SetFloat("Speed", Mathf.Abs(input));

        // Pickup (E)
        if (Input.GetKeyDown(KeyCode.E) && itemInRange != null && invManager.inventory.Count < invManager.maxSlots && !isBusy)
            StartCoroutine(PickupRoutine(itemInRange));

        // Drop (Q)
        if (Input.GetKeyDown(KeyCode.Q) && invManager.inventory.Count > 0 && !isBusy)
            DropItem(selectedIndex);

        // Switching Slots
        if (!isBusy)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) { selectedIndex = 0; RefreshHand(); }
            if (Input.GetKeyDown(KeyCode.Alpha2)) { selectedIndex = 1; RefreshHand(); }
            if (Input.GetKeyDown(KeyCode.Alpha3)) { selectedIndex = 2; RefreshHand(); }
            if (Input.GetKeyDown(KeyCode.Alpha4)) { selectedIndex = 3; RefreshHand(); }
        }
    }

    void PlayFootstepDirectly()
    {
        // Ignore the player's own layer (typically Default or Player)
        // We cast from a point slightly offset from center to avoid hitting self
        Vector2 origin = (Vector2)transform.position + Vector2.down * 0.5f;
        
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayDistance, groundLayer);
        Debug.DrawRay(origin, Vector2.down * rayDistance, Color.cyan, 0.5f);

        if (hit.collider != null)
        {
            // Debug to confirm we actually hit the floor
            Debug.Log("Hit floor: " + hit.collider.name);
            
            if (hit.collider.CompareTag("Grass")) audioSource.PlayOneShot(grassClip);
            else if (hit.collider.CompareTag("Floor")) audioSource.PlayOneShot(floorClip);
        }
        else
        {
            Debug.Log("Raycast hit nothing! Check if floor is on 'Ground' layer.");
        }
    }

    IEnumerator PickupRoutine(ItemPickup item)
    {
        isBusy = true; 
        foreach (Transform child in handSlot) DestroyImmediate(child.gameObject);
        invManager.AddItem(item.itemData);
        item.gameObject.SetActive(false); 
        selectedIndex = invManager.inventory.Count - 1;
        GameObject visualCopy = Instantiate(item.itemData.prefab, item.transform.position, Quaternion.identity);
        yield return StartCoroutine(MoveToHeadSmoothly(visualCopy));
        Destroy(visualCopy);
        RefreshHand();
        isBusy = false; 
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
            box_move_or_adjust(itemObj, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void box_move_or_adjust(GameObject visual, float t) {}

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