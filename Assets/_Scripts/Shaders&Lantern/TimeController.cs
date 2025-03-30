using UnityEngine;
using TMPro;
using UnityEngine.Rendering;  // General rendering namespace
using UnityEngine.Rendering.Universal;  // Specific URP namespace

public class TimeController : Singleton<TimeController>
{
    [SerializeField] private bool IsCycleOn;
    
    [SerializeField] private TextMeshProUGUI _timeDisplay; // Reference to the TextMesh Pro UI component for time
    [SerializeField] private Volume _globalVolume; // Reference to the global post-processing volume

    private float _timeSpeed = 1.0f; // How fast time progresses
    private float _timeOfDay = 0.0f; // Time of day in hours

    [SerializeField] private Light2D _globalLight; // Reference to a global light source to simulate sunlight/moonlight
    [SerializeField] private Color _dayColor;
    [SerializeField] private Color _nightColor;

    public bool IsNight { get; private set; }

    void Start()
    {
        // Initialize the light with daytime settings
        _globalLight.color = _dayColor;
        _globalLight.intensity = 1.0f;
        IsNight = false;
    }

    void Update()
    {
        if (!IsCycleOn) return;
        UpdateTimeOfDay();
        UpdateLighting();
        UpdateUI();
    }

    void UpdateTimeOfDay()
    {
        // Time progresses continuously
        _timeOfDay += Time.deltaTime * _timeSpeed;
        if (_timeOfDay >= 24.0f)
        {
            _timeOfDay = 0.0f; // Reset the time at midnight
        }
    }

    void UpdateLighting()
    {
        // Interpolate light color and intensity based on time of day
        if (_timeOfDay < 6 || _timeOfDay > 18) // Nighttime
        {
            _globalLight.color = _nightColor;
            _globalLight.intensity = 0.5f; // Dimmer light at night
            IsNight = true;
        }
        else // Daytime
        {
            _globalLight.color = _dayColor;
            _globalLight.intensity = 1.0f; // Brighter light during the day
            IsNight = false;
        }

        // Adjust post-processing weight based on time for smooth transition
        if (_timeOfDay >= 18 && _timeOfDay < 20) // Dusk
        {
            _globalVolume.weight = (_timeOfDay - 18) / 2; // Gradually increase weight
        }
        else if (_timeOfDay >= 6 && _timeOfDay < 8) // Dawn
        {
            _globalVolume.weight = 1 - ((_timeOfDay - 6) / 2); // Gradually decrease weight
        }
        else
        {
            _globalVolume.weight = _timeOfDay > 20 || _timeOfDay < 6 ? 1 : 0;
        }
        _globalLight.color = Color.Lerp(_dayColor, _nightColor, _globalVolume.weight);

    }

    void UpdateUI()
    {
        // Update the time display
        _timeDisplay.text = string.Format("{0:00}:{1:00}", (int)_timeOfDay, (int)(_timeOfDay * 60) % 60);
    }

}
