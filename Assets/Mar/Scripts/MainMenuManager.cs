using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;

public class MainManager : MonoBehaviour
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
    public string saveKeyName = "SavedSceneIndex";

    private void Start()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        
        // Start by fading the menu in from black
        if (ScreenFader.Instance != null)
            StartCoroutine(ScreenFader.Instance.FadeIn(fadeSpeed));

        SetupInitialFocus();
    }

    private void Update()
    {
        // Focus handling
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            {
                SetupInitialFocus();
            }
        }

        // Space bar click logic
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
        
        if (PlayerPrefs.HasKey(saveKeyName))
        {
            resumeBtn.interactable = true;
            EventSystem.current.SetSelectedGameObject(resumeButtonGo);
        }
        else
        {
            resumeBtn.interactable = false;
            EventSystem.current.SetSelectedGameObject(newGameButtonGo);
        }
    }

    public void NewGame()
    {
        StartCoroutine(Transition(1)); // Loads scene index 1
    }

    public void ResumeGame()
    {
        int sceneIndex = PlayerPrefs.GetInt(saveKeyName, 1);
        StartCoroutine(Transition(sceneIndex));
    }

    public void QuitGame()
    {
        StartCoroutine(TransitionAndQuit());
    }

    private IEnumerator Transition(int index)
    {
        EventSystem.current.SetSelectedGameObject(null); // Prevent double clicking
        yield return StartCoroutine(ScreenFader.Instance.FadeOut(fadeSpeed));
        SceneManager.LoadScene(index);
    }

    private IEnumerator TransitionAndQuit()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return StartCoroutine(ScreenFader.Instance.FadeOut(fadeSpeed));
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void OpenSettings()
    {
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
        StartCoroutine(SelectButtonDelayed(settingsBackButtonGo));
    }

    public void CloseSettings()
    {
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        StartCoroutine(SelectButtonDelayed(settingsButtonGo));
    }

    private IEnumerator SelectButtonDelayed(GameObject buttonToSelect)
    {
        yield return new WaitForEndOfFrame(); 
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(buttonToSelect);
    }
}