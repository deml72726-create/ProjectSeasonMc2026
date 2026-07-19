using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    [System.Serializable]
    public struct CutsceneStep
    {
        public GameObject sceneObject;
        public TextMeshProUGUI textDisplay;
        [TextArea] public string dialogueText;
        public float lingerTime;
        public float fadeDuration;
        public bool triggerHospitalBips;
    }

    public CutsceneStep[] steps;
    public Image fadePanel;
    public AudioSource hospitalBips;
    public AudioSource typingAudioSource;
    public AudioClip typingKeySound;
    public float typingSpeed = 0.05f;

    void Start()
    {
        foreach (var s in steps) {
            s.sceneObject.SetActive(false);
            if(s.textDisplay) s.textDisplay.alpha = 0;
        }
        fadePanel.color = new Color(0, 0, 0, 1);
        StartCoroutine(PlayCutscene());
    }

    IEnumerator PlayCutscene()
    {
        for (int i = 0; i < steps.Length; i++)
        {
            steps[i].sceneObject.SetActive(true);
            if(steps[i].textDisplay) steps[i].textDisplay.text = "";

            // Fade In (Panel + Text Alpha together)
            yield return StartCoroutine(FadeStep(steps[i], true));

            if (steps[i].triggerHospitalBips && hospitalBips != null && !hospitalBips.isPlaying)
                hospitalBips.Play();
            
            if (steps[i].textDisplay != null && !string.IsNullOrEmpty(steps[i].dialogueText))
                yield return StartCoroutine(TypeText(steps[i].textDisplay, steps[i].dialogueText));

            yield return new WaitForSeconds(steps[i].lingerTime);

            // Fade Out (Panel + Text Alpha together)
            yield return StartCoroutine(FadeStep(steps[i], false));
            
            steps[i].sceneObject.SetActive(false);
            if (i == steps.Length - 1) SceneManager.LoadScene("MainMenu");
        }
    }

    IEnumerator FadeStep(CutsceneStep step, bool fadeIn)
    {
        float elapsed = 0;
        float startAlpha = fadeIn ? 1 : 0;
        float endAlpha = fadeIn ? 0 : 1;

        while (elapsed < step.fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / step.fadeDuration;
            float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, t);
            
            fadePanel.color = new Color(0, 0, 0, currentAlpha);
            if(step.textDisplay) step.textDisplay.alpha = (1 - currentAlpha); // Text fades perfectly with panel
            
            yield return null;
        }
    }

    IEnumerator TypeText(TextMeshProUGUI display, string text)
    {
        foreach (char c in text.ToCharArray())
        {
            display.text += c;
            if (typingAudioSource != null && typingKeySound != null)
                typingAudioSource.PlayOneShot(typingKeySound);

            float pause = typingSpeed;
            if (c == '.' || c == '!' || c == '?') pause = typingSpeed * 10f;
            else if (c == ',' || c == ';' || c == ':') pause = typingSpeed * 5f;
            yield return new WaitForSeconds(pause);
        }
    }
}