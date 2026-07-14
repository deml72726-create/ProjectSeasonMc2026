using UnityEngine;

public class TextJuice : MonoBehaviour
{
    public float pulseSpeed = 2.5f;
    public float pulseAmplitude = 0.05f;
    private Vector3 baseScale;

    void Start()
    {
        baseScale = transform.localScale;
    }

    void Update()
    {
        float scaleFactor = 1.0f + (Mathf.Sin(Time.time * pulseSpeed) * pulseAmplitude);
        transform.localScale = baseScale * scaleFactor;
    }
}