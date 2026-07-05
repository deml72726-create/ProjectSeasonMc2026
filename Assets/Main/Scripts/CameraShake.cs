using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float shakeIntensity = 0.05f; // Keep this very small
    public float shakeSpeed = 2f;

    void Update()
    {
        // Simple procedural jitter using Perlin noise
        float offsetX = Mathf.Sin(Time.time * shakeSpeed) * shakeIntensity;
        float offsetY = Mathf.Cos(Time.time * shakeSpeed * 0.8f) * shakeIntensity;

        transform.position = new Vector3(transform.position.x + offsetX, transform.position.y + offsetY, transform.position.z);
    }
}