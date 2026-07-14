using UnityEngine;

public class FloatingItem : MonoBehaviour
{
    public float floatSpeed = 3.0f;
    public float floatAmplitude = 15.0f;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float floatOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.localPosition = new Vector3(startPos.x, startPos.y + floatOffset, startPos.z);
    }
}