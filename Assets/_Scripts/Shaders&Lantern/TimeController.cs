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
    
    [SerializeField] private GameObject _faceLight;

    private float baseCycleDuration = 30f; // 2 minutes
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
        // convert real seconds into game hours based on speed
        float deltaTimeHours = Time.deltaTime * _timeSpeed;
        _elapsedTime += deltaTimeHours;

        // How many full 6AM→6AM cycles have passed?
        float cycleLengthInGameHours = 24f;
        float gameTimeSinceStart = _elapsedTime - 6f;

        int newCycleCount = Mathf.FloorToInt(gameTimeSinceStart / cycleLengthInGameHours);

        if (newCycleCount > completedCycles)
        {
            completedCycles = newCycleCount;

            // Each cycle adds 30 real-time seconds to the next day/night cycle
            currentCycleDuration = baseCycleDuration + (completedCycles * cycleIncrement);

            SetTimeSpeedForCurrentCycle(); // recalculate time speed
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
            _faceLight.SetActive(true);
            _globalLight.color = _nightColor;
            _globalLight.intensity = 0.5f;
            IsNight = true;
        }
        else // Day
        {
            _faceLight.SetActive(false);
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
            AudioManager.Instance.SwitchToDay();
        }
        else if (_nightAudioSource != null && (timeOfDay >= 18f || timeOfDay < 6f) && !_nightAudioSource.isPlaying)
        {
            if (_dayAudioSource != null) _dayAudioSource.Stop();
            _nightAudioSource.Play();
            AudioManager.Instance.SwitchToNight();
        }
    }

}
