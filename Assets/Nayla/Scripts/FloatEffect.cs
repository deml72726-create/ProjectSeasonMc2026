using UnityEngine;

public class FloatEffect : MonoBehaviour
{
    public float floatSpeed = 5f;
    public float floatHeight = 0.1f;
    private Vector3 startLocalPos;

    void Start() => startLocalPos = transform.localPosition;

    void Update()
    {
        transform.localPosition = startLocalPos + new Vector3(0, Mathf.Sin(Time.time * floatSpeed) * floatHeight, 0);
    }
}