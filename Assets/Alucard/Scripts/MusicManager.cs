using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    private AudioSource audioSource;
    private Coroutine fadeCoroutine;
    private float defaultVolume;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        defaultVolume = audioSource.volume;
    }

    public void ChangeMusic(AudioClip newClip, float fadeDuration)
    {
        if (audioSource.clip == newClip) return;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeMusicRoutine(newClip, fadeDuration));
    }

    IEnumerator FadeMusicRoutine(AudioClip newClip, float duration)
    {
        float elapsed = 0f;
        float startVolume = audioSource.volume;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.clip = newClip;

        if (newClip != null)
        {
            audioSource.Play();
            elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(0f, defaultVolume, elapsed / duration);
                yield return null;
            }
            audioSource.volume = defaultVolume;
        }
        else
        {
            audioSource.Stop();
        }
    }
}