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
    public PlayerMovement playerMovement;

    public static string finalCodeCombination = "1 2 3 4";
    public static bool isSolved = false;
    private int currentSoupState = 0;
    private float lastClickTime = 0f;
    private float clickCooldown = 0.3f;

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

            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                OnSoupClicked();
            }
        }
    }

    public void OpenSoupPuzzle()
    {
        if (soupCanvas != null && soupCanvas.activeSelf) return;

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        if (isSolved)
        {
            if (soupCanvas != null)
            {
                soupCanvas.SetActive(true);
            }

            if (soupSprites != null && soupSprites.Length > 3 && soupFillImage != null)
            {
                soupFillImage.sprite = soupSprites[3];
                float newScale = 1.0f - (3 * 0.25f);
                soupFillImage.transform.localScale = new Vector3(newScale, newScale, 1f);
            }

            if (combinationText != null)
            {
                combinationText.text = finalCodeCombination;
                combinationText.gameObject.SetActive(true);
            }
            return;
        }

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
            if (playerMovement != null)
            {
                playerMovement.enabled = true;
            }
            return;
        }

        if (soupCanvas != null)
        {
            soupCanvas.SetActive(true);
        }

        if (soupRectTransform != null)
        {
            soupRectTransform.anchoredPosition = new Vector2(-15.6579f, 11.3101f);
            soupRectTransform.sizeDelta = new Vector2(815.3259f, 759.473f);
            soupRectTransform.localRotation = Quaternion.Euler(0, 0, -1.967f);
            soupRectTransform.localScale = Vector3.one;
        }

        currentSoupState = 0;

        if (soupSprites != null && soupSprites.Length > 0 && soupFillImage != null)
        {
            soupFillImage.sprite = soupSprites[0];
            soupFillImage.transform.localScale = Vector3.one;
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
        if (Time.time - lastClickTime < clickCooldown) return;
        lastClickTime = Time.time;

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

        InventoryManager inventoryManager = FindFirstObjectByType<InventoryManager>();
        if (inventoryManager != null && ratItemData != null)
        {
            inventoryManager.RemoveItem(ratItemData);
            
            InventoryUI invUI = FindFirstObjectByType<InventoryUI>();
            if (invUI != null) invUI.UpdateUI();
        }

        if (sfxSource != null && sfxSolve != null)
        {
            sfxSource.PlayOneShot(sfxSolve);
        }

        if (combinationText != null)
        {
            combinationText.text = finalCodeCombination;
            combinationText.gameObject.SetActive(true);
        }

        StartCoroutine(AutoCloseAfterSolve());
    }

    IEnumerator AutoCloseAfterSolve()
    {
        yield return new WaitForSeconds(2.0f);
        CloseSoupPuzzle();
    }

    public void CloseSoupPuzzle()
    {
        if (soupCanvas != null)
        {
            soupCanvas.SetActive(false);
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
    }
}