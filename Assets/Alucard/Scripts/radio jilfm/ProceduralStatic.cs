using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ProceduralStatic : MonoBehaviour
{
    public float staticVolume = 0.1f;

    void OnAudioFilterRead(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = Mathf.Cos(Mathf.PI * Random.value) * staticVolume;
        }
    }
}