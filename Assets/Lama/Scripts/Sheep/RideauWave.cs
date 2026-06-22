using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(Image))]
public class UIAlphaCurtainWarp : BaseMeshEffect
{
    [Header("Wind Settings")]
    public float waveSpeed = 5f;
    public float waveIntensity = 30f; // Distance in pixels the bottom moves
    public float waveFrequency = 2f;  // How many ripples ripple through it

    [Header("Protection Zone")]
    [Range(0f, 1f)]
    public float rigidTopThreshold = 0.9f; // 0.9 means the top 10% stays perfectly still

    void Update()
    {
        // Force the UI graphic to update its vertices every frame for the animation
        if (graphic != null)
        {
            graphic.SetVerticesDirty();
        }
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive()) return;

        List<UIVertex> vertices = new List<UIVertex>();
        vh.GetUIVertexStream(vertices);

        // Find the absolute top and bottom boundaries of the UI image mesh
        float minY = float.MaxValue;
        float maxY = float.MinValue;
        for (int i = 0; i < vertices.Count; i++)
        {
            float y = vertices[i].position.y;
            if (y < minY) minY = y;
            if (y > maxY) maxY = y;
        }

        float totalHeight = maxY - minY;

        // Loop through every vertex rendering this image
        for (int i = 0; i < vertices.Count; i++)
        {
            UIVertex v = vertices[i];

            // Calculate where this vertex sits vertically from 0 (bottom) to 1 (top)
            float normalizedY = (v.position.y - minY) / totalHeight;

            // If the vertex is in the top 10% (above 0.9), factor is 0 (no movement).
            // If it's at the very bottom (0.0), factor is 1 (maximum movement).
            float waveFactor = 0f;
            if (normalizedY < rigidTopThreshold)
            {
                // This creates a smooth linear fade out of the effect as it goes up
                waveFactor = 1f - (normalizedY / rigidTopThreshold);
                
                // Square it to make the transition from rigid to waving look more natural and smooth
                waveFactor *= waveFactor; 
            }

            // Calculate a unique offset using both Time and the vertex position
            float waveValue = Mathf.Sin((Time.time * waveSpeed) + (normalizedY * waveFrequency));

            // Displace the X position of the vertex
            v.position.x += waveValue * waveIntensity * waveFactor;

            // Apply the modification back
            vertices[i] = v;
        }

        vh.Clear();
        vh.AddUIVertexTriangleStream(vertices);
    }
}