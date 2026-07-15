using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HammerCursor : MonoBehaviour
{
    public RectTransform hammerRect;
    public float swingAngle = 45.0f;
    public float swingSpeed = 15.0f;
    private bool isSwinging = false;
    private Quaternion originalRotation;

    void Start()
    {
        Cursor.visible = false;
        if (hammerRect == null)
        {
            hammerRect = GetComponent<RectTransform>();
        }
        originalRotation = hammerRect.localRotation;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        hammerRect.position = mousePos;

        if (Mouse.current.leftButton.wasPressedThisFrame && !isSwinging)
        {
            StartCoroutine(SwingSequence());
        }
    }

    IEnumerator SwingSequence()
    {
        isSwinging = true;
        float time = 0.0f;
        float duration = 0.12f;

        Quaternion targetRot = Quaternion.Euler(0, 0, swingAngle * Mathf.Cos(Mathf.PI));

        while (time < duration)
        {
            time += Time.deltaTime;
            hammerRect.localRotation = Quaternion.Lerp(originalRotation, targetRot, time / duration);
            yield return null;
        }

        time = 0.0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            hammerRect.localRotation = Quaternion.Lerp(targetRot, originalRotation, time / duration);
            yield return null;
        }

        hammerRect.localRotation = originalRotation;
        isSwinging = false;
    }

    void OnDestroy()
    {
        Cursor.visible = true;
    }
}