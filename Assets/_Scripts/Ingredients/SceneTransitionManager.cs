using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class SceneTransitionManager : Singleton<SceneTransitionManager>
{
    private CanvasGroup fadeCanvas;
    private float fadeDuration = 0.2f;

    private void Start()
    {
        fadeCanvas = GetComponentInChildren<CanvasGroup>();
        fadeCanvas.alpha = 1f;
        fadeCanvas.blocksRaycasts = true;
        StartCoroutine(FadeOut());
    }

    public void TransitionTo(string sceneName)
    {
        StartCoroutine(TransitionRoutine(sceneName));
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        yield return StartCoroutine(FadeIn());

        Debug.Log("transitioning to scene: " + sceneName);
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName);
        while (!loadOp.isDone) yield return null;

        yield return StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float time = 0;
        while (time < fadeDuration)
        {
            fadeCanvas.alpha = 1 - (time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }
        fadeCanvas.alpha = 0;
        fadeCanvas.blocksRaycasts = false;
    }

    private IEnumerator FadeIn()
    {
        fadeCanvas.blocksRaycasts = true;
        float time = 0;
        while (time < fadeDuration)
        {
            fadeCanvas.alpha = time / fadeDuration;
            time += Time.deltaTime;
            yield return null;
        }
        fadeCanvas.alpha = 1;
    }
}