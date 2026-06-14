using System.Collections.Generic;
using UnityEngine;
using TMPro; // Required for TextMeshPro components

public class ResolutionSettings : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    private Resolution[] allResolutions;
    private List<Resolution> filteredResolutions;

    private void Start()
    {
        if (resolutionDropdown == null)
        {
            Debug.LogError("Assign the Resolution Dropdown in the Inspector!");
            return;
        }

        // 1. Get all screen resolutions supported by the monitor
        allResolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();

        resolutionDropdown.ClearOptions();
        List<string> optionsList = new List<string>();
        int currentResolutionIndex = 0;

        // 2. Filter out duplicate resolutions caused by different refresh rates (Hz)
        for (int i = 0; i < allResolutions.Length; i++)
        {
            // Optional: You can filter for specific aspect ratios here if desired.
            // For general menus, we check if we already saved this Width x Height
            bool isDuplicate = false;
            for (int j = 0; j < filteredResolutions.Count; j++)
            {
                if (filteredResolutions[j].width == allResolutions[i].width &&
                    filteredResolutions[j].height == allResolutions[i].height)
                {
                    isDuplicate = true;
                    break;
                }
            }

            if (!isDuplicate)
            {
                filteredResolutions.Add(allResolutions[i]);
                
                // Create the text string that the user sees in the menu
                string optionText = allResolutions[i].width + " x " + allResolutions[i].height;
                optionsList.Add(optionText);

                // Check if this option matches the player's current actual screen resolution
                if (allResolutions[i].width == Screen.currentResolution.width &&
                    allResolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = filteredResolutions.Count - 1;
                }
            }
        }

        // 3. Add the clean string list to the TMP Dropdown component
        resolutionDropdown.AddOptions(optionsList);

        // 4. Set the dropdown choice to match the player's current configuration
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // 5. Add a listener to detect when a new resolution option is clicked
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    public void SetResolution(int resolutionIndex)
    {
        // Grab the chosen resolution data structure from our filtered list
        Resolution selectedResolution = filteredResolutions[resolutionIndex];

        // Apply the resolution changes instantly to the Game Engine
        // Screen.fullScreen Mode keeps the game in its current windowed/fullscreen setting
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
    }

    private void OnDestroy()
    {
        if (resolutionDropdown != null)
        {
            resolutionDropdown.onValueChanged.RemoveListener(SetResolution);
        }
    }
    
    public void SetFullscreen(bool isFullscreen)
{
    Screen.fullScreen = isFullscreen;
}
}