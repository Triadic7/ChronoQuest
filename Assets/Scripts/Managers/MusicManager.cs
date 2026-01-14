using System;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    /// <summary>
    /// Represents music associated with a specific stage.
    /// </summary>
    [Serializable]
    public struct StageMusic
    {
        public int stageId;
        public AudioClip clip;
    }

    /// <summary>
    /// Gets the singleton instance of the MusicManager.
    /// </summary>
    public static MusicManager Instance { get; private set; }

    /// <summary>
    /// Represents the primary audio source used for music within the scene.
    /// </summary>
    [Header("Sources")]
    [SerializeField]
    private AudioSource musicSourceA;

    /// <summary>
    /// Represents the secondary audio source used for music playback in the scene.
    /// </summary>
    [SerializeField]
    private AudioSource musicSourceB;

    /// <summary>
    /// Gets or sets the default background music clip used when no specific music is assigned.
    /// </summary>
    [Header("Defaults")]
    [SerializeField]
    private AudioClip defaultMusic;

    /// <summary>
    /// Represents the audio clip used for main menu background music.
    /// </summary>
    [SerializeField]
    private AudioClip mainMenuMusic;

    /// <summary>
    /// An array of music tracks associated with each stage.
    /// </summary>
    [SerializeField]
    private StageMusic[] stageMusic;

    /// <summary>
    /// Gets or sets the default volume level used when initializing audio.
    /// </summary>
    [SerializeField]
    [Min(0f)]
    private float defaultVolume = 1f;

    /// <summary>
    /// Gets or sets the duration, in seconds, used for crossfade transitions.
    /// </summary>
    [SerializeField]
    [Min(0f)]
    private float crossfadeSeconds = 1f;

    /// <summary>
    /// Active audio source currently playing music.
    /// </summary>
    private AudioSource activeSource;

    /// <summary>
    /// Inactive audio source used for crossfading.
    /// </summary>
    private AudioSource inactiveSource;

    /// <summary>
    /// Volume level for music.
    /// </summary>
    private float volume = 1f;

    /// <summary>
    /// Is music muted?
    /// </summary>
    private bool isMuted;

    /// <summary>
    /// Is a crossfade currently in progress?
    /// </summary>
    private bool isCrossfading;

    /// <summary>
    /// Crossfade start time.
    /// </summary>
    private float crossfadeStartTime;

    /// <summary>
    /// Length of crossfade transition.
    /// </summary>
    private float crossfadeDuration;

    /// <summary>
    /// Crossfade starting volume.
    /// </summary>
    private float fromVolume;

    /// <summary>
    /// Crossfade target volume.
    /// </summary>
    private float toVolume;

    /// <summary>
    /// Represents the configuration key used to store or retrieve the music volume setting.
    /// </summary>
    private const string MusicVolumeKey = "Music.Volume";

    /// <summary>
    /// Represents the configuration key used to store the muted state of music settings.
    /// </summary>
    private const string MusicMutedKey = "Music.Muted";

    /// <summary>
    /// Occurs when the currently playing audio track changes.
    /// </summary>
    public event Action<AudioClip> OnTrackChanged;

    /// <summary>
    /// Provides a lookup table that maps stage identifiers to their corresponding audio clips.
    /// </summary>
    private Dictionary<int, AudioClip> stageMusicLookup;

    /// <summary>
    /// Gets the current audio volume level.
    /// </summary>
    public float Volume => volume;

    /// <summary>
    /// Gets a value indicating whether the audio is currently muted.
    /// </summary>
    public bool IsMuted => isMuted;

    /// <summary>
    /// Gets the audio clip currently assigned to the active audio source.
    /// </summary>
    public AudioClip CurrentClip => activeSource != null ? activeSource.clip : null;

    /// <summary>
    /// Initializes the music manager instance and configures audio sources, volume settings, and event hooks at
    /// startup.
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        EnsureAudioSources();

        BuildStageMusicLookup();

        volume = Mathf.Clamp01(PlayerPrefs.GetFloat(MusicVolumeKey, defaultVolume));
        isMuted = PlayerPrefs.GetInt(MusicMutedKey, 0) == 1;

        ApplyVolumeImmediate();

        activeSource = musicSourceA;
        inactiveSource = musicSourceB;

        HookGameEvents();
    }

    /// <summary>
    /// Initializes playback of the main menu or default background music.
    /// </summary>
    private void Start()
    {
        if (mainMenuMusic != null)
        {
            PlayMainMenuMusic(immediate: true);
            return;
        }

        if (defaultMusic != null && CurrentClip == null)
        {
            Play(defaultMusic, loop: true, fadeSeconds: 0f);
        }
    }

    /// <summary>
    /// Handles cleanup logic when the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (Instance == this)
        {
            UnhookGameEvents();
            Instance = null;
        }
    }

    /// <summary>
    /// Plays the main menu music track, optionally starting playback immediately without a fade-in.
    /// </summary>
    public void PlayMainMenuMusic(bool immediate = false)
    {
        if (mainMenuMusic == null)
        {
            return;
        }

        Play(mainMenuMusic, loop: true, fadeSeconds: immediate ? 0f : (float?)null);
    }

    public void PlayDefaultMusic(bool immediate = false)
    {
        if (defaultMusic == null)
        {
            return;
        }

        Play(defaultMusic, loop: true, fadeSeconds: immediate ? 0f : (float?)null);
    }

    /// <summary>
    /// Plays the background music associated with the specified stage.
    /// </summary>
    /// <param name="stageId">The identifier of the stage whose music should be played.</param>
    /// <param name="immediate">If true, the music starts immediately without a fade-in.</param>
    public void PlayStageMusic(int stageId, bool immediate = false)
    {
        if (!TryGetStageClip(stageId, out AudioClip clip) || clip == null)
        {
            if (defaultMusic != null)
            {
                Play(defaultMusic, loop: true, fadeSeconds: immediate ? 0f : (float?)null);
            }
            return;
        }

        Play(clip, loop: true, fadeSeconds: immediate ? 0f : (float?)null);
    }

    /// <summary>
    /// Performs a crossfade update between two audio sources if a crossfade is in progress.
    /// </summary>
    private void Update()
    {
        if (!isCrossfading)
        {
            return;
        }

        float t = crossfadeDuration <= 0f ? 1f : Mathf.Clamp01((Time.unscaledTime - crossfadeStartTime) / crossfadeDuration);
        float a = Mathf.Lerp(fromVolume, 0f, t);
        float b = Mathf.Lerp(0f, toVolume, t);

        SetSourceVolume(activeSource, a);
        SetSourceVolume(inactiveSource, b);

        if (t >= 1f)
        {
            activeSource.Stop();
            activeSource.clip = null;

            (activeSource, inactiveSource) = (inactiveSource, activeSource);

            SetSourceVolume(inactiveSource, 0f);
            isCrossfading = false;
        }
    }

    /// <summary>
    /// Plays the specified audio clip as background music, optionally looping and crossfading from the current track.
    /// </summary>
    /// <param name="clip">The audio clip to play.</param>
    /// <param name="loop">Indicates whether the clip should loop continuously.</param>
    /// <param name="fadeSeconds">The duration to crossfade from the current track to the new clip. If null, the default crossfade
    /// duration is used. If zero or negative, the transition occurs instantly.</param>
    /// <param name="targetVolume">The target volume for the new clip.</param>
    public void Play(AudioClip clip, bool loop = true, float? fadeSeconds = null, float? targetVolume = null)
    {
        if (clip == null)
        {
            Debug.LogWarning("MusicManager.Play called with null clip.");
            return;
        }

        EnsureAudioSources();

        if (activeSource == null || inactiveSource == null)
        {
            Debug.LogError("MusicManager is missing AudioSources.");
            return;
        }

        if (activeSource.clip == clip)
        {
            if (!activeSource.isPlaying)
            {
                activeSource.Play();
            }
            return;
        }

        inactiveSource.Stop();
        inactiveSource.clip = clip;
        inactiveSource.loop = loop;

        float fade = fadeSeconds ?? crossfadeSeconds;
        float v = Mathf.Clamp01(targetVolume ?? volume);

        if (isMuted)
        {
            v = 0f;
        }

        if (!activeSource.isPlaying || fade <= 0f)
        {
            activeSource.Stop();
            activeSource.clip = clip;
            activeSource.loop = loop;
            SetSourceVolume(activeSource, v);
            activeSource.Play();

            inactiveSource.clip = null;
            SetSourceVolume(inactiveSource, 0f);
            isCrossfading = false;

            OnTrackChanged?.Invoke(clip);
            return;
        }

        // Crossfade
        crossfadeStartTime = Time.unscaledTime;
        crossfadeDuration = fade;
        fromVolume = GetSourceVolume(activeSource);
        toVolume = v;

        SetSourceVolume(inactiveSource, 0f);
        inactiveSource.Play();
        isCrossfading = true;

        OnTrackChanged?.Invoke(clip);
    }

    public void Stop(float fadeSeconds = 0f)
    {
        EnsureAudioSources();

        if (activeSource == null)
        {
            return;
        }

        if (fadeSeconds <= 0f)
        {
            activeSource.Stop();
            activeSource.clip = null;
            inactiveSource.Stop();
            inactiveSource.clip = null;
            isCrossfading = false;
            return;
        }

        // Fade out by crossfading to silence.
        inactiveSource.Stop();
        inactiveSource.clip = null;
        SetSourceVolume(inactiveSource, 0f);

        crossfadeStartTime = Time.unscaledTime;
        crossfadeDuration = fadeSeconds;
        fromVolume = GetSourceVolume(activeSource);
        toVolume = 0f;
        isCrossfading = true;
    }

    /// <summary>
    /// Sets the music volume to the specified level.
    /// </summary>
    /// <param name="newVolume">The desired volume level.</param>
    /// <param name="save">If true, the new volume setting is saved to storage.</param>
    public void SetVolume(float newVolume, bool save = true)
    {
        volume = Mathf.Clamp01(newVolume);

        if (save)
        {
            PlayerPrefs.SetFloat(MusicVolumeKey, volume);
            PlayerPrefs.Save();
        }

        ApplyVolumeImmediate();
    }

    /// <summary>
    /// Sets the muted state for music playback.
    /// </summary>
    /// <param name="muted">A value indicating whether music playback should be muted.</param>
    /// <param name="save">A value indicating whether the muted state should be persisted.</param>
    public void SetMuted(bool muted, bool save = true)
    {
        isMuted = muted;

        if (save)
        {
            PlayerPrefs.SetInt(MusicMutedKey, muted ? 1 : 0);
            PlayerPrefs.Save();
        }

        ApplyVolumeImmediate();
    }

    /// <summary>
    /// Toggles the muted state between muted and unmuted.
    /// </summary>
    /// <param name="save">Indicates whether the new muted state should be persisted.</param>
    public void ToggleMuted(bool save = true) => SetMuted(!isMuted, save);

    /// <summary>
    /// Pauses audio playback on both active and inactive audio sources.
    /// </summary>
    public void Pause()
    {
        EnsureAudioSources();
        activeSource?.Pause();
        inactiveSource?.Pause();
    }

    /// <summary>
    /// Resumes playback of any paused audio sources managed by this instance.
    /// </summary>
    public void Resume()
    {
        EnsureAudioSources();

        if (activeSource != null && activeSource.clip != null)
        {
            activeSource.UnPause();
        }

        if (inactiveSource != null && inactiveSource.clip != null)
        {
            inactiveSource.UnPause();
        }
    }

    /// <summary>
    /// Applies the current volume and mute settings to the active and inactive audio sources immediately.
    /// </summary>
    private void ApplyVolumeImmediate()
    {
        EnsureAudioSources();

        float v = isMuted ? 0f : volume;

        if (activeSource != null)
        {
            SetSourceVolume(activeSource, v);
        }

        if (inactiveSource != null)
        {
            // Keep inactive silent (except while crossfading)
            if (!isCrossfading)
            {
                SetSourceVolume(inactiveSource, 0f);
            }
        }
    }

    /// <summary>
    /// Initializes and configures the audio sources used for music.
    /// </summary>
    private void EnsureAudioSources()
    {
        if (musicSourceA == null)
        {
            musicSourceA = CreateChildSource("MusicSourceA");
        }

        if (musicSourceB == null)
        {
            musicSourceB = CreateChildSource("MusicSourceB");
        }

        ConfigureSourceDefaults(musicSourceA);
        ConfigureSourceDefaults(musicSourceB);
    }

    /// <summary>
    /// Initializes the stage music lookup dictionary by mapping valid stage identifiers to their corresponding audio
    /// clips.
    /// </summary>
    private void BuildStageMusicLookup()
    {
        stageMusicLookup = new Dictionary<int, AudioClip>();

        if (stageMusic == null)
        {
            return;
        }

        foreach (StageMusic entry in stageMusic)
        {
            if (entry.stageId <= 0 || entry.clip == null)
            {
                continue;
            }

            stageMusicLookup[entry.stageId] = entry.clip;
        }
    }

    /// <summary>
    /// Attempts to retrieve the audio clip associated with the specified stage identifier.
    /// </summary>
    /// <param name="stageId">The identifier of the stage for which to retrieve the audio clip.</param>
    /// <param name="clip">When this method returns, contains the audio clip associated with the specified stage if found.</param>
    /// <returns>True if an audio clip is found for the specified stage identifier; otherwise, false.</returns>
    private bool TryGetStageClip(int stageId, out AudioClip clip)
    {
        clip = null;

        if (stageId <= 0)
        {
            return false;
        }

        if (stageMusicLookup == null)
        {
            BuildStageMusicLookup();
        }

        return stageMusicLookup != null && stageMusicLookup.TryGetValue(stageId, out clip) && clip != null;
    }

    /// <summary>
    /// Subscribes to game events to enable handling of main menu and stage start actions.
    /// </summary>
    private void HookGameEvents()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null)
        {
            return;
        }

        gm.OnMainMenu += HandleMainMenu;

        gm.OnLocationSelected += HandleLocationSelected;

    }

    /// <summary>
    /// Detaches event handlers from game-related events to prevent further event processing.
    /// </summary>
    private void UnhookGameEvents()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null)
        {
            return;
        }

        gm.OnMainMenu -= HandleMainMenu;

        gm.OnLocationSelected -= HandleLocationSelected;

    }

    /// <summary>
    /// Handles the main menu event by start playback of the main menu music.
    /// </summary>
    private void HandleMainMenu()
    {
        PlayMainMenuMusic();
    }

    /// <summary>
    /// Handles the start of a stage by initiating playback of the corresponding stage music.
    /// </summary>
    /// <param name="stage">The stage model representing the stage that has started.</param>
    private void HandleLocationSelected(LocationModel _)
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || gm.CurrentSave == null || gm.CurrentSave.Stage == null)
        {
            return;
        }

        PlayStageMusic(gm.CurrentSave.Stage.StageID);
    }


    /// <summary>
    /// Creates a new child GameObject with the specified name and attaches an AudioSource component to it.
    /// </summary>
    /// <param name="name">The name to assign to the newly created child GameObject.</param>
    /// <returns>An AudioSource component attached to the newly created child GameObject.</returns>
    private AudioSource CreateChildSource(string name)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(transform, false);
        return go.AddComponent<AudioSource>();
    }

    /// <summary>
    /// Configures the specified audio source with default playback settings.
    /// </summary>
    /// <param name="source">The audio source to configure.</param>
    private void ConfigureSourceDefaults(AudioSource source)
    {
        if (source == null)
        {
            return;
        }

        source.playOnAwake = false;
        source.loop = true;
        source.spatialBlend = 0f;
    }

    /// <summary>
    /// Sets the volume of the specified audio source to a value between 0.0 and 1.0.
    /// </summary>
    /// <param name="source">The audio source whose volume will be set.</param>
    /// <param name="v">The desired volume level.</param>
    private static void SetSourceVolume(AudioSource source, float v)
    {
        if (source != null)
        {
            source.volume = Mathf.Clamp01(v);
        }
    }

    /// <summary>
    /// Retrieves the volume level from the specified audio source.
    /// </summary>
    /// <param name="source">The audio source from which to obtain the volume.</param>
    /// <returns>The volume of the provided audio source as a floating-point value.</returns>
    private static float GetSourceVolume(AudioSource source) => source != null ? source.volume : 0f;
}
