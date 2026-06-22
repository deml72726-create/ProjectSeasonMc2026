using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UICardGameManager : MonoBehaviour
{
    public GameObject toyRewardPrefab;
    public Transform rewardSpawnPoint;
    public GameObject missingCardButton;
    public GameObject cardGameCanvas;
    private UICard firstCard;
    private UICard secondCard;
    private bool canFlip = true;
    private int matchesFound = 0;
    private int totalPairsNeeded = 6;

    public void OpenCardGame()
    {
        if (cardGameCanvas != null)
        {
            cardGameCanvas.SetActive(true);
        }
    }

    public void CloseCardGame()
    {
        if (cardGameCanvas != null)
        {
            cardGameCanvas.SetActive(false);
        }
    }

    public void OnCardClicked(UICard clickedCard)
    {
        if (!canFlip || clickedCard.GetFlipped()) return;
        StartCoroutine(FlipCardSequence(clickedCard));
    }

    IEnumerator FlipCardSequence(UICard card)
    {
        card.SetFlipped(true);

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
                    firstButton.interactable = false;
                }
                if (secondButton != null)
                {
                    secondButton.interactable = false;
                }

                matchesFound++;

                if (matchesFound >= totalPairsNeeded)
                {
                    WinGame();
                }
            }
            else
            {
                firstCard.SetFlipped(false);
                secondCard.SetFlipped(false);
            }

            firstCard = null;
            secondCard = null;
            canFlip = true;
        }
    }

    void WinGame()
    {
        if (toyRewardPrefab != null && rewardSpawnPoint != null)
        {
            Instantiate(toyRewardPrefab, rewardSpawnPoint.position, Quaternion.identity);
        }
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