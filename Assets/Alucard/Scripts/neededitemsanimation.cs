using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class neededitemsanimation : MonoBehaviour
{
    public Sprite itemSprite;
    public string itemName = "Key Item";
    public string itemDescription = "A special item used in your adventure.";
    public UnityEvent onPickupEvent;
    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (NewItemPopup.Instance != null)
            {
                NewItemPopup.Instance.ShowUnlockPopup(itemSprite, itemName, itemDescription);
            }

            if (onPickupEvent != null)
            {
                onPickupEvent.Invoke();
            }

            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}