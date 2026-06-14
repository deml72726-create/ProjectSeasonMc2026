using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerTransform;
    public Vector3 offset;
    public float smoothSpeed;
    void FixedUpdate()
    {
        Vector3 desiredPosition = playerTransform.position + offset;
        Vector3 smoothedPostion = Vector3.Lerp(transform.position, desiredPosition,smoothSpeed * Time.fixedDeltaTime);
        transform.position = smoothedPostion;   
    }
}
