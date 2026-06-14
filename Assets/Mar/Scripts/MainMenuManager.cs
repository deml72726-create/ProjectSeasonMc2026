using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem; 
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    [Header("Buttons")]
    public GameObject resumeButtonGo;
    public GameObject newGameButtonGo;
    public GameObject settingsButtonGo;
    public GameObject settingsBackButtonGo;

    [Header("Settings")]
    [Range(0.1f, 2.0f)]
    public float fadeSpeed = 0.4f;
    // Your teammate needs to use this exact string in their save script!
    public string saveKeyName = "SavedSceneIndex"; 

    private void Start()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        
        // Explicitly check for save and setup buttons
        SetupInitialFocus();
    }

    private void Update()
    {
        // Re-focus if mouse clicks away and user presses a key
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                SetupInitialFocus();
            }
        }

        // Space bar click
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            GameObject selected = EventSystem.current.currentSelectedGameObject;
            if (selected != null)
            {
                Button btn = selected.GetComponent<Button>();
                if (btn != null && btn.interactable)
                {
                    btn.onClick.Invoke();
                }
            }
        }
    }

    private void SetupInitialFocus()
    {
        Button resumeBtn = resumeButtonGo.GetComponent<Button>();
        
        // CHECK IF SAVE EXISTS
        if (PlayerPrefs.HasKey(saveKeyName))
        {
            resumeBtn.interactable = true;
            EventSystem.current.SetSelectedGameObject(resumeButtonGo);
        }
        else
        {
            // NO SAVE FOUND
            resumeBtn.interactable = false;
            EventSystem.current.SetSelectedGameObject(newGameButtonGo);
        }
    }

    public void NewGame()
    {
        // For testing purposes, you can uncomment the line below to delete saves
        // PlayerPrefs.DeleteAll(); 
        
        StartCoroutine(Transition(1));
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ResumeGame()
    {
        // Get the saved index, default to 1 if something goes wrong
        int sceneIndex = PlayerPrefs.GetInt(saveKeyName, 1);
        StartCoroutine(Transition(sceneIndex));
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void QuitGame()
    {
        StartCoroutine(TransitionAndQuit());
        EventSystem.current.SetSelectedGameObject(null);
    }

    private IEnumerator Transition(int index)
    {
        yield return StartCoroutine(ScreenFader.Instance.FadeOut(fadeSpeed));
        SceneManager.LoadScene(index);
    }

    private IEnumerator TransitionAndQuit()
    {
        yield return StartCoroutine(ScreenFader.Instance.FadeOut(fadeSpeed));
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // --- SETTINGS FIX ---
public void OpenSettings()
    {
        // 1. Clear selection so the EventSystem "forgets" the main menu buttons
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);

        // 2. Wait a frame then select the back button
        StartCoroutine(SelectButtonDelayed(settingsBackButtonGo));
    }

    public void CloseSettings()
    {
        // 1. Clear selection so the EventSystem "forgets" the back button
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        // 2. Wait a frame then select the settings button
        StartCoroutine(SelectButtonDelayed(settingsButtonGo));
    }

    // This helper makes sure the selection happens AFTER the panel is active
    private IEnumerator SelectButtonDelayed(GameObject buttonToSelect)
    {
        yield return new WaitForEndOfFrame(); 
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(buttonToSelect);
        }
    }
 
}