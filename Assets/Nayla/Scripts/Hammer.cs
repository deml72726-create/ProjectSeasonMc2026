using UnityEngine;

public class Hammer : MonoBehaviour
{
    void Update()
    {
        // Get mouse position in screen space
        Vector3 mousePos = Input.mousePosition;
        
        // Move the hammer object to the mouse position
        transform.position = mousePos;
    }
}