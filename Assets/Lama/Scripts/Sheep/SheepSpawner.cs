using System.Collections.Generic; // Required to use Lists
using UnityEngine;

public class HerdSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject sheepPrefab;
    public float spawnInterval = 3f;
    public int maxSheepCount = 4; // Set this to 4

    // This list will keep track of every sheep we have spawned
    private List<GameObject> activeSheep = new List<GameObject>();
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnSheep();
            timer = 0f;
        }
    }

    void SpawnSheep()
    {
        // 1. Spawn the new sheep
        GameObject newSheep = Instantiate(sheepPrefab, transform.position, Quaternion.identity);
        
        // 2. Add it to our tracking list
        activeSheep.Add(newSheep);

        // 3. Check if we have too many
        if (activeSheep.Count > maxSheepCount)
        {
            // Get the oldest sheep (index 0)
            GameObject oldestSheep = activeSheep[0];
            
            // Destroy it from the scene
            Destroy(oldestSheep);
            
            // Remove it from our list
            activeSheep.RemoveAt(0);
        }
    }
}