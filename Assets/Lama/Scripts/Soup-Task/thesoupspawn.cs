using System.Collections.Generic;
using UnityEngine;

public class SimpleBoundsSpawner : MonoBehaviour
{
    [Header("What to Spawn")]
    public GameObject prefabToSpawn;
    public int amount = 10;
    public float squareSize = 1f; 

    [Header("Controls")]
    [Tooltip("Press this key in-game to randomize the positions.")]
    public KeyCode respawnKey = KeyCode.R;

    [Header("The Zones")]
    public CircleCollider2D purpleCircle;
    public BoxCollider2D blueBox;

    // We keep track of positions for the math checks
    private List<Vector2> savedPositions = new List<Vector2>();
    
    // We keep track of the actual GameObjects so we can delete them later
    private List<GameObject> spawnedObjects = new List<GameObject>();

    void Start()
    {
        SpawnWithoutOverlaps();
    }

    void Update()
    {
        // Check if the player pressed the respawn key this frame
        if (Input.GetKeyDown(KeyCode.R))
        {
            ClearOldSpawns();
            SpawnWithoutOverlaps();
        }
    }

    void ClearOldSpawns()
    {
        // 1. Destroy every physical square we previously spawned
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        
        // 2. Wipe our memory lists clean so we can start fresh
        spawnedObjects.Clear();
        savedPositions.Clear();
    }

    void SpawnWithoutOverlaps()
    {
        Vector2 center = purpleCircle.bounds.center;
        float safeRadius = purpleCircle.bounds.extents.x - (squareSize / 2f);

        int attempts = 0;
        int maxAttempts = amount * 500;

        while (savedPositions.Count < amount && attempts < maxAttempts)
        {
            attempts++;

            Vector2 randomPos = center + (Random.insideUnitCircle * safeRadius);
            Bounds proposedSquareBounds = new Bounds(randomPos, new Vector3(squareSize, squareSize, 10f));

            if (blueBox.bounds.Intersects(proposedSquareBounds))
            {
                continue; 
            }

            bool hitsAnotherSquare = false;
            foreach (Vector2 existingPos in savedPositions)
            {
                if (Mathf.Abs(randomPos.x - existingPos.x) < squareSize && 
                    Mathf.Abs(randomPos.y - existingPos.y) < squareSize)
                {
                    hitsAnotherSquare = true;
                    break;
                }
            }

            if (hitsAnotherSquare)
            {
                continue; 
            }

            // Spot is mathematically perfect!
            savedPositions.Add(randomPos);
            GameObject newSpawn = Instantiate(prefabToSpawn, randomPos, Quaternion.identity);
            
            // Add the new object to our list so we can destroy it next time we press 'R'
            spawnedObjects.Add(newSpawn); 
        }

        if (savedPositions.Count < amount)
        {
            Debug.LogWarning($"Ran out of space! Only spawned {savedPositions.Count} out of {amount}.");
        }
    }
}