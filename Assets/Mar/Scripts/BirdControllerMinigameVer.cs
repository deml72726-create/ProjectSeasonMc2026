using UnityEngine;
using System.Collections.Generic;

public class InteractiveBird : MonoBehaviour
{
    public Sprite beakOpen;
    public Sprite beakClosed;
    public GameObject melodyPrefab; 
    public Transform mouthPoint;
    public List<AudioClip> melodySounds; 
    
    private int currentMelodyIndex = 0;
    private SpriteRenderer sr;
    private AudioSource audioSource;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        audioSource = gameObject.AddComponent<AudioSource>();
        sr.sprite = beakClosed;
    }

    void OnMouseDown()
    {
        // SAFETY CHECK: If manager was destroyed or missing, do nothing (stops the error)
        if (SceneManager.Instance == null) return;

        // CRASH CHECK: If you forgot sounds in the Inspector
        if (melodySounds == null || melodySounds.Count == 0) return;

        if (SceneManager.Instance.isInMinigame)
        {
            PlayMelody();
        }
    }

    void PlayMelody()
    {
        sr.sprite = beakOpen;
        audioSource.PlayOneShot(melodySounds[currentMelodyIndex]);
        
        if (melodyPrefab != null && mouthPoint != null)
            Instantiate(melodyPrefab, mouthPoint.position, Quaternion.identity);
        
        currentMelodyIndex = (currentMelodyIndex + 1) % melodySounds.Count;

        CancelInvoke("CloseBeak");
        Invoke("CloseBeak", 0.2f);
    }

    void CloseBeak() => sr.sprite = beakClosed;
}