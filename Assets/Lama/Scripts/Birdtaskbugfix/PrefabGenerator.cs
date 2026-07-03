using System.Collections.Generic;
using UnityEngine;

public class PrefabGenerator : MonoBehaviour
{
    private int currentIndex = 0;

    // Must be public so GameManager can call it
    public void SpawnNextFromCombination(List<GameObject> activeCombination)
    {
        if (activeCombination == null || activeCombination.Count == 0)
        {
            Debug.LogWarning("The combination list is empty!");
            return;
        }

        // Spawn the prefab at the current index at this Generator's position
        GameObject prefabToSpawn = activeCombination[currentIndex];
        Instantiate(prefabToSpawn, transform.position, Quaternion.identity);

        Debug.Log($"Spawned: {prefabToSpawn.name} at index {currentIndex}");

        // Move to the next index, wrapping back to 0
        currentIndex = (currentIndex + 1) % activeCombination.Count;
    }
}