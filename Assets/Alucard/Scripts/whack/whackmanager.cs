using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class WhackGameManager : MonoBehaviour
{
    public static WhackGameManager Instance;

    public WhackGame[] allOtters;
    public float minSpawnDelay = 0.6f;
    public float maxSpawnDelay = 1.8f;
    public int evilWhackedNeeded = 5;

    public ItemData coinItemData;
    public ItemData hamsterItemData;
    public Collider2D computerCollider;
    public MonoBehaviour computerInteractScript;

    public GameObject gameCanvas;
    public CanvasGroup anxietyVignette;
    public TMP_Text scoreText;

    public AudioSource sfxSource;
    public AudioClip sfxHitEvil;
    public AudioClip sfxHitCute;

    private int evilWhackedCount = 0;
    private bool isGamePlaying = false;
    private bool hasUnlockedWhack = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (gameCanvas != null) gameCanvas.SetActive(false);
    }

    void Update()
    {
        if (isGamePlaying && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseGame();
        }
    }

    public void StartGame()
    {
        if (!hasUnlockedWhack)
        {
            InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
            if (inventoryManager != null && coinItemData != null)
            {
                if (!inventoryManager.inventory.Contains(coinItemData))
                {
                    Debug.Log("The machine is locked. It needs a coin to play!");
                    return;
                }

                inventoryManager.RemoveItem(coinItemData);
                FindObjectOfType<InventoryUI>().UpdateUI();

                GameObject handObj = GameObject.Find("HandSlot");
                if (handObj != null)
                {
                    foreach (Transform child in handObj.transform)
                    {
                        ItemPickup pickup = child.GetComponent<ItemPickup>();
                        if (pickup != null && pickup.itemData == coinItemData)
                        {
                            Destroy(child.gameObject);
                            break;
                        }
                    }
                }
                hasUnlockedWhack = true;
            }
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        evilWhackedCount = 0;
        UpdateScoreText();
        
        if (gameCanvas != null) gameCanvas.SetActive(true);

        isGamePlaying = true;
        StartCoroutine(SpawnSequence());
    }

    public void CloseGame()
    {
        isGamePlaying = false;
        StopAllCoroutines();

        evilWhackedCount = 0;
        if (anxietyVignette != null) anxietyVignette.alpha = 0f;

        if (gameCanvas != null) gameCanvas.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    IEnumerator SpawnSequence()
    {
        while (isGamePlaying)
        {
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);

            if (allOtters != null && allOtters.Length > 0)
            {
                int rIndex = Random.Range(0, allOtters.Length);
                if (allOtters[rIndex] != null)
                {
                    bool spawnEvil = Random.value < 0.6f;
                    allOtters[rIndex].PopUp(spawnEvil);
                }
            }
        }
    }

    public void OnOtterWhacked(bool isEvil)
    {
        if (!isGamePlaying) return;

        if (isEvil)
        {
            if (sfxSource != null && sfxHitEvil != null) sfxSource.PlayOneShot(sfxHitEvil);

            evilWhackedCount++;
            UpdateScoreText();

            if (evilWhackedCount >= evilWhackedNeeded) WinGame();
        }
        else
        {
            if (sfxSource != null && sfxHitCute != null) sfxSource.PlayOneShot(sfxHitCute);
            evilWhackedCount = 0;
            UpdateScoreText();
            StartCoroutine(ScreenShakeEffect());
            IncreaseAnxietyVignette();
        }
    }

    void UpdateScoreText()
    {
        if (scoreText != null) scoreText.text = "Eliminated: " + evilWhackedCount + " / " + evilWhackedNeeded;
    }

    void IncreaseAnxietyVignette()
    {
        if (anxietyVignette != null) anxietyVignette.alpha = Mathf.Min(0.8f, anxietyVignette.alpha + 0.15f);
    }

    IEnumerator ScreenShakeEffect()
    {
        Vector3 originalPos = gameCanvas.transform.localPosition;
        float time = 0.0f;
        float duration = 0.4f;
        float magnitude = 15.0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float x = Random.Range(magnitude * Mathf.Cos(Mathf.PI), magnitude);
            float y = Random.Range(magnitude * Mathf.Cos(Mathf.PI), magnitude);
            gameCanvas.transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            yield return null;
        }
        gameCanvas.transform.localPosition = originalPos;
    }

    void WinGame()
    {
        isGamePlaying = false;
        StopAllCoroutines();
        hasUnlockedWhack = false;

        if (computerCollider != null) computerCollider.enabled = false;
        if (computerInteractScript != null) computerInteractScript.enabled = false;

        if (NewItemPopup.Instance != null && hamsterItemData != null)
        {
            NewItemPopup.Instance.ShowUnlockPopup(hamsterItemData.icon, hamsterItemData.itemName, "A friendly little hamster that was trapped inside the machine. He is now your companion.");
        }

        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager != null && hamsterItemData != null) inventoryManager.AddItem(hamsterItemData);

        CloseGame();
    }
}