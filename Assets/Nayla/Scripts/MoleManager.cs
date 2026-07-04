using UnityEngine;

public class MoleManager : MonoBehaviour
{
    public WhackGame[] allHoles;

    void Start() { InvokeRepeating("Spawn", 1f, 1.5f); }

    void Spawn()
    {
        allHoles[Random.Range(0, allHoles.Length)].PopUp();
    }
}