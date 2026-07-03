using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }

    [Header("The Containers")]
    public GameObject mainRoomContainer; // Drag the parent object of your whole room here
    public GameObject minigameContainer; // Drag the parent object of your minigame here

    [Header("Player & Movement")]
    public MonoBehaviour playerMovementScript; 
    
    [Header("Cameras")]
    public GameObject mainCamera;
    public GameObject minigameCamera;

    [Header("UI & Transition")]
    public GameObject minigameUI; 
    public float fadeDuration = 0.6f;
    
    [HideInInspector] public bool isInMinigame = false;

    // Bridge for loading scenes
    public static void LoadScene(string s) => UnityEngine.SceneManagement.SceneManager.LoadScene(s);
    public static void LoadScene(int i) => UnityEngine.SceneManagement.SceneManager.LoadScene(i);

    private void Awake()
    {
        // Simple Singleton (No DontDestroyOnLoad)
        Instance = this;

        // Ensure cameras are at -10 depth
        if(mainCamera != null) mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, -10);
        if(minigameCamera != null) minigameCamera.transform.position = new Vector3(minigameCamera.transform.position.x, minigameCamera.transform.position.y, -10);

        // Standard start state
        if (mainRoomContainer != null) mainRoomContainer.SetActive(true);
        if (minigameContainer != null) minigameContainer.SetActive(false);
        if (mainCamera != null) mainCamera.SetActive(true);
        if (minigameCamera != null) minigameCamera.SetActive(false);
        if (minigameUI != null) minigameUI.SetActive(false);
    }

    public void StartBirdMinigame() { StartCoroutine(Transition(true)); }
    public void OnBackClicked() { StartCoroutine(Transition(false)); }

    private IEnumerator Transition(bool toMinigame)
    {
        // 1. Move Fader and Fade to Black
        BirdFader.Instance.MoveToCamera(toMinigame ? mainCamera : minigameCamera);
        yield return StartCoroutine(BirdFader.Instance.FadeRoutine(1f, fadeDuration));

        // 2. SWITCH EVERYTHING
        if (toMinigame) {
            mainRoomContainer.SetActive(false); // HIDE THE ROOM
            minigameContainer.SetActive(true);  // SHOW THE GAME
            mainCamera.SetActive(false);
            minigameCamera.SetActive(true);
            minigameUI.SetActive(true);
            if (playerMovementScript != null) playerMovementScript.enabled = false;
            isInMinigame = true;
        } else {
            mainRoomContainer.SetActive(true);  // SHOW THE ROOM
            minigameContainer.SetActive(false); // HIDE THE GAME
            mainCamera.SetActive(true);
            minigameCamera.SetActive(false);
            minigameUI.SetActive(false);
            if (playerMovementScript != null) playerMovementScript.enabled = true;
            isInMinigame = false;
        }

        // 3. Move Fader to new camera and Fade to Clear
        BirdFader.Instance.MoveToCamera(toMinigame ? minigameCamera : mainCamera);
        yield return StartCoroutine(BirdFader.Instance.FadeRoutine(0f, fadeDuration));
    }
}