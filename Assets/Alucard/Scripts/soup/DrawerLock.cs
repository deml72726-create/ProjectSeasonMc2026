using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class DrawerLock : MonoBehaviour
{
    public GameObject keypadCanvas;
    
    public GameObject keypadPanel;
    public GameObject drawerOpenPanel;
    
    public TMP_Text displayText;
    public PlayerMovement playerMovement;

    public AudioSource sfxSource;
    public AudioClip clickClip;
    public AudioClip buzzerClip;
    public AudioClip unlockClip;

    public ItemData xylophoneItemData;
    public Button xylophoneButton;

    public GameObject xylophonePlacerObject;

    private string currentInput = "";
    private bool isUnlocked = false;
    private bool isProcessing = false;

    void Start()
    {
        if (xylophoneButton != null)
        {
            xylophoneButton.onClick.AddListener(CollectXylophone);
        }

        if (sfxSource == null)
        {
            sfxSource = GetComponent<AudioSource>();
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
        
        sfxSource.spatialBlend = 0f;
    }

    void Update()
    {
        if (keypadCanvas != null && keypadCanvas.activeSelf && !isProcessing)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseKeypad();
            }
        }
    }

    public void OpenKeypad()
    {
        if (isUnlocked || isProcessing) return;

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        currentInput = "";
        if (displayText != null)
        {
            displayText.color = Color.white;
            displayText.text = "----";
        }
        
        keypadPanel.SetActive(true);
        drawerOpenPanel.SetActive(false);
        
        if (keypadCanvas != null)
        {
            keypadCanvas.SetActive(true);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void PressNumber(int number)
    {
        if (isProcessing || currentInput.Length >= 4) return;

        if (sfxSource != null && clickClip != null)
        {
            sfxSource.PlayOneShot(clickClip);
        }

        currentInput += number.ToString();
        UpdateDisplayVisuals();

        if (currentInput.Length == 4)
        {
            StartCoroutine(ProcessCombination());
        }
    }

    private void UpdateDisplayVisuals()
    {
        if (displayText == null) return;
        
        string displayString = currentInput;
        while (displayString.Length < 4)
        {
            displayString += "-";
        }
        displayText.text = displayString;
    }

    public void ClearInput()
    {
        if (isProcessing) return;

        if (sfxSource != null && clickClip != null)
        {
            sfxSource.PlayOneShot(clickClip);
        }

        currentInput = "";
        UpdateDisplayVisuals();
    }

    private IEnumerator ProcessCombination()
    {
        isProcessing = true;
        yield return new WaitForSeconds(0.4f);

        string correctCombination = SoupPuzzleManager.finalCodeCombination.Replace(" ", "");

        if (currentInput == correctCombination)
        {
            if (displayText != null) displayText.color = Color.green;
            if (sfxSource != null && unlockClip != null) sfxSource.PlayOneShot(unlockClip);
            
            yield return new WaitForSeconds(0.8f);
            ShowOpenDrawerScreen();
        }
        else
        {
            if (displayText != null) displayText.color = Color.red;
            if (sfxSource != null && buzzerClip != null) sfxSource.PlayOneShot(buzzerClip);
            
            yield return new WaitForSeconds(0.8f);
            
            if (displayText != null) displayText.color = Color.white;
            currentInput = "";
            UpdateDisplayVisuals();
            isProcessing = false;
        }
    }

    private void ShowOpenDrawerScreen()
    {
        keypadPanel.SetActive(false);
        drawerOpenPanel.SetActive(true);
        isProcessing = false;
    }

    public void CollectXylophone()
    {
        isUnlocked = true;

        InventoryManager inventoryManager = FindFirstObjectByType<InventoryManager>();
        if (inventoryManager != null && xylophoneItemData != null)
        {
            inventoryManager.AddItem(xylophoneItemData);
        }

        if (xylophonePlacerObject != null)
        {
            xylophonePlacerObject.SetActive(true);
        }

        if (keypadCanvas != null)
        {
            keypadCanvas.SetActive(false);
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }

    public void CloseKeypad()
    {
        if (isProcessing) return;

        if (keypadCanvas != null)
        {
            keypadCanvas.SetActive(false);
        }
        
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}