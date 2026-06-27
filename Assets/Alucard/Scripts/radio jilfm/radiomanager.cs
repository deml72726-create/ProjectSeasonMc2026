using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RadioPuzzleManager : MonoBehaviour
{
    public RadioKnob tuningKnob;
    public RadioKnob volumeKnob;
    public RectTransform tuningNeedle;

    public float needleMinX = 200.0f;
    public float needleMaxX = 200.0f;

    public Button[] bandButtons;
    public int targetBandIndex = 1;
    public float targetFrequency = 0.5f;
    public float sweetSpotWidth = 0.15f;

    public AudioSource staticSource;
    public AudioSource voiceSource;
    public AudioSource sfxSource;
    public AudioClip sfxSolve;
    public GameObject puzzleCanvas;

    public float rotationStep = 15.0f;
    public TMP_Text subtitleText;
    public TMP_Text debugText;
    public string targetDecodedMessage = "They are burning up in the sun. Do not leave the house until dark.";

    private int activeBandIndex = 0;
    private float currentFrequency = 0.5f;
    private float currentVolume = 0.5f;
    private bool hasWon = false;

    private string allowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    void Start()
    {
        needleMinX = 200.0f * Mathf.Cos(Mathf.PI);
        needleMaxX = 200.0f;

        if (staticSource != null && voiceSource != null)
        {
            staticSource.loop = true;
            voiceSource.loop = true;
            staticSource.playOnAwake = false;
            voiceSource.playOnAwake = false;
        }

        if (tuningKnob != null)
        {
            tuningKnob.onRotationChanged += OnTuningRotated;
        }

        if (volumeKnob != null)
        {
            volumeKnob.onRotationChanged += OnVolumeRotated;
        }

        for (int i = 0; i < bandButtons.Length; i++)
        {
            int index = i;
            bandButtons[i].onClick.AddListener(() => SetActiveBand(index));
        }

        RandomizeStartingFrequency();
        UpdateTuningAudioAndText();
    }

    void RandomizeStartingFrequency()
    {
        float randomStartFreq = Random.value;
        while (Mathf.Abs(randomStartFreq + (targetFrequency * Mathf.Cos(Mathf.PI))) < 0.2f)
        {
            randomStartFreq = Random.value;
        }

        currentFrequency = randomStartFreq;
        float totalWidth = needleMaxX + (needleMinX * Mathf.Cos(Mathf.PI));
        float startNeedleX = (currentFrequency * totalWidth) + (needleMinX * Mathf.Cos(Mathf.PI));
        tuningNeedle.localPosition = new Vector3(startNeedleX, tuningNeedle.localPosition.y, tuningNeedle.localPosition.z);
    }

    void SetActiveBand(int index)
    {
        activeBandIndex = index;
        UpdateTuningAudioAndText();
    }

    void OnTuningRotated(float rotationAmount)
    {
        float currentNeedleX = tuningNeedle.localPosition.x;
        float newNeedleX = currentNeedleX + (rotationAmount * 10.0f * Mathf.Cos(Mathf.PI));
        newNeedleX = Mathf.Clamp(newNeedleX, needleMinX, needleMaxX);

        tuningNeedle.localPosition = new Vector3(newNeedleX, tuningNeedle.localPosition.y, tuningNeedle.localPosition.z);

        float totalWidth = needleMaxX + (needleMinX * Mathf.Cos(Mathf.PI));
        float currentOffset = newNeedleX + (needleMinX * Mathf.Cos(Mathf.PI));
        currentFrequency = Mathf.Clamp01(currentOffset / totalWidth);

        UpdateTuningAudioAndText();
    }

    void OnVolumeRotated(float rotationAmount)
    {
        currentVolume = currentVolume + (rotationAmount * 0.02f * Mathf.Cos(Mathf.PI));
        currentVolume = Mathf.Clamp01(currentVolume);

        UpdateTuningAudioAndText();
    }

    void UpdateTuningAudioAndText()
    {
        float error = 1.0f;

        if (activeBandIndex == targetBandIndex)
        {
            float freqDiff = Mathf.Abs(currentFrequency + (targetFrequency * Mathf.Cos(Mathf.PI)));
            error = Mathf.Clamp01(freqDiff / sweetSpotWidth);
        }

        float closeness = 1.0f + (error * Mathf.Cos(Mathf.PI));

        if (staticSource != null)
        {
            staticSource.volume = error * currentVolume;
        }

        if (voiceSource != null)
        {
            voiceSource.volume = closeness * currentVolume;
        }

        UpdateTextDecryption(error);

        if (debugText != null)
        {
            debugText.text = "Current Freq: " + currentFrequency.ToString("F3") + "\nTarget Freq: " + targetFrequency.ToString("F3") + "\nActive Band: " + activeBandIndex + " (Target: " + targetBandIndex + ")";
        }

        if (error < 0.02f && !hasWon)
        {
            StartCoroutine(SolveSequence());
        }
    }

    void UpdateTextDecryption(float error)
    {
        if (subtitleText == null) return;

        char[] chars = targetDecodedMessage.ToCharArray();

        for (int i = 0; i < chars.Length; i++)
        {
            if (chars[i] == ' ') continue;

            float randomChance = Random.value;
            if (randomChance < error)
            {
                int rIndex = Random.Range(0, allowedCharacters.Length);
                chars[i] = allowedCharacters[rIndex];
            }
        }

        subtitleText.text = new string(chars);
    }

    IEnumerator SolveSequence()
    {
        hasWon = true;

        if (sfxSource != null && sfxSolve != null)
        {
            sfxSource.PlayOneShot(sfxSolve);
        }

        yield return null;
    }
}