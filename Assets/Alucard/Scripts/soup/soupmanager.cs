using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class SoupPuzzleManager : MonoBehaviour
{
    public GameObject soupCanvas;
    public Image soupImage;
    public Sprite[] soupSprites;
    public TMP_Text combinationText;

    public ItemData ratItemData;

    public AudioSource sfxSource;
    public AudioClip sfxSlurp;
    public AudioClip sfxSolve;

    private int currentSoupState = 0;
    private bool isSolved = false;
    private string finalCodeCombination = "1 2 3 4";

    void Start()
    {
        if (soupCanvas != null)
        {
            soupCanvas.SetActive(false);
        }
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
        bool hasRat = false;

        if (inventoryManager != null && ratItemData != null)
        {
            for (int i = 0; i < inventoryManager.inventory.Count; i++)
            {
                if (inventoryManager.inventory[i] == ratItemData)
                {
                    hasRat = true;
                    break;
                }
            }
        }

        if (!hasRat)
        {
            Debug.Log("I cannot eat this cold soup alone. I need my little companion to help me!");
            return;
        }

        if (soupCanvas != null)
        {
            soupCanvas.SetActive(true);
        }

        isSolved = false;
        currentSoupState = 0;

        if (soupSprites != null && soupSprites.Length > 0 && soupImage != null)
        {
            soupImage.sprite = soupSprites[0];
        }

        if (combinationText != null)
        {
            combinationText.gameObject.SetActive(false);
        }

        GenerateRandomCombination();
    }

    void GenerateRandomCombination()
    {
        int partOne = Random.Range(1, 10);
        int partTwo = Random.Range(0, 10);
        int partThree = Random.Range(0, 10);
        int partFour = Random.Range(0, 10);

        finalCodeCombination = partOne.ToString() + " " + partTwo.ToString() + " " + partThree.ToString() + " " + partFour.ToString();
    }

    public void OnSoupClicked()
    {
        if (isSolved || soupSprites == null || soupImage == null) return;

        if (currentSoupState < 3)
        {
            currentSoupState++;
            soupImage.sprite = soupSprites[currentSoupState];

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

        Debug.Log("Soup successfully eaten! Code revealed: " + finalCodeCombination);
    }

    public void CloseSoupPuzzle()
    {
        if (soupCanvas != null)
        {
            soupCanvas.SetActive(false);
        }
    }
}