using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("AudioManager");
                    instance = go.AddComponent<AudioManager>();
                    instance.Initialize();
                }
            }
            return instance;
        }
    }

    private AudioSource[] audioSources;
    private AudioClip inventoryOpen;
    private AudioClip inventoryClose;
    private AudioClip upgradesOpen;
    private AudioClip upgradesClose;
    private AudioClip throwSound;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        // Create audio sources
        audioSources = new AudioSource[3]; // One for UI, one for gameplay, one for music if needed
        for (int i = 0; i < audioSources.Length; i++)
        {
            audioSources[i] = gameObject.AddComponent<AudioSource>();
            audioSources[i].playOnAwake = false;
        }

        // Load audio clips
        inventoryOpen = Resources.Load<AudioClip>("Audio/UI/inventory_on");
        inventoryClose = Resources.Load<AudioClip>("Audio/UI/inventory_off");
        upgradesOpen = Resources.Load<AudioClip>("Audio/UI/upgrades_on");
        upgradesClose = Resources.Load<AudioClip>("Audio/UI/upgrades_off");
        throwSound = Resources.Load<AudioClip>("Audio/UI/throw");

        // Debug logging for clip loading
        Debug.Log($"Loaded audio clips - Inventory Open: {inventoryOpen != null}, Close: {inventoryClose != null}");
        Debug.Log($"Loaded audio clips - Upgrades Open: {upgradesOpen != null}, Close: {upgradesClose != null}");
        Debug.Log($"Loaded audio clips - Throw: {throwSound != null}");

        if (!ValidateAudioClips())
        {
            Debug.LogError("Some audio clips failed to load in AudioManager!");
        }
    }

    private bool ValidateAudioClips()
    {
        return inventoryOpen != null && 
               inventoryClose != null && 
               upgradesOpen != null && 
               upgradesClose != null &&
               throwSound != null;
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning($"Attempted to play null audio clip!");
            return;
        }
        
        // Use the first available audio source or the one that's not playing
        AudioSource source = audioSources[0];
        foreach (AudioSource audioSource in audioSources)
        {
            if (!audioSource.isPlaying)
            {
                source = audioSource;
                break;
            }
        }
        
        Debug.Log($"Playing sound clip: {clip.name}");
        source.clip = clip;
        source.Play();
    }

    // Specific sound methods
    public void PlayInventoryOpen() => PlaySound(inventoryOpen);
    public void PlayInventoryClose() => PlaySound(inventoryClose);
    public void PlayUpgradesOpen() => PlaySound(upgradesOpen);
    public void PlayUpgradesClose() => PlaySound(upgradesClose);
    public void PlayThrow() => PlaySound(throwSound);
}
