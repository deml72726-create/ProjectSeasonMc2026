using UnityEngine;
using UnityEngine.UI;

public class XylophoneKey : MonoBehaviour
{
    public int keyID; // 0, 1, 2...7
    public AudioClip mySound; // The sound this specific key plays

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(PlayNote);
    }

    void PlayNote()
    {
        GetComponent<AudioSource>().PlayOneShot(mySound);
        XylophoneManager.Instance.KeyPressed(keyID);
    }
}