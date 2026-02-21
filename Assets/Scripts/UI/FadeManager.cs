using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    [SerializeField] CanvasGroup fadeGroup;
    [SerializeField] float fadeDuration = 0.5f;

    void Awake()
    {
        fadeGroup.alpha = 0f;
    }

    public void LoadSceneWithFade(string sceneName)
        => StartCoroutine(CoLoad(sceneName));

    IEnumerator CoLoad(string sceneName)
    {
        yield return Fade(1f);

        yield return null;

        yield return SceneManager.LoadSceneAsync(sceneName);

        yield return Fade(0f); // fade back in
    }

    IEnumerator Fade(float target)
    {
        float start = fadeGroup.alpha;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / fadeDuration; // unscaled so it works if timeScale=0
            fadeGroup.alpha = Mathf.Lerp(start, target, t);
            yield return null;
        }

        fadeGroup.alpha = target;
    }
}
