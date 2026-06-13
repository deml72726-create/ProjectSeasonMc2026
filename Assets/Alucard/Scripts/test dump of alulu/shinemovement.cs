using UnityEngine;

public class ShineMovement : MonoBehaviour
{
    public float speed = 3.0f;
    public float startY = 3.0f; 
    public float endY = -3.0f;  

    void Update()
    {
        // Adding Space.World forces the rotated rectangle to move straight down
        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);

        if (transform.localPosition.y < endY)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, startY, transform.localPosition.z);
        }
    }
}