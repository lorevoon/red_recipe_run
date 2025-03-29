using UnityEngine;
using UnityEngine.Rendering;  // General rendering namespace
using UnityEngine.Rendering.Universal;  // Specific URP namespace

public class DayNightCycle : MonoBehaviour
{
    public Volume globalVolume; // Reference to the global post-processing volume
    public Transform clockHand; // Reference to the clock hand transform

    public float timeSpeed = 1.0f; // How fast time progresses
    private float timeOfDay = 0.0f; // Time of day in hours

    public Light2D globalLight; // Reference to a global light source to simulate sunlight/moonlight
    public Color dayColor;
    public Color nightColor;

    public AudioSource dayAudioSource; // Reference for day audio source
    public AudioSource nightAudioSource; // Reference for night audio source

    void Start()
    {
        // Initialize the light with daytime settings
        globalLight.color = dayColor;
        globalLight.intensity = 1.0f;
    }

    void Update()
    {
        UpdateTimeOfDay();
        UpdateLighting();
        UpdateClockHandRotation(); // Update the clock hand rotation
        UpdateAudio();
    }

    void UpdateTimeOfDay()
    {
        // Time progresses continuously
        timeOfDay += Time.deltaTime * timeSpeed;
        if (timeOfDay >= 24.0f)
        {
            timeOfDay = 0.0f; // Reset the time at midnight
        }
    }

    void UpdateLighting()
    {
        // Interpolate light color and intensity based on time of day
        if (timeOfDay < 6 || timeOfDay > 18) // Nighttime
        {
            globalLight.color = nightColor;
            globalLight.intensity = 0.5f; // Dimmer light at night
        }
        else // Daytime
        {
            globalLight.color = dayColor;
            globalLight.intensity = 1.0f; // Brighter light during the day
        }

        // Adjust post-processing weight based on time for smooth transition
        if (timeOfDay >= 18 && timeOfDay < 20) // Dusk
        {
            globalVolume.weight = (timeOfDay - 18) / 2; // Gradually increase weight
        }
        else if (timeOfDay >= 6 && timeOfDay < 8) // Dawn
        {
            globalVolume.weight = 1 - ((timeOfDay - 6) / 2); // Gradually decrease weight
        }
        else
        {
            globalVolume.weight = timeOfDay > 20 || timeOfDay < 6 ? 1 : 0;
        }
    }

    void UpdateClockHandRotation()
    {
        float rotationDegrees = ((timeOfDay / 24f) * 360f) - 90f; // Subtract 90 to start from 12 o'clock
        clockHand.localEulerAngles = new Vector3(0, 0, -rotationDegrees);
    }

    void UpdateAudio()
    {
        // Play bird sounds during the day (6 AM to 6 PM)
        if ((timeOfDay >= 6 && timeOfDay < 18) && !dayAudioSource.isPlaying)
        {
            nightAudioSource.Stop();
            dayAudioSource.Play();
        }
        // Play owl sounds during the night (6 PM to 6 AM)
        else if ((timeOfDay >= 18 || timeOfDay < 6) && !nightAudioSource.isPlaying)
        {
            dayAudioSource.Stop();
            nightAudioSource.Play();
        }
    }
}
