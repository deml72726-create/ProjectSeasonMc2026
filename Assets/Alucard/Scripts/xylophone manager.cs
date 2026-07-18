using UnityEngine;
using System.Collections.Generic;

public class XylophoneManager : MonoBehaviour
{
    public static XylophoneManager Instance;
    public List<int> correctMelody = new List<int>();
    private List<int> playerInput = new List<int>();

    void Awake() { Instance = this; }

    public void KeyPressed(int id)
    {
        playerInput.Add(id);
        int i = playerInput.Count - 1;

        if (playerInput[i] != correctMelody[i])
        {
            playerInput.Clear();
            return;
        }

        if (playerInput.Count == correctMelody.Count)
        {
            GameManagerBird.Instance.StartCoroutine(GameManagerBird.Instance.CloseBirdTask());
            playerInput.Clear();
        }
    }
}