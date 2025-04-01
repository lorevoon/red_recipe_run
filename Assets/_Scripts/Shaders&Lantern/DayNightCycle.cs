using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    public Volume globalVolume;
    public Transform clockHand;

    public float timeSpeed = 1.0f; // Speed of time progression
    private float timeOfDay = 6.0f; // Start at 6 AM

    public Light2D globalLight;
    public Color dayColor;
    public Color nightColor;

    public AudioSource dayAudioSource;
    public AudioSource nightAudioSource;

    void Start()
    {
        globalLight.color = dayColor;
        globalLight.intensity = 1.0f;

        // Start playing day audio at the beginning
        dayAudioSource.Play();
        clockHand.localEulerAngles = new Vector3(0, 0, 180f); // Start at 6 o'clock (pointing down)
    }

    void Update()
    {
        UpdateTimeOfDay();
        UpdateLighting();
        UpdateClockHandRotation();
        UpdateAudio();
    }

    void UpdateTimeOfDay()
    {
        // Time progresses continuously
        timeOfDay += Time.deltaTime * timeSpeed;

        // Ensure the timeOfDay wraps around after 24 hours
        if (timeOfDay >= 24.0f)
        {
            timeOfDay = 0.0f;
        }
    }

    void UpdateLighting()
    {
        if (timeOfDay >= 18.0f || timeOfDay < 6.0f) // Nighttime (6 PM to 6 AM)
        {
            globalLight.color = nightColor;
            globalLight.intensity = 0.5f;
        }
        else // Daytime (6 AM to 6 PM)
        {
            globalLight.color = dayColor;
            globalLight.intensity = 1.0f;
        }

        // Smooth transitions for global volume (dusk/dawn)
        if (timeOfDay >= 18 && timeOfDay < 20) // Dusk (6 PM to 8 PM)
        {
            globalVolume.weight = (timeOfDay - 18) / 2;
        }
        else if (timeOfDay >= 6 && timeOfDay < 8) // Dawn (6 AM to 8 AM)
        {
            globalVolume.weight = 1 - ((timeOfDay - 6) / 2);
        }
        else
        {
            globalVolume.weight = (timeOfDay >= 20 || timeOfDay < 6) ? 1 : 0;
        }
    }

    void UpdateClockHandRotation()
    {
        // Convert 24-hour time to 12-hour cycle (clock makes full rotation every 12 hours)
        float twelveHourTime = timeOfDay % 12f;
        // Calculate rotation (0-360 degrees over 12 hours)
        float rotationDegrees = (twelveHourTime / 12f) * 360f;
        // Apply rotation (negative because Unity rotates clockwise with positive angles)
        clockHand.localEulerAngles = new Vector3(0, 0, -rotationDegrees);
    }

    void UpdateAudio()
    {
        // Play day audio from 6 AM to 6 PM
        if (timeOfDay >= 6.0f && timeOfDay < 18.0f && !dayAudioSource.isPlaying)
        {
            nightAudioSource.Stop();
            dayAudioSource.Play();
        }
        // Play night audio from 6 PM to 6 AM
        else if ((timeOfDay >= 18.0f || timeOfDay < 6.0f) && !nightAudioSource.isPlaying)
        {
            dayAudioSource.Stop();
            nightAudioSource.Play();
        }
    }
}