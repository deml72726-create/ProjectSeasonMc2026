using UnityEngine;

public class WhackGame : MonoBehaviour
{
    public RectTransform otterRect; 
    public float popSpeed = 10f;
    private bool isUp = false;
    private float targetY = 0f; // Visible Y
    private float hiddenY = -100f; // Hidden Y

    void Start() { otterRect.anchoredPosition = new Vector2(0, hiddenY); }

    void Update()
    {
        float currentY = Mathf.Lerp(otterRect.anchoredPosition.y, isUp ? targetY : hiddenY, Time.deltaTime * popSpeed);
        otterRect.anchoredPosition = new Vector2(0, currentY);
    }

    public void PopUp() { isUp = true; Invoke("Hide", 1f); }
    private void Hide() { isUp = false; }

    public void OnOtterClicked()
    {
        if (isUp)
        {
            isUp = false;
            ScoreManager.Instance.AddScore(10);
            Debug.Log("Hit!");
        }
    }
}