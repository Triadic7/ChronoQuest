using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    /// <summary>
    /// Gets the singleton instance of the SoundManager.
    /// </summary>
    public static SoundManager Instance { get; private set; }

    /// <summary>
    /// Gets or sets the default volume level for sound effects.
    /// </summary>
    [Header("Defaults")]
    [SerializeField]
    [Min(0f)]
    private float defaultSfxVolume = 1f;

    /// <summary>
    /// Default mute state for sound effects.
    /// </summary>
    [SerializeField]
    private bool defaultMute;

    /// <summary>
    /// Gets or sets the initial number of objects to allocate in the pool.
    /// </summary>
    [Header("Pooling")]
    [SerializeField]
    [Min(1)]
    private int initialPoolSize = 8;

    /// <summary>
    /// Represents the collection of available audio sources in the pool.
    /// </summary>
    private readonly Queue<AudioSource> pool = new Queue<AudioSource>();

    /// <summary>
    /// The current volume level for sound effects.
    /// </summary>
    private float volume = 1f;

    /// <summary>
    /// Is the sound effects muted.
    /// </summary>
    private bool isMuted;

    /// <summary>
    /// Represents the configuration key used to store or retrieve the sound effects volume setting.
    /// </summary>
    private const string SfxVolumeKey = "Sfx.Volume";

    /// <summary>
    /// Represents the configuration key used to store or retrieve the muted state of sound effects.
    /// </summary>
    private const string SfxMutedKey = "Sfx.Muted";

    /// <summary>
    /// Gets the current volume level for sound effects.
    /// </summary>
    public float Volume => volume;

    /// <summary>
    /// Gets a value indicating whether the sound effects are muted.
    /// </summary>
    public bool IsMuted => isMuted;

    /// <summary>
    /// Initializes the singleton instance of the audio manager and prepares the audio source pool. If an instance
    /// already exists, destroys the current game object.
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        volume = Mathf.Clamp01(PlayerPrefs.GetFloat(SfxVolumeKey, defaultSfxVolume));
        isMuted = PlayerPrefs.GetInt(SfxMutedKey, defaultMute ? 1 : 0) == 1;

        for (int i = 0; i < initialPoolSize; i++)
        {
            pool.Enqueue(CreatePooledSource());
        }
    }

    /// <summary>
    /// Performs cleanup operations when the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    /// <summary>
    /// Plays the specified audio clip once at the current position with the given volume scale.
    /// </summary>
    /// <param name="clip">The audio clip to play. Cannot be null.</param>
    /// <param name="volumeScale">A multiplier applied to the audio source's volume.</param>
    public void Play(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null || isMuted)
        {
            return;
        }

        AudioSource src = GetSource();
        src.transform.position = transform.position;
        src.spatialBlend = 0f;
        src.clip = clip;
        src.loop = false;
        src.volume = Mathf.Clamp01(volume * Mathf.Clamp01(volumeScale));
        src.Play();
    }

    /// <summary>
    /// Sets the sound effect volume to the specified level, optionally saving the value to storage.
    /// </summary>
    /// <param name="newVolume">The new volume level to set.</param>
    /// <param name="save">>A value indicating whether to save the muted state to storage.</param>
    public void SetVolume(float newVolume, bool save = true)
    {
        volume = Mathf.Clamp01(newVolume);

        if (save)
        {
            PlayerPrefs.SetFloat(SfxVolumeKey, volume);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Sets the muted state for sound effects and optionally saves the setting to storage.
    /// </summary>
    /// <param name="muted">A value indicating whether sound effects should be muted..</param>
    /// <param name="save">A value indicating whether to save the muted state to storage.</param>
    public void SetMuted(bool muted, bool save = true)
    {
        isMuted = muted;

        if (save)
        {
            PlayerPrefs.SetInt(SfxMutedKey, muted ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Toggles the muted state between muted and unmuted.
    /// </summary>
    /// <param name="save">>A value indicating whether to save the muted state to persistent storage.</param>
    public void ToggleMuted(bool save = true) => SetMuted(!isMuted, save);

    private AudioSource GetSource()
    {
        while (pool.Count > 0)
        {
            AudioSource src = pool.Dequeue();
            if (src != null)
            {
                if (!src.isPlaying)
                {
                    pool.Enqueue(src);
                    return src;
                }

                pool.Enqueue(src);
            }
        }

        AudioSource created = CreatePooledSource();
        pool.Enqueue(created);
        return created;
    }

    /// <summary>
    /// Creates a new AudioSource instance configured for use in a sound effect pool.
    /// </summary>
    /// <returns>An AudioSource component attached to a new GameObject, initialized with default settings for pooled sound
    /// effects.</returns>
    private AudioSource CreatePooledSource()
    {
        GameObject go = new GameObject("SFXSource");
        go.transform.SetParent(transform, false);

        AudioSource src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = false;
        src.spatialBlend = 0f;
        return src;
    }
}
