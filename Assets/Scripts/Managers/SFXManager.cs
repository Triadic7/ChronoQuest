using UnityEngine;

/// <summary>
/// The sound effects manager.
/// </summary>
public class SFXManager : MonoBehaviour
{
    /// <summary>
    /// The key to player prefs;
    /// </summary>
    private const string PlayerPrefsKey = "SFXVolume";

    /// <summary>
    /// The volume
    /// </summary>
    public float Volume { get; private set; } = 1f;

    /// <summary>
    /// Plays audio clip.
    /// </summary>
    /// <param name="clip">The clip to play.</param>
    public void PlaySound(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        AudioSource.PlayClipAtPoint(clip, Vector3.zero, this.Volume);
    }

    /// <summary>
    /// Sets the volume and saves to player prefs.
    /// </summary>
    /// <param name="volume">The volume.</param>
    public void SetVolume(float volume)
    {
        this.Volume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(PlayerPrefsKey, this.Volume);
        PlayerPrefs.Save();

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetVolume(this.Volume, save: false);
        }
    }

    private void Awake()
    {
        // Load saved volume or default to 1f.
        this.Volume = PlayerPrefs.GetFloat(PlayerPrefsKey, 1f);

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetVolume(this.Volume, save: false);
        }
    }
}
