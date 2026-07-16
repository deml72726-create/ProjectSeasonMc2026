using UnityEngine;

public class CameraSway : MonoBehaviour
{
    public float swayAmount = 0.5f; // How far it moves
    public float swaySpeed = 0.5f;  // How fast it drifts

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Creates a slow, looping drift using Sine waves
        float x = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        float y = Mathf.Cos(Time.time * swaySpeed * 0.5f) * swayAmount;

        transform.position = startPos + new Vector3(x, y, 0);
    }
}