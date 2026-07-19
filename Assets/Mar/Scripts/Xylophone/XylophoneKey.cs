using UnityEngine;
using UnityEngine.UI;

public class XylophoneKey : MonoBehaviour
{
    public int keyID;
    public AudioClip mySound;
    private AudioSource audioSource;
    private Image image;
    private Color originalColor;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(PlayNote);
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        
        image = GetComponent<Image>();
        if (image != null)
        {
            originalColor = image.color;
        }
    }

    void PlayNote()
    {
        audioSource.PlayOneShot(mySound);
        XylophoneManager.Instance.KeyPressed(keyID);
    }

    public void SetGreen()
    {
        if (image != null)
        {
            image.color = Color.green;
        }
    }

    public void SetRed()
    {
        if (image != null)
        {
            image.color = Color.red;
        }
    }

    public void ResetColor()
    {
        if (image != null)
        {
            image.color = originalColor;
        }
    }
}