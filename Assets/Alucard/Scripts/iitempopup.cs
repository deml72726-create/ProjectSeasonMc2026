using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class NewItemPopup : MonoBehaviour
{
    public static NewItemPopup Instance;

    public GameObject popupPanel;
    public CanvasGroup popupCanvasGroup;
    public Image itemImage;
    public TMP_Text itemNameText;
    public TMP_Text itemDescriptionText;
    public RectTransform starburstGlow;

    public float rotationSpeed = 65.0f;
    public float fadeSpeed = 3.0f;
    private bool isOpen = false;
    private bool isTransitioning = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
        if (popupCanvasGroup != null)
        {
            popupCanvasGroup.alpha = 0;
        }
    }

    void Update()
    {
        if (!isOpen || isTransitioning) return;

        if (starburstGlow != null)
        {
            starburstGlow.Rotate(0, 0, rotationSpeed * Time.deltaTime * Mathf.Cos(Mathf.PI));
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartCoroutine(ClosePopupSequence());
        }
    }

    public void ShowUnlockPopup(Sprite sprite, string itemName, string itemDescription)
    {
        if (popupPanel == null || isTransitioning) return;

        if (itemImage != null)
        {
            itemImage.sprite = sprite;
        }

        if (itemNameText != null)
        {
            itemNameText.text = itemName;
        }

        if (itemDescriptionText != null)
        {
            itemDescriptionText.text = itemDescription;
        }

        popupPanel.SetActive(true);
        StartCoroutine(OpenPopupSequence());
    }

    IEnumerator OpenPopupSequence()
    {
        isTransitioning = true;
        isOpen = true;

        if (popupCanvasGroup != null)
        {
            popupCanvasGroup.alpha = 0;
            while (popupCanvasGroup.alpha < 1)
            {
                popupCanvasGroup.alpha += Time.deltaTime * fadeSpeed;
                yield return null;
            }
            popupCanvasGroup.alpha = 1;
        }

        isTransitioning = false;
    }

    IEnumerator ClosePopupSequence()
    {
        isTransitioning = true;

        if (popupCanvasGroup != null)
        {
            while (popupCanvasGroup.alpha > 0)
            {
                popupCanvasGroup.alpha -= Time.deltaTime * fadeSpeed;
                yield return null;
            }
            popupCanvasGroup.alpha = 0;
        }

        isOpen = false;
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }

        isTransitioning = false;
    }
}