using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class SoupPuzzleManager : MonoBehaviour
{
    public GameObject soupCanvas;
    public Image soupFillImage;
    public RectTransform soupRectTransform;
    public Sprite[] soupSprites;
    public TMP_Text combinationText;
    public ItemData ratItemData;
    public AudioSource sfxSource;
    public AudioClip sfxSlurp;
    public AudioClip sfxSolve;

    private int currentSoupState = 0;
    private bool isSolved = false;
    public static string finalCodeCombination = "1 2 3 4";
    private float lastClickTime = 0f;
    private float clickCooldown = 0.3f;

    void Start()
    {
        if (soupCanvas != null) soupCanvas.SetActive(false);
    }

    void Update()
    {
        if (soupCanvas != null && soupCanvas.activeSelf)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame) CloseSoupPuzzle();
            if (Keyboard.current.eKey.wasPressedThisFrame) OnSoupClicked();
        }
    }

    public void OpenSoupPuzzle()
    {
        if (soupCanvas != null && soupCanvas.activeSelf) return;

        InventoryManager inventoryManager = FindFirstObjectByType<InventoryManager>();
        bool hasRat = false;
        int ratIndex = -1;

        if (inventoryManager != null && ratItemData != null)
        {
            for (int i = 0; i < inventoryManager.inventory.Count; i++)
            {
                if (inventoryManager.inventory[i] == ratItemData)
                {
                    hasRat = true;
                    ratIndex = i; // Save the position of the rat
                    break;
                }
            }
        }

        if (!hasRat) return;

        // REMOVE THE RAT FROM INVENTORY
        if (inventoryManager != null && ratIndex != -1)
        {
            inventoryManager.inventory.RemoveAt(ratIndex);
            // If your inventory manager has a refresh method, call it here:
            // inventoryManager.UpdateUI();
        }

        soupCanvas.SetActive(true);

        if (soupRectTransform != null)
        {
            soupRectTransform.anchoredPosition = new Vector2(-15.6579f, 11.3101f);
            soupRectTransform.sizeDelta = new Vector2(815.3259f, 759.473f);
            soupRectTransform.localRotation = Quaternion.Euler(0, 0, -1.967f);
            soupRectTransform.localScale = Vector3.one;
        }

        if (!isSolved)
        {
            currentSoupState = 0;
            if (soupSprites != null && soupSprites.Length > 0 && soupFillImage != null)
            {
                soupFillImage.sprite = soupSprites[0];
                soupFillImage.transform.localScale = Vector3.one;
            }
            GenerateRandomCombination();
        }

        if (combinationText != null) combinationText.gameObject.SetActive(isSolved);
    }

    void GenerateRandomCombination()
    {
        finalCodeCombination = $"{Random.Range(1, 10)} {Random.Range(0, 10)} {Random.Range(0, 10)} {Random.Range(0, 10)}";
    }

    public void OnSoupClicked()
    {
        if (Time.time - lastClickTime < clickCooldown) return;
        lastClickTime = Time.time;

        if (isSolved || soupSprites == null || soupFillImage == null) return;

        if (currentSoupState < 3)
        {
            currentSoupState++;
            soupFillImage.sprite = soupSprites[currentSoupState];
            float newScale = 1.0f - (currentSoupState * 0.25f);
            soupFillImage.transform.localScale = new Vector3(newScale, newScale, 1f);

            if (sfxSource != null && sfxSlurp != null) sfxSource.PlayOneShot(sfxSlurp);
            if (currentSoupState == 3) SolvePuzzle();
        }
    }

    void SolvePuzzle()
    {
        isSolved = true;
        if (sfxSource != null && sfxSolve != null) sfxSource.PlayOneShot(sfxSolve);
        if (combinationText != null)
        {
            combinationText.text = finalCodeCombination;
            combinationText.gameObject.SetActive(true);
        }
    }

    public void CloseSoupPuzzle()
    {
        if (soupCanvas != null) soupCanvas.SetActive(false);
    }
}