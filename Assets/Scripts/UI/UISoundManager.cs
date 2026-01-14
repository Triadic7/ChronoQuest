using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISoundManager : MonoBehaviour
{

    /// <summary>
    /// Gets the singleton instance of the UISoundManager.
    /// </summary>
    public static UISoundManager Instance { get; private set; }

    /// <summary>
    /// The audio clip that is played when a UI element is clicked.
    /// </summary>
    [Header("UI clips")]
    [SerializeField]
    private AudioClip click;

    /// <summary>
    /// Gets or sets a value indicating whether selectable events are automatically hooked.
    /// </summary>
    [Header("Behavior")]
    [SerializeField]
    private bool autoHookSelectableEvents = true;

    /// <summary>
    /// Gets or sets a value indicating whether inactive objects are included in the operation.
    /// </summary>
    [SerializeField]
    private bool includeInactive = true;

    /// <summary>
    /// Gets or sets the scale factor applied to the audio volume.
    /// </summary>
    [SerializeField]
    [Min(0f)]
    private float volumeScale = 1f;

    /// <summary>
    /// Contains the set of instance identifiers that are currently hooked.
    /// </summary>
    private readonly HashSet<int> hookedInstanceIds = new HashSet<int>();

    /// <summary>
    /// The sfx manager.
    /// </summary>
    private SFXManager sfxManager;

    /// <summary>
    /// Initializes the singleton instance of the component and ensures it persists across scene loads.
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

        // Cache sfx manager.
        this.sfxManager = GameManager.Instance.SFXManager;
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
    /// Handles initialization logic when the component is enabled.
    /// </summary>
    private void OnEnable()
    {
        if (autoHookSelectableEvents)
        {
            HookSceneUI();
        }
    }

    /// <summary>
    /// Attaches event listeners to all UI Selectable components in the current scene to enable automatic handling of
    /// user interactions for playing UI sound effects.
    /// </summary>
    private void HookSceneUI()
    {
        if (!autoHookSelectableEvents)
        {
            return;
        }

        Selectable[] selectables = FindObjectsByType<Selectable>(includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (Selectable s in selectables)
        {
            if (s == null)
            {
                continue;
            }

            int id = s.GetInstanceID();
            if (!hookedInstanceIds.Add(id))
            {
                continue;
            }

            Button b = s as Button;
            if (b != null)
            {
                b.onClick.AddListener(PlayClick);
                continue;
            }
        }
    }

    /// <summary>
    /// Plays the standard UI click sound effect.
    /// </summary>
    private void PlayClick()
    {
        if (sfxManager != null && click != null)
        {
            sfxManager.PlaySound(click);
        }
    }
}
