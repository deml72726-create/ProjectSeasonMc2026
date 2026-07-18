using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class UICardGameManager : MonoBehaviour
{
    public static UICardGameManager Instance;
    public ItemData cardTicketItem; 
    public GameObject toyRewardPrefab;
    public Transform rewardSpawnPoint;
    public GameObject missingCardButton;
    public GameObject cardGameCanvas;
    public CanvasGroup anxietyVignette;
    public int currentRound = 1;

    [Header("UI Dialogue System")]
    public CanvasGroup targetDialogueGroup;
    public TMP_Text targetDialogueText;
    public string rulesDialogue = "Match the pairs. Do not make a single mistake, or we start over.";
    public string mistakeDialogue = "Are you sure you are doing it the right way?";

    [Header("Victory Reward")]
    public Sprite coinRewardSprite;
    public ItemData coinItemData;

    public UICard[] allCards;
    public Transform shuffleCenterPoint;
    public float dealSpeed = 8.0f;
    private Vector3[] targetPositions;
    private bool hasUnlockedGame = false;
    public AudioSource sfxSource;
    public AudioSource ambientSource;
    public AudioSource starFaceSource;
    
    public AudioClip sfxShuffle;
    public AudioClip sfxGroupBack;
    public AudioClip sfxPutDown;
    public AudioClip sfxSelect;
    public AudioClip sfxWrong;
    public AudioClip sfxRight;
    public AudioClip sfxStarFace;
    public AudioClip sfxFlipBackGroup;
    public AudioClip ambientGothic;

    public bool isPlayingCardGame = false;

    private UICard firstCard;
    private UICard secondCard;
    private bool canFlip = false;
    private int matchesFound = 0;
    private int totalPairsNeeded;
    private Coroutine fadeCoroutine;
    private Coroutine activeDialogueCoroutine;
    private Coroutine typeCoroutine;
    private int correctStreak = 0;
    private float wrongPlayProbability = 0.7f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        if (sfxSource != null)
        {
            sfxSource.playOnAwake = false;
            sfxSource.Stop();
        }
        if (ambientSource != null)
        {
            ambientSource.playOnAwake = false;
            ambientSource.Stop();
        }
        if (starFaceSource != null)
        {
            starFaceSource.playOnAwake = false;
            starFaceSource.Stop();
        }
    }

    void Start()
    {
        if (allCards == null || allCards.Length == 0)
        {
            Debug.LogError("All Cards array is empty in the Inspector");
            return;
        }

        if (shuffleCenterPoint == null)
        {
            Debug.LogError("Shuffle Center Point is not assigned in the Inspector");
            return;
        }

        if (missingCardButton != null)
        {
            missingCardButton.SetActive(false);
        }

        if (targetDialogueGroup != null)
        {
            targetDialogueGroup.alpha = 0.0f;
            targetDialogueGroup.blocksRaycasts = false;
            targetDialogueGroup.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (cardGameCanvas != null && cardGameCanvas.activeSelf)
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                ResetAndCloseCardGame();
            }

            if (Keyboard.current != null && Keyboard.current.f12Key.wasPressedThisFrame)
            {
                WinGame();
            }
        }
    }

    void SaveCardTargetPositions()
    {
        targetPositions = new Vector3[allCards.Length];
        for (int i = 0; i < allCards.Length; i++)
        {
            if (allCards[i] != null)
            {
                targetPositions[i] = allCards[i].GetComponent<RectTransform>().localPosition;
            }
        }
    }

    void ShuffleTargetPositions()
    {
        for (int i = targetPositions.Length - 1; i > 0; i--)
        {
            int r = Random.Range(0, i + 1);
            Vector3 temp = targetPositions[i];
            targetPositions[i] = targetPositions[r];
            targetPositions[r] = temp;
        }
    }

    public void StartRound()
    {
        if (targetPositions == null)
        {
            SaveCardTargetPositions();
        }

        matchesFound = 0;
        firstCard = null;
        secondCard = null;

        int activeCardsCount = 0;
        if (currentRound == 1)
        {
            activeCardsCount = 4;
            TriggerDialogue(rulesDialogue);
        }
        else if (currentRound == 2)
        {
            activeCardsCount = 8;
        }
        else if (currentRound == 3)
        {
            activeCardsCount = 10;
        }
        else if (currentRound == 4)
        {
            activeCardsCount = 10;
        }

        totalPairsNeeded = activeCardsCount / 2;

        UpdateAnxietyVignette();
        ShuffleTargetPositions();

        System.Collections.Generic.List<int> availableIndices = new System.Collections.Generic.List<int>();
        for (int i = 0; i < activeCardsCount; i++)
        {
            availableIndices.Add(i);
        }

        System.Collections.Generic.List<int> scribbledIndices = new System.Collections.Generic.List<int>();
        if (currentRound == 4)
        {
            for (int k = 0; k < 3; k++)
            {
                int rIndex = Random.Range(0, availableIndices.Count);
                scribbledIndices.Add(availableIndices[rIndex]);
                availableIndices.RemoveAt(rIndex);
            }
        }

        for (int i = 0; i < allCards.Length; i++)
        {
            if (allCards[i] != null)
            {
                if (i < activeCardsCount)
                {
                    allCards[i].gameObject.SetActive(true);
                    allCards[i].SetAlpha(1.0f);
                    Button btn = allCards[i].GetComponent<Button>();
                    if (btn != null)
                    {
                        btn.enabled = true;
                    }

                    allCards[i].isScribbled = scribbledIndices.Contains(i);
                    allCards[i].SetFlipped(false);
                }
                else
                {
                    allCards[i].gameObject.SetActive(false);
                }
            }
        }

        StartCoroutine(RoundStartSequence(activeCardsCount));
    }

    public IEnumerator RoundStartSequence(int activeCount)
    {
        canFlip = false;

        for (int i = 0; i < activeCount; i++)
        {
            if (allCards[i] != null && shuffleCenterPoint != null)
            {
                allCards[i].GetComponent<RectTransform>().localPosition = shuffleCenterPoint.localPosition;
                allCards[i].GetComponent<RectTransform>().localRotation = Quaternion.identity;
                allCards[i].SetFlipped(false);
            }
        }

        if (sfxSource != null && sfxShuffle != null)
        {
            sfxSource.PlayOneShot(sfxShuffle);
        }

        yield return new WaitForSeconds(0.6f);

        for (int i = 0; i < activeCount; i++)
        {
            if (allCards[i] != null)
            {
                Vector3 randomOffset = new Vector3(Random.Range(-15.0f, 15.0f), Random.Range(-15.0f, 15.0f), 0);
                float randomRotation = Random.Range(-15.0f, 15.0f);

                StartCoroutine(MoveCardToPosition(allCards[i].GetComponent<RectTransform>(), targetPositions[i] + randomOffset, randomRotation));
                
                if (sfxSource != null && sfxPutDown != null)
                {
                    sfxSource.PlayOneShot(sfxPutDown);
                }

                yield return new WaitForSeconds(0.12f);
            }
        }

        yield return new WaitForSeconds(0.6f);

        for (int i = 0; i < activeCount; i++)
        {
            if (allCards[i] != null)
            {
                allCards[i].SetFlipped(true);
            }
        }

        yield return new WaitForSeconds(2.0f);

        for (int i = 0; i < activeCount; i++)
        {
            if (allCards[i] != null && allCards[i].GetFlipped())
            {
                allCards[i].SetFlipped(false);

                if (sfxSource != null && sfxFlipBackGroup != null)
                {
                    sfxSource.PlayOneShot(sfxFlipBackGroup);
                }

                yield return new WaitForSeconds(0.12f);
            }
        }

        if (currentRound == 4)
        {
            yield return StartCoroutine(ShellGameSequence(activeCount));
        }

        canFlip = true;
    }

    IEnumerator ShellGameSequence(int activeCount)
    {
        int totalSwaps = 4;
        for (int s = 0; s < totalSwaps; s++)
        {
            int idx1 = Random.Range(0, activeCount);
            int idx2 = Random.Range(0, activeCount);
            while (idx1 == idx2)
            {
                idx2 = Random.Range(0, activeCount);
            }

            yield return StartCoroutine(SwapTwoCards(allCards[idx1], allCards[idx2]));
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator SwapTwoCards(UICard card1, UICard card2)
    {
        RectTransform rect1 = card1.GetComponent<RectTransform>();
        RectTransform rect2 = card2.GetComponent<RectTransform>();
        Vector3 pos1 = rect1.localPosition;
        Vector3 pos2 = rect2.localPosition;

        float time = 0;
        float duration = 0.4f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float progress = time / duration;
            rect1.localPosition = Vector3.Lerp(pos1, pos2, progress);
            rect2.localPosition = Vector3.Lerp(pos2, pos1, progress);
            yield return null;
        }

        rect1.localPosition = pos2;
        rect2.localPosition = pos1;

        int index1 = System.Array.IndexOf(allCards, card1);
        int index2 = System.Array.IndexOf(allCards, card2);
        Vector3 tempPos = targetPositions[index1];
        targetPositions[index1] = targetPositions[blockIndex(index2)];
        targetPositions[index2] = tempPos;
    }

    int blockIndex(int idx)
    {
        return idx;
    }

    IEnumerator MoveCardToPosition(RectTransform cardTransform, Vector3 destination, float rotationZ)
    {
        Quaternion targetRotation = Quaternion.Euler(0, 0, rotationZ);

        while (Vector3.Distance(cardTransform.localPosition, destination) > 1.0f)
        {
            cardTransform.localPosition = Vector3.Lerp(cardTransform.localPosition, destination, Time.deltaTime * dealSpeed);
            cardTransform.localRotation = Quaternion.Lerp(cardTransform.localRotation, targetRotation, Time.deltaTime * dealSpeed);
            yield return null;
        }

        cardTransform.localPosition = destination;
        cardTransform.localRotation = targetRotation;
    }
    public void OpenCardGame()
    {
    InventoryManager inv = FindFirstObjectByType<InventoryManager>();
    bool hasTicket = inv != null && inv.inventory.Contains(cardTicketItem);

    if (!hasUnlockedGame)
    {
        if (hasTicket)
        {
            inv.RemoveItem(cardTicketItem);
            
            InventoryUI invUI = FindObjectOfType<InventoryUI>();
            if (invUI != null) invUI.UpdateUI();

            // SAFELY find the hand slot
            // Instead of Find("HandSlot"), look for the player or a known reference
            GameObject handObj = GameObject.Find("HandSlot"); 
            
            if (handObj != null)
            {
                foreach (Transform child in handObj.transform)
                {
                    ItemPickup pickup = child.GetComponent<ItemPickup>();
                    if (pickup != null && pickup.itemData == cardTicketItem)
                    {
                        Destroy(child.gameObject);
                        break; // Stop after destroying the correct item
                    }
                }
            }
            else
            {
                Debug.LogError("Could not find 'HandSlot'! Check the name in Hierarchy.");
            }
            
            hasUnlockedGame = true;
        }
        else
        {
            Debug.Log("Need a ticket!");
            return;
        }
    }

    isPlayingCardGame = true;
    if (cardGameCanvas != null) cardGameCanvas.SetActive(true);
    
    if (ambientSource != null && ambientGothic != null)
    {
        ambientSource.clip = ambientGothic;
        ambientSource.loop = true;
        ambientSource.Play();
    }
    
    currentRound = 1;
    StartRound();
    }
    public void CloseCardGame()
    {
        isPlayingCardGame = false;

        if (cardGameCanvas != null)
        {
            cardGameCanvas.SetActive(false);
        }

        if (ambientSource != null)
        {
            ambientSource.Stop();
        }

        if (starFaceSource != null)
        {
            starFaceSource.Stop();
        }

        if (targetDialogueGroup != null)
        {
            targetDialogueGroup.alpha = 0.0f;
            targetDialogueGroup.blocksRaycasts = false;
            targetDialogueGroup.gameObject.SetActive(false);
        }

        // Reset variables so the game can be re-entered
        StopAllCoroutines();
        firstCard = null;
        secondCard = null;
        canFlip = false;
        matchesFound = 0;
        correctStreak = 0;

        ShinyInteractable tableScript = FindFirstObjectByType<ShinyInteractable>();
        if (tableScript != null)
        {
            tableScript.enabled = true;
        }
        
        // Ensure cursor is returned to game state
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ResetAndCloseCardGame()
    {
        StopAllCoroutines();

        if (starFaceSource != null)
        {
            starFaceSource.Stop();
        }

        firstCard = null;
        secondCard = null;
        canFlip = true;
        matchesFound = 0;
        correctStreak = 0;

        CloseCardGame();
    }

    public void OnCardClicked(UICard clickedCard)
    {
        if (!canFlip || clickedCard.GetFlipped()) return;
        StartCoroutine(FlipCardSequence(clickedCard));
    }

    IEnumerator FlipCardSequence(UICard card)
    {
        if (currentRound == 4 && !card.isScribbled)
        {
            if (Random.value < 0.4f)
            {
                card.isScribbled = true;
            }
        }

        card.SetFlipped(true);

        if (sfxSource != null && sfxSelect != null)
        {
            sfxSource.PlayOneShot(sfxSelect);
        }

        if (card.isScribbled && starFaceSource != null && sfxStarFace != null)
        {
            starFaceSource.pitch = Random.Range(0.8f, 1.2f);
            starFaceSource.clip = sfxStarFace;
            starFaceSource.loop = true;
            starFaceSource.Play();
        }

        if (firstCard == null)
        {
            firstCard = card;
        }
        else
        {
            secondCard = card;
            canFlip = false;

            yield return new WaitForSeconds(1.0f);

            if (firstCard.cardID == secondCard.cardID)
            {
                Button firstButton = firstCard.GetComponent<Button>();
                Button secondButton = secondCard.GetComponent<Button>();

                if (firstButton != null)
                {
                    firstButton.enabled = false;
                }
                if (secondButton != null)
                {
                    secondButton.enabled = false;
                }

                firstCard.isScribbled = false;
                secondCard.isScribbled = false;
                firstCard.SetFlipped(true);
                secondCard.SetFlipped(true);

                if (starFaceSource != null && starFaceSource.isPlaying)
                {
                    starFaceSource.Stop();
                }

                if (sfxSource != null && sfxRight != null)
                {
                    float volumeFactor = Mathf.Max(0.3f, Mathf.Pow(0.85f, correctStreak));
                    sfxSource.PlayOneShot(sfxRight, volumeFactor);
                }

                correctStreak++;
                matchesFound++;

                if (matchesFound >= totalPairsNeeded)
                {
                    StartCoroutine(RoundTransitionSequence());
                }
            }
            else
            {
                if (starFaceSource != null && starFaceSource.isPlaying)
                {
                    starFaceSource.Stop();
                }

                correctStreak = 0;

                if (Random.value < wrongPlayProbability && sfxSource != null && sfxWrong != null)
                {
                    sfxSource.PlayOneShot(sfxWrong);
                }

                TriggerDialogue(mistakeDialogue);
                StartCoroutine(RestartRoundSequence());
            }

            firstCard = null;
            secondCard = null;
            canFlip = true;
        }
    }

    IEnumerator RestartRoundSequence()
    {
        canFlip = false;
        yield return new WaitForSeconds(0.6f);

        for (int i = 0; i < allCards.Length; i++)
        {
            if (allCards[i] != null && allCards[i].gameObject.activeSelf && allCards[i].GetFlipped())
            {
                allCards[i].SetFlipped(false);

                if (sfxSource != null && sfxFlipBackGroup != null)
                {
                    sfxSource.PlayOneShot(sfxFlipBackGroup);
                }

                yield return new WaitForSeconds(0.12f);
            }
        }

        yield return new WaitForSeconds(0.5f);

        if (sfxSource != null && sfxGroupBack != null)
        {
            sfxSource.PlayOneShot(sfxGroupBack);
        }

        float time = 0;
        float duration = 0.8f;
        Vector3 centerPos = shuffleCenterPoint.localPosition;

        while (time < duration)
        {
            time += Time.deltaTime;
            float progress = time / duration;

            for (int i = 0; i < allCards.Length; i++)
            {
                if (allCards[i] != null && allCards[i].gameObject.activeSelf)
                {
                    RectTransform rect = allCards[i].GetComponent<RectTransform>();
                    rect.localPosition = Vector3.Lerp(rect.localPosition, centerPos, progress);
                    rect.localRotation = Quaternion.Lerp(rect.localRotation, Quaternion.identity, progress);
                }
            }
            yield return null;
        }

        yield return new WaitForSeconds(0.3f);
        StartRound();
    }

    IEnumerator RoundTransitionSequence()
    {
        canFlip = false;
        yield return new WaitForSeconds(0.6f);

        if (starFaceSource != null && starFaceSource.isPlaying)
        {
            starFaceSource.Stop();
        }

        for (int i = 0; i < allCards.Length; i++)
        {
            if (allCards[i] != null && allCards[i].gameObject.activeSelf && allCards[i].GetFlipped())
            {
                allCards[i].SetFlipped(false);

                if (sfxSource != null && sfxFlipBackGroup != null)
                {
                    sfxSource.PlayOneShot(sfxFlipBackGroup);
                }

                yield return new WaitForSeconds(0.12f);
            }
        }

        yield return new WaitForSeconds(0.5f);

        if (sfxSource != null && sfxGroupBack != null)
        {
            sfxSource.PlayOneShot(sfxGroupBack);
        }

        float time = 0;
        float duration = 0.8f;
        Vector3 centerPos = shuffleCenterPoint.localPosition;

        while (time < duration)
        {
            time += Time.deltaTime;
            float progress = time / duration;

            for (int i = 0; i < allCards.Length; i++)
            {
                if (allCards[i] != null && allCards[i].gameObject.activeSelf)
                {
                    RectTransform rect = allCards[i].GetComponent<RectTransform>();
                    rect.localPosition = Vector3.Lerp(rect.localPosition, centerPos, progress);
                    rect.localRotation = Quaternion.Lerp(rect.localRotation, Quaternion.identity, progress);
                }
            }
            yield return null;
        }

        yield return new WaitForSeconds(0.3f);

        currentRound++;
        if (currentRound <= 4)
        {
            StartRound();
        }
        else
        {
            WinGame();
        }
    }

    public void UpdateAnxietyVignette()
    {
        if (anxietyVignette != null)
        {
            float targetAlpha = (float)currentRound / 4.0f;
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(FadeVignetteSequence(targetAlpha));
        }
    }

    IEnumerator FadeVignetteSequence(float targetAlpha)
    {
        float startAlpha = anxietyVignette.alpha;
        float time = 0;
        float duration = 1.5f;

        while (time < duration)
        {
            time += Time.deltaTime;
            anxietyVignette.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }
        anxietyVignette.alpha = targetAlpha;
    }

    public void TriggerDialogue(string message)
    {
        if (targetDialogueGroup != null && targetDialogueText != null)
        {
            targetDialogueGroup.gameObject.SetActive(true);
            targetDialogueGroup.alpha = 1.0f;
            targetDialogueGroup.blocksRaycasts = true;

            if (typeCoroutine != null)
            {
                StopCoroutine(typeCoroutine);
            }
            typeCoroutine = StartCoroutine(TypeDialogueText(message));

            if (activeDialogueCoroutine != null)
            {
                StopCoroutine(activeDialogueCoroutine);
            }
            activeDialogueCoroutine = StartCoroutine(AutoCloseDialogue());
        }
    }

    IEnumerator TypeDialogueText(string message)
    {
        targetDialogueText.text = "";
        float typeSpeed = 0.05f;

        foreach (char c in message.ToCharArray())
        {
            targetDialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    IEnumerator AutoCloseDialogue()
    {
        yield return new WaitForSeconds(3.0f);
        if (targetDialogueGroup != null)
        {
            targetDialogueGroup.alpha = 0.0f;
            targetDialogueGroup.blocksRaycasts = false;
            targetDialogueGroup.gameObject.SetActive(false);
        }
        activeDialogueCoroutine = null;
    }

    void WinGame()
    {
        if (NewItemPopup.Instance != null && coinRewardSprite != null)
        {
            NewItemPopup.Instance.ShowUnlockPopup(coinRewardSprite, "Golden Coin", "A shiny coin rewarded by Mr. Star Face.");
        }

        if (toyRewardPrefab != null && rewardSpawnPoint != null)
        {
            // Spawn the item
            GameObject droppedItem = Instantiate(toyRewardPrefab, rewardSpawnPoint.position, Quaternion.identity);
            
            // IMPORTANT: Ensure the prefab is active and enabled
            droppedItem.SetActive(true);
            
            ItemPickup pickup = droppedItem.GetComponent<ItemPickup>();
            if (pickup != null)
            {
                pickup.itemData = coinItemData;
                
                // Force-enable the collider so you can actually walk into it to pick it up
                Collider2D col = droppedItem.GetComponent<Collider2D>();
                if (col != null) col.enabled = true;
            }
        }

        hasUnlockedGame = false; 
        CloseCardGame();
    }

    public void InsertMissingCard()
    {
        if (missingCardButton != null)
        {
            missingCardButton.SetActive(true);
        }
    }
}