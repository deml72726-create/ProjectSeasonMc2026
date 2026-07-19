using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class XylophoneManager : MonoBehaviour
{
    public static XylophoneManager Instance;
    public List<int> correctMelody = new List<int>();
    private List<int> playerInput = new List<int>();

    public XylophoneKey[] allKeys; 
    public AudioSource sfxSource;
    public AudioClip winSFX;
    public AudioClip loseSFX;

    private bool isProcessing = false;

    void Awake() 
    { 
        Instance = this; 
    }

    public void KeyPressed(int id)
    {
        if (isProcessing) return;

        playerInput.Add(id);
        int i = playerInput.Count - 1;

        if (playerInput[i] != correctMelody[i])
        {
            StartCoroutine(ResetSequence());
            return;
        }

        if (id < allKeys.Length && allKeys[id] != null)
        {
            allKeys[id].SetGreen();
        }

        if (playerInput.Count == correctMelody.Count)
        {
            StartCoroutine(WinSequence());
        }
    }

    IEnumerator ResetSequence()
    {
        isProcessing = true;
        playerInput.Clear();

        if (sfxSource != null && loseSFX != null)
        {
            sfxSource.PlayOneShot(loseSFX);
        }

        foreach (var key in allKeys)
        {
            if (key != null) key.SetRed();
        }

        yield return new WaitForSeconds(0.6f);

        foreach (var key in allKeys)
        {
            if (key != null) key.ResetColor();
        }

        isProcessing = false;
    }

    IEnumerator WinSequence()
    {
        isProcessing = true;

        if (sfxSource != null && winSFX != null)
        {
            sfxSource.PlayOneShot(winSFX);
        }

        yield return new WaitForSeconds(0.8f);

        foreach (var key in allKeys)
        {
            if (key != null) key.ResetColor();
        }

        XylophonePlacer placer = FindFirstObjectByType<XylophonePlacer>();
        if (placer != null)
        {
            placer.DisableInteractionPermanently();
        }

        if (GameManagerBird.Instance != null)
        {
            GameManagerBird.Instance.TriggerVictoryCinematic();
        }

        isProcessing = false;
    }
}