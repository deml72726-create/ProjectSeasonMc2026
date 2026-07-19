using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class radiomanager : MonoBehaviour
{
    public RadioKnob tuningKnob;
    public RadioKnob volumeKnob;
    public RectTransform tuningNeedle;

    public float needleMinX = -240.0f;
    public float needleMaxX = 286.0f;

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
    public float maxKnobTurnRotation = 270.0f;
    public TMP_Text subtitleText;
    public TMP_Text debugText;
    public string targetDecodedMessage = "They are burning up in the sun. Do not leave the house until dark.";

    public ItemData radioKnobItemData;
    public GameObject tuningKnobVisual;

    public GameObject mrStarFaceObject;
    public GameObject objectToEnable;

    public TMP_Text wallClueText;
    public PlayerMovement playerMovement;
    public Image signalGlowLight;

    public static bool isSolved = false;
    public static bool hasKnob = false;
    private static float currentFrequency = 0.5f;
    private static int activeBandIndex = 0;
    private static bool isFirstTimeInit = true;

    private float currentVolume = 0.5f;
    private string allowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    void Start()
    {
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

        if (tuningKnobVisual != null)
        {
            tuningKnobVisual.SetActive(false);
        }

        if (isFirstTimeInit)
        {
            isFirstTimeInit = false;
            targetBandIndex = Random.Range(0, bandButtons.Length);
            targetFrequency = Random.Range(0.15f, 0.85f);
            RandomizeStartingFrequency();
        }
        else
        {
            float savedNeedleX = Mathf.Lerp(needleMinX, needleMaxX, isSolved ? targetFrequency : currentFrequency);
            tuningNeedle.localPosition = new Vector3(savedNeedleX, tuningNeedle.localPosition.y, tuningNeedle.localPosition.z);
        }

        if (wallClueText != null)
        {
            wallClueText.gameObject.SetActive(true);
            
            if (targetBandIndex == 0)
            {
                float amValue = Mathf.Round(530f + (targetFrequency * 1070f));
                wallClueText.text = "BAND: AM\nFREQ: " + amValue + " kHz";
            }
            else
            {
                float fmValue = 88.0f + (targetFrequency * 20.0f);
                wallClueText.text = "BAND: FM\nFREQ: " + fmValue.ToString("F1") + " MHz";
            }
        }
    }

    void Update()
    {
        if (puzzleCanvas != null && puzzleCanvas.activeSelf)
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                ResetAndCloseRadio();
            }
        }
    }

    void RandomizeStartingFrequency()
    {
        float randomStartFreq = Random.value;
        while (Mathf.Abs(randomStartFreq - targetFrequency) < 0.2f)
        {
            randomStartFreq = Random.value;
        }

        currentFrequency = randomStartFreq;
        float startNeedleX = Mathf.Lerp(needleMinX, needleMaxX, currentFrequency);
        tuningNeedle.localPosition = new Vector3(startNeedleX, tuningNeedle.localPosition.y, tuningNeedle.localPosition.z);
        UpdateKnobVisualRotations();
    }

    void SetActiveBand(int index)
    {
        if (isSolved) return;
        activeBandIndex = index;
        UpdateTuningAudioAndText();
    }

    void OnTuningRotated(float rotationAmount)
    {
        if (isSolved || !hasKnob) return;

        float speedFactor = (needleMaxX - needleMinX) / maxKnobTurnRotation;

        float currentNeedleX = tuningNeedle.localPosition.x;
        float newNeedleX = currentNeedleX - (rotationAmount * speedFactor);
        newNeedleX = Mathf.Clamp(newNeedleX, needleMinX, needleMaxX);

        float error = 1.0f;
        if (activeBandIndex == targetBandIndex)
        {
            float freqDiff = Mathf.Abs(currentFrequency - targetFrequency);
            error = Mathf.Clamp01(freqDiff / sweetSpotWidth);
        }

        float jitter = error * currentVolume * Random.Range(-1.6f, 1.6f);
        tuningNeedle.localPosition = new Vector3(newNeedleX + jitter, tuningNeedle.localPosition.y, tuningNeedle.localPosition.z);

        currentFrequency = Mathf.InverseLerp(needleMinX, needleMaxX, newNeedleX);

        UpdateKnobVisualRotations();
        UpdateTuningAudioAndText();
    }

    void OnVolumeRotated(float rotationAmount)
    {
        if (isSolved) return;

        currentVolume = Mathf.Clamp01(currentVolume - (rotationAmount * 0.005f));
        UpdateKnobVisualRotations();
        UpdateTuningAudioAndText();
    }

    void UpdateKnobVisualRotations()
    {
        if (tuningKnob != null)
        {
            float targetKnobZ = (currentFrequency * 180f) - 90f;
            tuningKnob.transform.localRotation = Quaternion.Euler(0, 0, -targetKnobZ);
        }

        if (volumeKnob != null)
        {
            float targetVolumeZ = (currentVolume * 180f) - 90f;
            volumeKnob.transform.localRotation = Quaternion.Euler(0, 0, -targetVolumeZ);
        }
    }

    void UpdateTuningAudioAndText()
    {
        if (isSolved) return;

        float error = 1.0f;

        if (activeBandIndex == targetBandIndex)
        {
            float freqDiff = Mathf.Abs(currentFrequency - targetFrequency);
            error = Mathf.Clamp01(freqDiff / sweetSpotWidth);
        }

        float closeness = 1.0f - error;

        if (staticSource != null)
        {
            staticSource.volume = error * currentVolume;
        }

        if (voiceSource != null)
        {
            voiceSource.volume = closeness * currentVolume;
        }

        UpdateTextDecryption(error);

        if (signalGlowLight != null)
        {
            Color glowColor = signalGlowLight.color;
            glowColor.a = closeness * (0.75f + Mathf.PingPong(Time.time * 2.5f, 0.25f));
            signalGlowLight.color = glowColor;
        }

        if (debugText != null)
        {
            if (activeBandIndex == 0)
            {
                float currentAM = Mathf.Round(530f + (currentFrequency * 1070f));
                float targetAM = Mathf.Round(530f + (targetFrequency * 1070f));
                debugText.text = "Current: " + currentAM + " kHz\nTarget: " + targetAM + " kHz\nBand: AM";
            }
            else
            {
                float currentFM = 88.0f + (currentFrequency * 20.0f);
                float targetFM = 88.0f + (targetFrequency * 20.0f);
                debugText.text = "Current: " + currentFM.ToString("F1") + " MHz\nTarget: " + targetFM.ToString("F1") + " MHz\nBand: FM";
            }
        }

        if (error < 0.02f)
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
        isSolved = true;

        if (mrStarFaceObject != null)
        {
            mrStarFaceObject.SetActive(false);
        }
        if (objectToEnable != null)
        {
            objectToEnable.SetActive(true);
        }

        float solvedNeedleX = Mathf.Lerp(needleMinX, needleMaxX, targetFrequency);
        tuningNeedle.localPosition = new Vector3(solvedNeedleX, tuningNeedle.localPosition.y, tuningNeedle.localPosition.z);

        if (signalGlowLight != null)
        {
            Color glowColor = signalGlowLight.color;
            glowColor.a = 1.0f;
            signalGlowLight.color = glowColor;
        }

        if (staticSource != null)
        {
            staticSource.volume = 0;
            staticSource.Stop();
        }

        if (voiceSource != null)
        {
            voiceSource.volume = 1.0f;
        }

        if (subtitleText != null)
        {
            subtitleText.text = targetDecodedMessage;
        }

        if (sfxSource != null && sfxSolve != null)
        {
            sfxSource.PlayOneShot(sfxSolve);
        }

        UpdateKnobVisualRotations();

        yield return null;
    }

    public void OpenRadio()
    {
        if (!hasKnob)
        {
            InventoryManager inv = FindFirstObjectByType<InventoryManager>();
            bool holdingKnob = false;
            ItemData itemToRemove = null;

            if (inv != null && radioKnobItemData != null)
            {
                foreach (var item in inv.inventory)
                {
                    if (item != null && (item == radioKnobItemData || item.itemName == radioKnobItemData.itemName))
                    {
                        holdingKnob = true;
                        itemToRemove = item;
                        break;
                    }
                }
            }

            if (holdingKnob && itemToRemove != null)
            {
                inv.RemoveItem(itemToRemove);
                
                InventoryUI invUI = FindFirstObjectByType<InventoryUI>();
                if (invUI != null) invUI.UpdateUI();

                GameObject handObj = GameObject.Find("HandSlot");
                if (handObj != null)
                {
                    foreach (Transform child in handObj.transform)
                    {
                        ItemPickup pickup = child.GetComponent<ItemPickup>();
                        if (pickup != null && (pickup.itemData == radioKnobItemData || pickup.itemData.itemName == radioKnobItemData.itemName))
                        {
                            Destroy(child.gameObject);
                            break;
                        }
                    }
                }

                hasKnob = true;
                if (tuningKnobVisual != null)
                {
                    tuningKnobVisual.SetActive(true);
                }
            }
            else
            {
                return;
            }
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        if (puzzleCanvas != null)
        {
            puzzleCanvas.SetActive(true);
        }

        if (staticSource != null && voiceSource != null && !isSolved)
        {
            staticSource.Play();
            voiceSource.Play();
        }
        else if (voiceSource != null && isSolved)
        {
            voiceSource.volume = 1.0f;
            voiceSource.Play();
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (isSolved)
        {
            float solvedNeedleX = Mathf.Lerp(needleMinX, needleMaxX, targetFrequency);
            tuningNeedle.localPosition = new Vector3(solvedNeedleX, tuningNeedle.localPosition.y, tuningNeedle.localPosition.z);
            if (subtitleText != null) subtitleText.text = targetDecodedMessage;
            UpdateKnobVisualRotations();
        }
        else
        {
            UpdateTuningAudioAndText();
        }
    }

    public void CloseRadio()
    {
        if (puzzleCanvas != null)
        {
            puzzleCanvas.SetActive(false);
        }
        if (staticSource != null && voiceSource != null)
        {
            staticSource.Stop();
            voiceSource.Stop();
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ResetAndCloseRadio()
    {
        StopAllCoroutines();
        CloseRadio();
    }
}