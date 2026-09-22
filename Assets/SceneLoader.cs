using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Put this on an empty GameObject (e.g. "SceneLoader") in any scene
// that needs to switch scenes. Hook its methods up to interactable
// events or UI button OnClick in the Inspector.
public class SceneLoader : MonoBehaviour
{
    [Tooltip("Optional: CanvasGroup on a black Image in front of the camera. Leave empty for no fade.")]
    public CanvasGroup fadeGroup;
    public float fadeDuration = 0.5f;

    bool isLoading;

    void Start()
    {
        // Fade in from black when the scene opens
        if (fadeGroup != null)
        {
            fadeGroup.alpha = 1f;
            StartCoroutine(Fade(1f, 0f));
        }
    }

    // Type the exact scene name in the Inspector event field
    public void LoadScene(string sceneName)
    {
        if (isLoading) return; // ignore double grabs / double clicks
        isLoading = true;
        StartCoroutine(FadeAndLoad(sceneName));
    }

    // Handy for a restart button
    public void ReloadCurrentScene()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator FadeAndLoad(string sceneName)
    {
        if (fadeGroup != null)
            yield return Fade(0f, 1f);

        SceneManager.LoadScene(sceneName);
    }

    IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeGroup.alpha = Mathf.Lerp(from, to, t / fadeDuration);
            yield return null;
        }
        fadeGroup.alpha = to;
    }
}
