using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeSettings : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("UI Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider; // <-- New SFX Slider added here

    // These MUST match the exact text of your Exposed Parameters in the Audio Mixer
    private const string MASTER_PARAM = "MasterVol";
    private const string MUSIC_PARAM = "MusicVol";
    private const string SFX_PARAM = "SFXVol"; // <-- New SFX Parameter string

    private void Start()
    {
        // 1. Load saved values from the player's computer (default to 80% if no save exists)
        float savedMaster = PlayerPrefs.GetFloat(MASTER_PARAM, 0.8f);
        float savedMusic = PlayerPrefs.GetFloat(MUSIC_PARAM, 0.8f);
        float savedSFX = PlayerPrefs.GetFloat(SFX_PARAM, 0.8f);

        // 2. Set the UI sliders to match the saved numbers visually
        if (masterSlider != null) masterSlider.value = savedMaster;
        if (musicSlider != null) musicSlider.value = savedMusic;
        if (sfxSlider != null) sfxSlider.value = savedSFX;

        // 3. Apply the volumes to the game engine immediately on startup
        SetMasterVolume(savedMaster);
        SetMusicVolume(savedMusic);
        SetSFXVolume(savedSFX);

        // 4. Hook up the sliders so dragging them triggers the volume changes instantly
        if (masterSlider != null) masterSlider.onValueChanged.AddListener(SetMasterVolume);
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(SetMusicVolume);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float sliderValue)
    {
        if (sliderValue <= 0.0001f) sliderValue = 0.0001f;
        float dB = Mathf.Log10(sliderValue) * 20f;
        
        audioMixer.SetFloat(MASTER_PARAM, dB);
        PlayerPrefs.SetFloat(MASTER_PARAM, sliderValue); 
    }

    public void SetMusicVolume(float sliderValue)
    {
        if (sliderValue <= 0.0001f) sliderValue = 0.0001f;
        float dB = Mathf.Log10(sliderValue) * 20f;
        
        audioMixer.SetFloat(MUSIC_PARAM, dB);
        PlayerPrefs.SetFloat(MUSIC_PARAM, sliderValue); 
    }

    public void SetSFXVolume(float sliderValue)
    {
        if (sliderValue <= 0.0001f) sliderValue = 0.0001f;
        float dB = Mathf.Log10(sliderValue) * 20f;
        
        audioMixer.SetFloat(SFX_PARAM, dB);
        PlayerPrefs.SetFloat(SFX_PARAM, sliderValue); 
    }

    private void OnDestroy()
    {
        // Clean up memory when the scene changes or the menu closes
        if (masterSlider != null) masterSlider.onValueChanged.RemoveListener(SetMasterVolume);
        if (musicSlider != null) musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
        if (sfxSlider != null) sfxSlider.onValueChanged.RemoveListener(SetSFXVolume);
    }
}