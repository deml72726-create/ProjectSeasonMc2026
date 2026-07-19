using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject controlsPanel;
    public GameObject settingsPanel;

    [Header("Transitions")]
    public Animator transitionAnimator;

    public void OpenPanel(GameObject panelToOpen)
    {
        // Debug check to ensure the connection is working
        if (transitionAnimator == null)
        {
            Debug.LogError("MenuManager: transitionAnimator is not assigned in the Inspector!");
            return;
        }

        Debug.Log("Button clicked. Starting transition for: " + (panelToOpen != null ? panelToOpen.name : "None"));
        StartCoroutine(SwitchPanelRoutine(panelToOpen));
    }

private IEnumerator SwitchPanelRoutine(GameObject panelToOpen)
{
    transitionAnimator.gameObject.SetActive(true);
    transitionAnimator.ResetTrigger("isTransition");
    transitionAnimator.SetTrigger("isTransition");

    // Change this value to exactly when your screen is fully black
    // If 0.5f is too fast or slow, try 0.45f or 0.55f
    yield return new WaitForSeconds(0.8f); 

    mainMenuPanel.SetActive(false);
    controlsPanel.SetActive(false);
    settingsPanel.SetActive(false);

    if (panelToOpen != null)
    {
        panelToOpen.SetActive(true);
    }
}
    public void LoadGame(string sceneName)
    {
        StartCoroutine(LoadLevelRoutine(sceneName));
    }

    private IEnumerator LoadLevelRoutine(string sceneName)
    {
        transitionAnimator.SetTrigger("isTransition");
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (controlsPanel.activeSelf || settingsPanel.activeSelf)
            {
                OpenPanel(mainMenuPanel);
            }
        }
    }
}