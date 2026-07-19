using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManagerBird : MonoBehaviour
{
    public static GameManagerBird Instance { get; private set; }
    public List<AudioClip> xylophoneSounds;
    public List<int> correctCombo = new List<int>();
    public AudioSource birdAudioSource;
    public Image fadeOverlay;
    
    public Transform birdTargetTransform; 
    public Camera mainCameraComponent;    
    public PlayerMovement playerMovementScript;
    public Animator birdAnimator; 
    public List<GameObject> melodyVfxPrefabs; 

    public XylophoneManager xylophoneManagerReference;

    public GameObject radioKnobPrefab; 
    public Transform nestDropPoint;     
    public float cinematicZoomSize = 5f; 

    public float zoomOrthographicSize = 3f;
    public float fadeDuration = 1.5f;
    public bool CanCloseTab = false;
    public bool OnBird = false;

    private Vector3 originalCameraPosition;
    private float originalCameraSize;
    private Coroutine cameraMoveCoroutine;
    private MonoBehaviour cinemachineBrainComponent;
    public static bool isPermanentlySolved = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        correctCombo.Clear();
        for (int i = 0; i < 4; i++)
        {
            correctCombo.Add(Random.Range(0, xylophoneSounds.Count));
        }
    }

    private void Start()
    {
        StartCoroutine(WaitAndAssignCombo());
        
        if (mainCameraComponent != null)
        {
            originalCameraPosition = mainCameraComponent.transform.position;
            originalCameraSize = mainCameraComponent.orthographicSize;
            cinemachineBrainComponent = mainCameraComponent.GetComponent("CinemachineBrain") as MonoBehaviour;
        }

        if (isPermanentlySolved)
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
        }
    }

    void Update()
    {
        if (isPermanentlySolved) return;

        if (OnBird && CanCloseTab && Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(CloseBirdTask());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isPermanentlySolved) return;

        if (other.CompareTag("Player") && !OnBird)
        {
            StartCoroutine(EnterBirdSequence());
        }
    }

    IEnumerator EnterBirdSequence()
    {
        OnBird = true;
        desactivatemovement(false);
        FadeOut();
        yield return new WaitForSeconds(fadeDuration);

        if (cinemachineBrainComponent != null)
        {
            cinemachineBrainComponent.enabled = false;
        }

        if (mainCameraComponent != null && birdTargetTransform != null)
        {
            originalCameraPosition = mainCameraComponent.transform.position;
            Vector3 targetPos = new Vector3(birdTargetTransform.position.x, birdTargetTransform.position.y, originalCameraPosition.z);
            
            if (cameraMoveCoroutine != null) StopCoroutine(cameraMoveCoroutine);
            cameraMoveCoroutine = StartCoroutine(MoveCameraRoutine(targetPos, zoomOrthographicSize));
        }
        
        FadeIn();
        yield return new WaitForSeconds(fadeDuration);

        PlayBirdMelody();
        CanCloseTab = true;
    }

    public void PlayBirdMelody() 
    {
        StartCoroutine(SingRoutine());
    }

    IEnumerator SingRoutine()
    {
        foreach (int index in correctCombo)
        {
            if (birdAnimator != null)
            {
                birdAnimator.SetTrigger("SingTrigger");
            }

            if (birdAudioSource != null && xylophoneSounds.Count > index)
            {
                birdAudioSource.PlayOneShot(xylophoneSounds[index]);
            }

            if (melodyVfxPrefabs != null && melodyVfxPrefabs.Count > 0 && birdTargetTransform != null)
            {
                int randomVfxIndex = Random.Range(0, melodyVfxPrefabs.Count);
                GameObject randomPrefab = melodyVfxPrefabs[randomVfxIndex];

                if (randomPrefab != null)
                {
                    Vector3 spawnPos = birdTargetTransform.position + new Vector3(0, 1.2f, -1f); 
                    GameObject spawnedVfx = Instantiate(randomPrefab, spawnPos, Quaternion.identity);
                    spawnedVfx.transform.localScale = spawnedVfx.transform.localScale * 0.5f;
                    Destroy(spawnedVfx, 1.5f); 
                }
            }

            yield return new WaitForSeconds(0.8f);
        }

        yield return new WaitForSeconds(2.0f);
        if (OnBird && !isPermanentlySolved)
        {
            StartCoroutine(CloseBirdTask());
        }
    }

    public void TriggerVictoryCinematic()
    {
        StartCoroutine(VictoryCinematicSequence());
    }

    IEnumerator VictoryCinematicSequence()
    {
        isPermanentlySolved = true;
        OnBird = false;
        CanCloseTab = false;

        desactivatemovement(false);
        FadeOut();
        yield return new WaitForSeconds(fadeDuration);

        if (cinemachineBrainComponent != null)
        {
            cinemachineBrainComponent.enabled = false;
        }

        if (mainCameraComponent != null && birdTargetTransform != null)
        {
            Vector3 targetPos = new Vector3(birdTargetTransform.position.x, birdTargetTransform.position.y - 1f, originalCameraPosition.z);
            
            if (cameraMoveCoroutine != null) StopCoroutine(cameraMoveCoroutine);
            cameraMoveCoroutine = StartCoroutine(MoveCameraRoutine(targetPos, cinematicZoomSize));
        }

        FadeIn();
        yield return new WaitForSeconds(fadeDuration);

        if (radioKnobPrefab != null && nestDropPoint != null)
        {
            Instantiate(radioKnobPrefab, nestDropPoint.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(3.0f);

        FadeOut();
        yield return new WaitForSeconds(fadeDuration);

        if (mainCameraComponent != null)
        {
            if (cameraMoveCoroutine != null) StopCoroutine(cameraMoveCoroutine);
            cameraMoveCoroutine = StartCoroutine(MoveCameraRoutine(originalCameraPosition, originalCameraSize));
        }

        FadeIn();
        yield return new WaitForSeconds(fadeDuration);

        if (cinemachineBrainComponent != null)
        {
            cinemachineBrainComponent.enabled = true;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        desactivatemovement(true);
    }

    public IEnumerator CloseBirdTask()
    {
        CanCloseTab = false;
        FadeOut();
        yield return new WaitForSeconds(fadeDuration);
        
        if (mainCameraComponent != null)
        {
            if (cameraMoveCoroutine != null) StopCoroutine(cameraMoveCoroutine);
            cameraMoveCoroutine = StartCoroutine(MoveCameraRoutine(originalCameraPosition, originalCameraSize));
        }
        
        FadeIn();
        yield return new WaitForSeconds(fadeDuration);

        if (cinemachineBrainComponent != null)
        {
            cinemachineBrainComponent.enabled = true;
        }
        
        desactivatemovement(true);
        OnBird = false;
    }

    IEnumerator MoveCameraRoutine(Vector3 targetPos, float targetSize)
    {
        float elapsed = 0f;
        float duration = 1.0f; 
        Vector3 startPos = mainCameraComponent.transform.position;
        float startSize = mainCameraComponent.orthographicSize;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t);

            mainCameraComponent.transform.position = Vector3.Lerp(startPos, targetPos, t);
            mainCameraComponent.orthographicSize = Mathf.Lerp(startSize, targetSize, t);
            yield return null;
        }

        mainCameraComponent.transform.position = targetPos;
        mainCameraComponent.orthographicSize = targetSize;
    }

    IEnumerator WaitAndAssignCombo()
    {
        yield return new WaitForSeconds(0.5f);
        if (xylophoneManagerReference != null)
        {
            xylophoneManagerReference.correctMelody = new List<int>(correctCombo);
        }
    }

    public void FadeOut() => StartCoroutine(FadeRoutine(0f, 1f));
    public void FadeIn() => StartCoroutine(FadeRoutine(1f, 0f));

    private void desactivatemovement(bool state)
    {
        if (playerMovementScript != null) playerMovementScript.enabled = state;
    }

    IEnumerator FadeRoutine(float startAlpha, float targetAlpha)
    {
        float elapsedTime = 0f;
        Color color = fadeOverlay.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            fadeOverlay.color = color;
            yield return null;
        }
        color.a = targetAlpha;
        fadeOverlay.color = color;
    }
}