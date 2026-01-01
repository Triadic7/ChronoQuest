using UnityEngine;

public class PlayerGameplayUI : MonoBehaviour
{
    /// <summary>
    /// The ui thats shwon during gameplay, like time, score, and pause menu.
    /// </summary>
    private GameObject playerGameplayUI;

    /// <summary>
    /// Cache playerGameplayUI.
    /// </summary>
    private void Awake()
    {
        this.playerGameplayUI = this.transform.GetChild(0).gameObject;
    }

    /// <summary>
    /// Add event listeners to enable and disable ui.
    /// </summary>
    private void Start()
    {
        GameManager gm = GameManager.Instance;

        if (gm == null)
        {
            Debug.LogError("GameManager not found.");
            return;
        }

        // Show UI.
        gm.OnGameResumed += this.ShowUI;
        gm.OnStageStart += this.ShowUI;

        // Hide UI.
        gm.OnGamePaused += this.HideUI;
        gm.OnStageEnd += this.HideUI;

        this.HideUI();
    }

    /// <summary>
    /// Displays the ui.
    /// </summary>
    private void ShowUI()
    {
        this.playerGameplayUI.SetActive(true);
    }

    /// <summary>
    /// Hides the ui.
    /// </summary>
    private void HideUI()
    {
        this.playerGameplayUI?.SetActive(false);
    }
}
