using UnityEngine;
using TMPro;
using UnityEngine.Rendering;  // General rendering namespace
using UnityEngine.Rendering.Universal;  // Specific URP namespace

public class DayNightCycle : MonoBehaviour
{
    public TextMeshProUGUI timeDisplay; // Reference to the TextMesh Pro UI component for time
    public Volume globalVolume; // Reference to the global post-processing volume

    public float timeSpeed = 1.0f; // How fast time progresses
    private float timeOfDay = 0.0f; // Time of day in hours

    public Light2D globalLight; // Reference to a global light source to simulate sunlight/moonlight
    public Color dayColor;
    public Color nightColor;

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
        UpdateUI();
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

    void UpdateUI()
    {
        // Update the time display
        timeDisplay.text = string.Format("{0:00}:{1:00}", (int)timeOfDay, (int)(timeOfDay * 60) % 60);
    }

}
