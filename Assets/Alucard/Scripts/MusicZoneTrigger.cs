using UnityEngine;

public class MusicZoneTrigger : MonoBehaviour
{
    public AudioClip roomMusicClip;
    public float fadeDuration = 1.2f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && roomMusicClip != null)
        {
            if (MusicManager.Instance != null)
            {
                MusicManager.Instance.ChangeMusic(roomMusicClip, fadeDuration);
            }
        }
    }
}