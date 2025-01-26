using UnityEngine;
using System;
using System.Collections;
using DG.Tweening; // <-- Include DOTween
// Note: We still need this for Coroutines used elsewhere, like the random announcer coroutine
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Serializable]
    public class Sound
    {
        public string name;                  // Identifier for the sound
        public AudioClip clip;               // The audio clip
        [Range(0f, 2f)]
        public float volume = 1f;            // Volume level for this sound
        [Range(.1f, 3f)]
        public float pitch = 1f;             // Pitch level for this sound
        public bool loop = false;            // Should the sound loop?

        [HideInInspector]
        public AudioSource source;           // The AudioSource for playing this sound
        // (Optional) You can store a reference to a running Tween if you want to cancel or manage it later
        [HideInInspector]
        public Tween fadeTween;
    }

    // An array of sound objects that you can configure in the Inspector
    public Sound[] sounds;
    public Sound[] announcerSounds;

    // Store reference to coroutine so we can stop it if needed
    private Coroutine randomAnnouncerCoroutine;

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
            s.source.pitch = 1;
            s.source.loop = s.loop;  // Set looping based on the Sound's boolean
            s.source.dopplerLevel = 0;  // Disable doppler effect, its buggy
        }

        // Create an AudioSource component for each announcer sound
        foreach (Sound s in announcerSounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = 1;
            s.source.loop = s.loop;  // Set looping based on the Sound's boolean
            s.source.dopplerLevel = 0;  // Disable doppler effect, its buggy
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

    /// Start playing random announcer sounds in intervals of 30 to 60 seconds.
    public void StartRandomInterval(string soundName)
    {
        // Only start the coroutine if it's not already running
        if (randomAnnouncerCoroutine == null)
        {
            randomAnnouncerCoroutine = StartCoroutine(RandomIntervalCoroutine(soundName));
        }
    }

    /// Stop playing random announcer sounds.
    public void StopRandomInterval()
    {
        // If the coroutine is running, stop it
        if (randomAnnouncerCoroutine != null)
        {
            StopCoroutine(randomAnnouncerCoroutine);
            randomAnnouncerCoroutine = null;
        }
    }

    /// Coroutine that plays a random announcer sound
    /// at random intervals between 30 and 60 seconds.
    private IEnumerator RandomIntervalCoroutine(string soundName)
    {
        while (true)
        {
            // Wait for a random amount of time between 30 and 60 seconds
            float waitTime = UnityEngine.Random.Range(30f, 60f);
            yield return new WaitForSeconds(waitTime);

            // Play a random announcer sound
            Play(soundName);
        }
    }

    /// <summary>
    /// Starts a looped sound from volume = 0 to targetVolume using DOTween for fading in.
    /// </summary>
    public void StartLoopingFadeIn(string soundName, float targetVolume, float fadeDuration)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        if (s == null)
        {
            Debug.LogWarning("AudioManager: Sound not found: " + soundName);
            return;
        }

        // If a fade tween is already running on this sound, kill it
        if (s.fadeTween != null && s.fadeTween.IsActive())
        {
            s.fadeTween.Kill();
        }

        // Set the source for looping, reset volume, and play
        s.source.loop = true;
        s.source.volume = 0f;
        s.source.Play();

        // Use DOTween to fade from 0 to targetVolume
        s.fadeTween = s.source.DOFade(targetVolume, fadeDuration)
                              .SetUpdate(false)   // SetUpdate(false) -> normal time, SetUpdate(true) -> ignore timescale
                              .OnComplete(() =>
                              {
                                  // Optionally do something when fade-in completes
                              });
    }

    /// <summary>
    /// Fades out a looped sound to volume = 0 using DOTween, then stops it.
    /// </summary>
    public void StopLoopingFadeOut(string soundName, float fadeDuration)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        if (s == null)
        {
            Debug.LogWarning("AudioManager: Sound not found: " + soundName);
            return;
        }

        // If a fade tween is already running on this sound, kill it
        if (s.fadeTween != null && s.fadeTween.IsActive())
        {
            s.fadeTween.Kill();
        }

        // Fade out to volume=0 and stop the AudioSource at the end
        s.fadeTween = s.source.DOFade(0f, fadeDuration)
                              .SetUpdate(false)
                              .OnUpdate(() =>
                              {
                                  // Optionally do something while fading
                              })
                              .OnComplete(() =>
                              {
                                  s.source.volume = 0f;
                                  s.source.Stop();
                              });
    }
}
