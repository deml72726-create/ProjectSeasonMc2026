using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class SoupPuzzleManager : MonoBehaviour
{
    public GameObject soupCanvas;
    public Image soupFillImage;
    public Sprite[] soupSprites;
    public TMP_Text combinationText;
    public ItemData hamsterItemData; // Renamed for clarity
    public AudioSource sfxSource;
    public AudioClip sfxSlurp;
    public AudioClip sfxSolve;

    private int currentSoupState = 0;
    private bool isSolved = false;
    private string finalCodeCombination = "1 2 3 4";

    void Start()
    {
        if (soupCanvas != null) soupCanvas.SetActive(false);
        if (soupFillImage != null) soupFillImage.transform.localScale = Vector3.one;
    }

    void Update()
    {
        if (soupCanvas != null && soupCanvas.activeSelf)
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                CloseSoupPuzzle();
            }
        }
    }

    public void OpenSoupPuzzle()
    {
        InventoryManager inventoryManager = FindFirstObjectByType<InventoryManager>();
        bool hasHamster = false;

        if (inventoryManager != null && hamsterItemData != null)
        {
            if (inventoryManager.inventory.Contains(hamsterItemData))
            {
                hasHamster = true;
            }
        }

        if (!hasHamster)
        {
            Debug.Log("I need my hamster companion to eat this soup!");
            return;
        }

        if (soupCanvas != null) soupCanvas.SetActive(true);

        isSolved = false;
        currentSoupState = 0;

        if (soupSprites != null && soupSprites.Length > 0 && soupFillImage != null)
        {
            soupFillImage.sprite = soupSprites[0];
            soupFillImage.transform.localScale = Vector3.one;
        }

        if (combinationText != null) combinationText.gameObject.SetActive(false);

        GenerateRandomCombination();
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void GenerateRandomCombination()
    {
        finalCodeCombination = $"{Random.Range(1, 10)} {Random.Range(0, 10)} {Random.Range(0, 10)} {Random.Range(0, 10)}";
    }

    public void OnSoupClicked()
    {
        if (isSolved || soupSprites == null || soupFillImage == null) return;

        if (currentSoupState < 3)
        {
            currentSoupState++;
            soupFillImage.sprite = soupSprites[currentSoupState];

            float newScale = 1.0f - (currentSoupState * 0.25f);
            soupFillImage.transform.localScale = new Vector3(newScale, newScale, 1f);

            if (sfxSource != null && sfxSlurp != null)
            {
                sfxSource.PlayOneShot(sfxSlurp);
            }

            if (currentSoupState == 3)
            {
                SolvePuzzle();
            }
        }
    }

    void SolvePuzzle()
    {
        isSolved = true;
        if (sfxSource != null && sfxSolve != null)
        {
            sfxSource.PlayOneShot(sfxSolve);
        }
        if (combinationText != null)
        {
            combinationText.text = finalCodeCombination;
            combinationText.gameObject.SetActive(true);
        }
    }

    public void CloseSoupPuzzle()
    {
        if (soupCanvas != null) soupCanvas.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}