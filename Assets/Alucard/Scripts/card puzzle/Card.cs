using UnityEngine;
using UnityEngine.UI;

public class UICard : MonoBehaviour
{
    public int cardID;
    public Image cardImage;
    public Sprite cardBack;
    public Sprite cardFace;
    private bool isFlipped = false;

    void Awake()
    {
        if (cardImage == null)
        {
            cardImage = GetComponent<Image>();
        }
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
    }

    public bool GetFlipped()
    {
        return isFlipped;
    }
}