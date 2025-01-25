using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Serializable]
    public class Sound
    {
        public string name;                  // Identifier for the sound
        public AudioClip clip;               // The audio clip
        [Range(0f, 1f)]
        public float volume = 1f;            // Volume level for this sound
        [Range(.1f, 3f)]
        public float pitch = 1f;             // Pitch level for this sound
        public bool loop = false;            // Should the sound loop?

        [HideInInspector]
        public AudioSource source;           // The AudioSource for playing this sound
    }

    // An array of sound objects that you can configure in the Inspector
    public Sound[] sounds;

    public Sound[] announcerSounds;

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
            s.source.loop = s.loop;  // Set looping based on the Sound's boolean
            // You can set more AudioSource properties here, e.g. spatialBlend, etc.
        }

        // Create an AudioSource component for each announcer sound
        foreach (Sound s in announcerSounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;  // Set looping based on the Sound's boolean
            // You can set more AudioSource properties here, e.g. spatialBlend, etc.
        }
    }

    /// Plays a sound by name (non-random pitch).
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

    /// Plays a sound by name, applying a random pitch in the range [0.5, 1.5].
    public void PlayRandomizedPitch(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        if (s == null)
        {
            Debug.LogWarning("AudioManager: Sound not found: " + soundName);
            return;
        }

        // Randomize pitch between 0.5 and 1.5
        s.source.pitch = UnityEngine.Random.Range(0.5f, 1.5f);
        s.source.Play();
    }

    /// Plays an announcer sound by name.
    public void PlayAnnouncer(string soundName)
    {
        Sound s = Array.Find(announcerSounds, sound => sound.name == soundName);
        if (s == null)
        {
            Debug.LogWarning("AudioManager: Sound not found: " + soundName);
            return;
        }
        s.source.Play();
    }

    /// selects an announcer sound based on a score.
    public void PlayCalculatedAnnouncerSound(int score)
    {
        switch (score)
        {
            case 1:
                PlayAnnouncer("FirstBlood");
                break;
            case 2:
                PlayAnnouncer("DoubleKill");
                break;
            case 3:
                PlayAnnouncer("TripleKill");
                break;
            default:
                PlayRandomAnnouncer();
                break;
        }
    }

    /// Plays a random announcer sound from a set of possibilities.
    void PlayRandomAnnouncer()
    {
        int randomIndex = UnityEngine.Random.Range(0, announcerSounds.Length - 3);

        // Play a random sound from the announcerSounds array
        switch (randomIndex)
        {
            case 0:
                PlayAnnouncer("TangoDown");
                break;
            case 1:
                PlayAnnouncer("Unstoppable");
                break;
            case 2:
                PlayAnnouncer("PayloadDelivered");
                break;
            case 3:
                PlayAnnouncer("EnemyEliminated");
                break;
            case 4:
                PlayAnnouncer("Elimination");
                break;
            default:
                Debug.LogWarning("AudioManager: Random announcer sound not found.");
                break;
        }
    }

    /// Play a looping sound by name (useful for something like a continuous train sound).
    public void PlayLoopingSound(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        if (s == null)
        {
            Debug.LogWarning("AudioManager: Sound not found: " + soundName);
            return;
        }
        // Ensure looping is on
        s.source.loop = true;
        s.source.Play();
    }


    /// Stop a looping sound by name.
    public void StopLoopingSound(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        if (s == null)
        {
            Debug.LogWarning("AudioManager: Sound not found: " + soundName);
            return;
        }
        s.source.Stop();
    }
}
