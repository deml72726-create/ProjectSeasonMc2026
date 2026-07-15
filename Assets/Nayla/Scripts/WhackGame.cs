using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class WhackGame : MonoBehaviour
{
    public RectTransform otterRect; 
    public Image otterImage;
    public Sprite cuteSprite;
    public Sprite evilSprite;

    public float popSpeed = 10.0f;
    public float hideOffset = 180.0f;
    public float visibleDuration = 1.2f;

    private bool isUp = false;
    private bool isEvil = false;
    private float currentY;
    private float targetY;
    private float hiddenY;

    void Start()
    {
        if (otterRect == null)
        {
            otterRect = GetComponent<RectTransform>();
        }
        if (otterImage == null)
        {
            otterImage = GetComponent<Image>();
        }

        targetY = otterRect.anchoredPosition.y;
        hiddenY = targetY + (hideOffset * Mathf.Cos(Mathf.PI));

        currentY = hiddenY;
        otterRect.anchoredPosition = new Vector2(otterRect.anchoredPosition.x, hiddenY);
    }

    void Update()
    {
        float target = isUp ? targetY : hiddenY;
        currentY = Mathf.Lerp(otterRect.anchoredPosition.y, target, Time.deltaTime * popSpeed);
        otterRect.anchoredPosition = new Vector2(otterRect.anchoredPosition.x, currentY);

        if (isUp && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
            if (RectTransformUtility.RectangleContainsScreenPoint(otterRect, mouseScreenPosition, null))
            {
                OnOtterClicked();
            }
        }
    }

    public void PopUp(bool setEvil)
    {
        if (isUp) return;
        isUp = true;
        isEvil = setEvil;

        if (otterImage != null)
        {
            otterImage.sprite = setEvil ? evilSprite : cuteSprite;
        }

        StartCoroutine(HideDelay());
    }

    IEnumerator HideDelay()
    {
        yield return new WaitForSeconds(visibleDuration);
        isUp = false;
    }

    void OnOtterClicked()
    {
        isUp = false;
        StopAllCoroutines();
        
        if (WhackGameManager.Instance != null)
        {
            WhackGameManager.Instance.OnOtterWhacked(isEvil);
        }
    }
}