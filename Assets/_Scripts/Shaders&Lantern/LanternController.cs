using UnityEngine;
using UnityEngine.Rendering.Universal; // Make sure to include this namespace

public class LanternController : MonoBehaviour {
    [SerializeField] private Light2D _light2D; // Reference to the Light2D component
    private TimeController _timeController;

    public bool _isLightOn = false; // Track the state of the light

    void Start()
    {
        _timeController = TimeController.Instance;
        _light2D.enabled = false;
    }
    
    void Update() {
        // Check if the 'Z' key is pressed
        if (Input.GetKeyDown(KeyCode.Z) && _timeController.IsNight) {
            _isLightOn = !_isLightOn; // Toggle the state of the light
            _light2D.enabled = _isLightOn; // Enable or disable the Light2D component
        }
        // turn off lantern when not night
        else if (!_timeController.IsNight) {
            _isLightOn = false;
            _light2D.enabled = false;
        }
    }
}
