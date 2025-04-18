using System.Collections;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioSource daySource;
    [SerializeField] private AudioSource nightSource;
    private float fadeDuration = 2f;

    private bool isDay = true;
    private AudioSource activeSource;
    private AudioSource inactiveSource;

    void Start()
    {
        daySource.Play();
        nightSource.Play();

        activeSource = daySource;
        inactiveSource = nightSource;
        inactiveSource.volume = 0f;
    }

    public void SwitchToDay()
    {
        if (isDay) return;
        activeSource = daySource;
        inactiveSource = nightSource;
        SyncSources(daySource, nightSource);
        StartCoroutine(FadeMusic(daySource, nightSource));
        isDay = true;
    }

    public void SwitchToNight()
    {
        if (!isDay) return;
        activeSource = nightSource;
        inactiveSource = daySource;
        SyncSources(nightSource, daySource);
        StartCoroutine(FadeMusic(nightSource, daySource));
        isDay = false;
    }

    private void SyncSources(AudioSource target, AudioSource reference)
    {
        target.timeSamples = reference.timeSamples;
    }

    private IEnumerator FadeMusic(AudioSource toFadeIn, AudioSource toFadeOut)
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            float t = timer / fadeDuration;
            toFadeIn.volume = Mathf.Lerp(0f, 1f, t);
            toFadeOut.volume = Mathf.Lerp(1f, 0f, t);
            timer += Time.deltaTime;
            yield return null;
        }

        toFadeIn.volume = 1f;
        toFadeOut.volume = 0f;

        activeSource = toFadeIn;
        inactiveSource = toFadeOut;
    }
}
