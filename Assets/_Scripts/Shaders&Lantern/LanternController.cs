using UnityEngine;
using UnityEngine.Rendering.Universal; // Make sure to include this namespace

public class LanternController : MonoBehaviour {
    [SerializeField] private Light2D _light2D; // Reference to the Light2D component
    [SerializeField] private AudioClip _turnOnSound;
    [SerializeField] private AudioClip _turnOffSound;

    private AudioSource _audioSource;
    private TimeController _timeController;
    private float _baseRadius; // Store the initial radius

    public bool _isLightOn = false; // Track the state of the light

    void Start()
    {
        _timeController = TimeController.Instance;
        _audioSource = GetComponent<AudioSource>();
        _light2D.enabled = false;
        
        // Store the base radius of the light
        _baseRadius = _light2D.pointLightOuterRadius;
    }
    
    void Update() {
        // Check if the 'Z' key is pressed
        if (Input.GetKeyDown(KeyCode.Z) && _timeController.IsNight) {
            _isLightOn = !_isLightOn; // Toggle the state of the light
            _light2D.enabled = _isLightOn; // Enable or disable the Light2D component
            _audioSource.clip = _isLightOn ? _turnOnSound : _turnOffSound;
            _audioSource.Play();
            
            // Update the light radius when turning on
            if (_isLightOn) {
                UpdateLightRadius();
            }
        }
        // turn off lantern when not night
        else if (!_timeController.IsNight) {
            _isLightOn = false;
            _light2D.enabled = false;
        }
    }
    
    // Called whenever the lantern is turned on or when the upgrade level changes
    public void UpdateLightRadius() 
    {
        if (UpgradeManager.Instance != null) 
        {
            // Get the current lantern radius from UpgradeManager
            float currentRadius = UpgradeManager.Instance.GetCurrentLanternRange();
            
            // Apply the new radius to the light
            _light2D.pointLightOuterRadius = currentRadius;
            
            // Adjust the inner radius to maintain the same ratio
            float ratio = _light2D.pointLightInnerRadius / _light2D.pointLightOuterRadius;
            _light2D.pointLightInnerRadius = currentRadius * ratio;
            
            Debug.Log($"Lantern radius updated to {currentRadius}");
        }
    }
}