using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class BarStateController : MonoBehaviour
{
    public AudioMixer myAudioMixer;
    public string parameterName;
    public Image uiImage;
    public Sprite[] barStates;
    public int currentIndex = 0;

    void Start()
    {
        // Set to initial state on start
        UpdateVisuals();
    }

    public void IncreaseState()
    {
        if (currentIndex < barStates.Length - 1)
        {
            currentIndex++;
            UpdateVisuals();
            UpdateAudio();
        }
    }

    public void DecreaseState()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateVisuals();
            UpdateAudio();
        }
    }

    private void UpdateVisuals()
    {
        if (uiImage != null && currentIndex >= 0 && currentIndex < barStates.Length)
        {
            uiImage.sprite = barStates[currentIndex];
        }
    }

   private void UpdateAudio()
{
    if (myAudioMixer != null)
    {
        // Using Mathf.Log10 creates a much more natural volume curve
        // index 0 = -80dB, index 5 = 0dB
        float volume = (currentIndex == 0) ? -80f : Mathf.Log10(currentIndex / 5f) * 20f;
        myAudioMixer.SetFloat(parameterName, volume);
    }
}
}