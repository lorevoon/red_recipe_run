using UnityEngine;
using UnityEngine.Rendering.Universal; 

public class LanternToggle : MonoBehaviour {
    public Light2D light2D; // Reference to the Light2D component
    public AudioClip turnOnSound; // Drag your on sound here in Unity's Inspector
    public AudioClip turnOffSound; // Drag your off sound here in Unity's Inspector
    private AudioSource audioSource; // Audio source component
    private bool isLightOn = false; // State of the lantern

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component attached to the same object
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) // Check if 'Z' key is pressed
        {
            isLightOn = !isLightOn; // Toggle the state of the light
            light2D.enabled = isLightOn; // Enable or disable the Light2D component

            // Set the clip based on the lantern's state and play it
            audioSource.clip = isLightOn ? turnOnSound : turnOffSound;
            audioSource.Play();
        }
    }
}
