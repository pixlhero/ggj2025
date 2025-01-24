using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    // A singleton instance to allow easy access from any script
    public static AudioManager Instance;

    [Serializable]
    public class Sound
    {
        public string name;         // Identifier for the sound
        public AudioClip clip;      // The audio clip
        [Range(0f, 1f)]
        public float volume = 1f;   // Volume level for this sound
        [Range(.1f, 3f)]
        public float pitch = 1f;    // Pitch level for this sound

        [HideInInspector]
        public AudioSource source;  // The AudioSource for playing this sound
    }

    // An array of sound objects that you can configure in the Inspector
    public Sound[] sounds;

    private void Awake()
    {
        // Enforce singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Make sure the AudioManager persists across scene loads
        DontDestroyOnLoad(gameObject);

        // Create an AudioSource component for each sound
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            // You can set more AudioSource properties here, e.g. loop, spatial settings, etc.
        }
    }

    /// <summary>
    /// Call this method to play a sound by its name.
    /// Example usage: AudioManager.Instance.Play("Explosion");
    /// </summary>
    /// <param name="soundName">Name of the sound to play</param>
    public void Play(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        if (s == null)
        {
            Debug.LogWarning("AudioManager: Sound not found: " + soundName);
            return;
        }
        s.source.Play();
    }
}
