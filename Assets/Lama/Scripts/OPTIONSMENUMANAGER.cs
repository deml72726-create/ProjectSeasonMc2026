using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class OptionsMenuManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterSlider, musicSlider, sfxSlider;
    
    private const string MASTER_PARAM = "MasterVol";
    private const string MUSIC_PARAM = "MusicVol";
    private const string SFX_PARAM = "SFXVol";

    [Header("Graphics Settings")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    // "Memory" variables to remember what the settings were before the player started messing with them
    private float savedMaster, savedMusic, savedSFX;
    private int savedResIndex;
    private bool savedFullscreen;
    
    private Resolution[] filteredResolutions;

    private void Start()
    {
        // 1. Setup the resolution dropdown list
        SetupResolutionDropdown();

        // 2. Load the actual saved data from the hard drive
        LoadSavedSettings();

        // 3. Make the UI match the saved data
        UpdateUIWithSavedSettings();

        // 4. Hook up sliders to preview audio in real-time (but NOT save yet)
        masterSlider.onValueChanged.AddListener(PreviewMasterVolume);
        musicSlider.onValueChanged.AddListener(PreviewMusicVolume);
        sfxSlider.onValueChanged.AddListener(PreviewSFXVolume);
    }

    // --- SETUP & MEMORY ---

    private void LoadSavedSettings()
    {
        savedMaster = PlayerPrefs.GetFloat(MASTER_PARAM, 0.8f);
        savedMusic = PlayerPrefs.GetFloat(MUSIC_PARAM, 0.8f);
        savedSFX = PlayerPrefs.GetFloat(SFX_PARAM, 0.8f);
        savedResIndex = PlayerPrefs.GetInt("ResIndex", GetCurrentResolutionIndex());
        savedFullscreen = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;

        // Apply these loaded settings to the engine immediately on boot
        SetMixerVolume(MASTER_PARAM, savedMaster);
        SetMixerVolume(MUSIC_PARAM, savedMusic);
        SetMixerVolume(SFX_PARAM, savedSFX);
    }

    private void UpdateUIWithSavedSettings()
    {
        // Temporarily turn off listeners so updating the UI doesn't trigger the preview functions
        masterSlider.SetValueWithoutNotify(savedMaster);
        musicSlider.SetValueWithoutNotify(savedMusic);
        sfxSlider.SetValueWithoutNotify(savedSFX);
        
        resolutionDropdown.SetValueWithoutNotify(savedResIndex);
        resolutionDropdown.RefreshShownValue();
        fullscreenToggle.SetIsOnWithoutNotify(savedFullscreen);
    }

    private void SetupResolutionDropdown()
    {
        Resolution[] allResolutions = Screen.resolutions;
        filteredResolutions = new Resolution[allResolutions.Length]; // Simplified for example
        List<string> options = new List<string>();

        int index = 0;
        foreach (Resolution res in allResolutions)
        {
            filteredResolutions[index] = res;
            options.Add(res.width + " x " + res.height);
            index++;
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);
    }

    private int GetCurrentResolutionIndex()
    {
        for (int i = 0; i < filteredResolutions.Length; i++)
        {
            if (filteredResolutions[i].width == Screen.currentResolution.width &&
                filteredResolutions[i].height == Screen.currentResolution.height)
                return i;
        }
        return 0; // Default to first if not found
    }

    // --- LIVE AUDIO PREVIEWS (Do not save) ---

    private void PreviewMasterVolume(float val) { SetMixerVolume(MASTER_PARAM, val); }
    private void PreviewMusicVolume(float val) { SetMixerVolume(MUSIC_PARAM, val); }
    private void PreviewSFXVolume(float val) { SetMixerVolume(SFX_PARAM, val); }

    private void SetMixerVolume(string param, float val)
    {
        if (val <= 0.0001f) val = 0.0001f;
        audioMixer.SetFloat(param, Mathf.Log10(val) * 20f);
    }

    // --- THE BUTTON FUNCTIONS ---

    // Map this to your "APPLY" Button's OnClick event
    public void ApplySettings()
    {
        // 1. Grab current UI values
        savedMaster = masterSlider.value;
        savedMusic = musicSlider.value;
        savedSFX = sfxSlider.value;
        savedResIndex = resolutionDropdown.value;
        savedFullscreen = fullscreenToggle.isOn;

        // 2. Save everything to PlayerPrefs
        PlayerPrefs.SetFloat(MASTER_PARAM, savedMaster);
        PlayerPrefs.SetFloat(MUSIC_PARAM, savedMusic);
        PlayerPrefs.SetFloat(SFX_PARAM, savedSFX);
        PlayerPrefs.SetInt("ResIndex", savedResIndex);
        PlayerPrefs.SetInt("Fullscreen", savedFullscreen ? 1 : 0);
        PlayerPrefs.Save(); // Force write to disk

        // 3. Apply Video changes to the engine
        Resolution res = filteredResolutions[savedResIndex];
        Screen.SetResolution(res.width, res.height, savedFullscreen);

        // (Audio is already applied via the real-time preview)
        Debug.Log("Settings Applied and Saved!");
    }

    // Map this to your "CANCEL" or "BACK" Button's OnClick event
    public void CancelSettings()
    {
        // 1. Snap UI back to the last saved memory
        UpdateUIWithSavedSettings();

        // 2. Snap the AudioMixer back to the last saved memory (undoing the live previews)
        SetMixerVolume(MASTER_PARAM, savedMaster);
        SetMixerVolume(MUSIC_PARAM, savedMusic);
        SetMixerVolume(SFX_PARAM, savedSFX);

        Debug.Log("Settings Cancelled, reverted to previous state.");
    }
}