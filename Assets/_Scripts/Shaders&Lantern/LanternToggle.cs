using UnityEngine;
using UnityEngine.Rendering.Universal; // Make sure to include this namespace

public class LanternToggle : MonoBehaviour {
    public Light2D light2D; // Reference to the Light2D component

    private bool isLightOn = false; // Track the state of the light

    void Update() {
        // Check if the 'Z' key is pressed
        if (Input.GetKeyDown(KeyCode.Z)) {
            isLightOn = !isLightOn; // Toggle the state of the light
            light2D.enabled = isLightOn; // Enable or disable the Light2D component
        }
    }
}
