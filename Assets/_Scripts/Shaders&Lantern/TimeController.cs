using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TimeController : Singleton<TimeController>
{
    [SerializeField] private bool IsCycleOn;

    [SerializeField] private Volume _globalVolume;
    [SerializeField] private Transform _clockHand;

    [SerializeField] private Light2D _globalLight;
    [SerializeField] private Color _dayColor;
    [SerializeField] private Color _nightColor;

    [SerializeField] private AudioSource _dayAudioSource;
    [SerializeField] private AudioSource _nightAudioSource;

    private float baseCycleDuration = 120f; // 2 minutes
    private float cycleIncrement = 30f;
    private int completedCycles = 0;
    private float currentCycleDuration;
    private float _timeSpeed;

    private float _elapsedTime = 6f; // Start at 6AM → 180 degrees
    public bool IsNight { get; private set; }

    void Start()
    {
        currentCycleDuration = baseCycleDuration;
        SetTimeSpeedForCurrentCycle();

        _globalLight.color = _dayColor;
        _globalLight.intensity = 1.0f;
        IsNight = false;

        _dayAudioSource.Play();
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

    void SetTimeSpeedForCurrentCycle()
    {
        _timeSpeed = 24f / currentCycleDuration;
    }

    void UpdateTimeOfDay()
    {
        float deltaTimeHours = Time.deltaTime * _timeSpeed;
        _elapsedTime += deltaTimeHours;

        // Check for completed full 24h cycles (not just up to 30)
        float totalTimePassed = _elapsedTime - 6f; // total time since first 6AM
        int newCycleCount = Mathf.FloorToInt(totalTimePassed / 24f);

        if (newCycleCount > completedCycles)
        {
            completedCycles = newCycleCount;
            currentCycleDuration = baseCycleDuration + (completedCycles * cycleIncrement);
            SetTimeSpeedForCurrentCycle();
        }
    }

    void UpdateClockHandRotation()
    {
        float twelveHourTime = _elapsedTime % 12f;
        float rotationDegrees = (twelveHourTime / 12f) * 360f;
        _clockHand.localEulerAngles = new Vector3(0, 0, -rotationDegrees);
    }

    void UpdateLighting()
    {
        float timeOfDay = _elapsedTime % 24f;

        if (timeOfDay >= 18f || timeOfDay < 6f) // Night
        {
            _globalLight.color = _nightColor;
            _globalLight.intensity = 0.5f;
            IsNight = true;
        }
        else // Day
        {
            _globalLight.color = _dayColor;
            _globalLight.intensity = 1.0f;
            IsNight = false;
        }

        if (timeOfDay >= 18f && timeOfDay < 20f) // Dusk
        {
            _globalVolume.weight = (timeOfDay - 18f) / 2f;
        }
        else if (timeOfDay >= 6f && timeOfDay < 8f) // Dawn
        {
            _globalVolume.weight = 1f - ((timeOfDay - 6f) / 2f);
        }
        else
        {
            _globalVolume.weight = (timeOfDay >= 20f || timeOfDay < 6f) ? 1f : 0f;
        }

        _globalLight.color = Color.Lerp(_dayColor, _nightColor, _globalVolume.weight);
    }

    void UpdateAudio()
    {
        float timeOfDay = _elapsedTime % 24f;

        if (_dayAudioSource != null && (timeOfDay >= 6f && timeOfDay < 18f) && !_dayAudioSource.isPlaying)
        {
            if (_nightAudioSource != null) _nightAudioSource.Stop();
            _dayAudioSource.Play();
        }
        else if (_nightAudioSource != null && (timeOfDay >= 18f || timeOfDay < 6f) && !_nightAudioSource.isPlaying)
        {
            if (_dayAudioSource != null) _dayAudioSource.Stop();
            _nightAudioSource.Play();
        }
    }

}
