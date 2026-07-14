using UnityEngine;
using UnityEngine.InputSystem;

public class CardPickup : MonoBehaviour
{
    public UICardGameManager cardGameManager;
    public Sprite cardItemSprite;

    void Update()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                if (cardGameManager != null)
                {
                    cardGameManager.InsertMissingCard();
                }

                if (NewItemPopup.Instance != null)
                {
                    NewItemPopup.Instance.ShowUnlockPopup(cardItemSprite, "Missing Card", "A lost drawing found in the room. It allows you to play the card game.");
                }

                gameObject.SetActive(false);
            }
        }
    }
}