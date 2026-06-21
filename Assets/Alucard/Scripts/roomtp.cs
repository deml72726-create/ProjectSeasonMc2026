using UnityEngine;

public class RoomTeleporter : MonoBehaviour
{
    public Transform destinationSpawnPoint;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = destinationSpawnPoint.position;
        }
    }
}