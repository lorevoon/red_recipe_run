using UnityEngine;
using TMPro;
using UnityEngine.Rendering;  // General rendering namespace
using UnityEngine.Rendering.Universal;  // Specific URP namespace

public class TimeController : Singleton<TimeController>
{
    [SerializeField] private bool IsCycleOn;

    [SerializeField] private Volume _globalVolume; // Reference to the global post-processing volume
    [SerializeField] private Transform _clockHand;

    public float _timeSpeed = 1.0f; // How fast time progresses
    public float _timeOfDay = 6.0f; // Time of day in hours

    [SerializeField] private Light2D _globalLight; // Reference to a global light source to simulate sunlight/moonlight
    [SerializeField] private Color _dayColor;
    [SerializeField] private Color _nightColor;

    [SerializeField] private AudioSource _dayAudioSource;
    [SerializeField] private AudioSource _nightAudioSource;

    public bool IsNight { get; private set; }

    void Start()
    {
        // Initialize the light with daytime settings
        _globalLight.color = _dayColor;
        _globalLight.intensity = 1.0f;
        IsNight = false;
        _dayAudioSource.Play();
        _clockHand.localEulerAngles = new Vector3(0, 0, 180f);
    }

    void Update()
    {
        if (!IsCycleOn)
        {
            _dayAudioSource.Stop();
            _nightAudioSource.Stop();
            return;
        }
        UpdateTimeOfDay();
        UpdateLighting();
        UpdateClockHandRotation();
        UpdateAudio();
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
    
    void UpdateClockHandRotation()
    {
        float twelveHourTime = _timeOfDay % 12f;
        // Calculate rotation (0-360 degrees over 12 hours)
        float rotationDegrees = (twelveHourTime / 12f) * 360f;
        // Apply rotation (negative because Unity rotates clockwise with positive angles)
        _clockHand.localEulerAngles = new Vector3(0, 0, -rotationDegrees);
    }
    
    void UpdateAudio()
    {
        // Play bird sounds during the day (6 AM to 6 PM)
        if ((_timeOfDay >= 6 && _timeOfDay < 18) && !_dayAudioSource.isPlaying)
        {
            _nightAudioSource.Stop();
            _dayAudioSource.Play();
        }
        // Play owl sounds during the night (6 PM to 6 AM)
        else if ((_timeOfDay >= 18 || _timeOfDay < 6) && !_nightAudioSource.isPlaying)
        {
            _dayAudioSource.Stop();
            _nightAudioSource.Play();
        }
    }

}
