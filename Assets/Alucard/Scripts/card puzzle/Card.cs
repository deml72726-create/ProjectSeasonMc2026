using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UICard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int cardID;
    public Image cardImage;
    public Sprite cardBack;
    public Sprite cardFace;
    public Material outlineMaterial;
    public GameObject scribbleOverlay;
    private Material defaultMaterial;
    private bool isFlipped = false;
    public bool isScribbled = false;

    void Awake()
    {
        if (cardImage == null)
        {
            cardImage = GetComponent<Image>();
        }
        defaultMaterial = cardImage.material;
    }

    void Start()
    {
        SetFlipped(false);
    }

    public void SetFlipped(bool flipped)
    {
        isFlipped = flipped;
        if (cardImage != null)
        {
            cardImage.sprite = flipped ? cardFace : cardBack;
        }
        if (scribbleOverlay != null)
        {
            scribbleOverlay.SetActive(flipped && isScribbled);
        }
    }

    public void SetAlpha(float alpha)
    {
        if (cardImage != null)
        {
            cardImage.color = new Color(cardImage.color.r, cardImage.color.g, cardImage.color.b, alpha);
        }
    }

    public bool GetFlipped()
    {
        return isFlipped;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isFlipped && cardImage != null && outlineMaterial != null)
        {
            cardImage.material = outlineMaterial;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (cardImage != null)
        {
            cardImage.material = defaultMaterial;
        }
    }
}