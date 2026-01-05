using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGameplayUI : MonoBehaviour
{
    /// <summary>
    /// Event that fires at the end that returns the total time.
    /// </summary>
    public event Action<float> OnStageTimeElapsed;

    /// <summary>
    /// The ui that shows during gameplay, like time, score, and pause menu.
    /// </summary>
    private GameObject playerGameplayUI;

    /// <summary>
    /// Timer.
    /// </summary>
    private float gameplayTime;

    /// <summary>
    /// Timer text.
    /// </summary>
    [SerializeField]
    private TMP_Text timeText;

    /// <summary>
    /// Objective text.
    /// </summary>
    [SerializeField]
    private TMP_Text objectiveText;

    /// <summary>
    /// The slider showing current progress.
    /// </summary>
    [SerializeField]
    private Slider progressSlider;

    /// <summary>
    /// Checks if timing.
    /// </summary>
    private bool isTiming;

    /// <summary>
    /// The array of failure boxes.
    /// </summary>
    [SerializeField]
    private TMP_Text[] failureBoxes;

    /// <summary>
    /// What failure the players are on.
    /// </summary>
    private int failureIndex;

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

        // Reset timer on new stage.
        gm.OnStageStart += () =>
        {
            this.gameplayTime = 0f;
        };

        GameStageManager stageManager = gm.GameStageManager;

        // Add failure event.
        stageManager.OnGameFailStrike += this.AddFailStrike;

        // Add success event.
        stageManager.OnGameProgressMade += this.AddProgress;

        // Hide UI.
        gm.OnGamePaused += this.HideUI;
        gm.OnStageEnd += this.HideUI;
        gm.OnStageEnd += this.StopTimerAndReport;

        // Cache objective for text.
        gm.GameStageManager.OnStageStarted += (stage) =>
        {
            this.objectiveText.text = stage.ObjectiveText;
            this.ResetStrikes();

            // Initialize progress bar.
            progressSlider.minValue = 0;
            progressSlider.maxValue = stage.ObjectiveProgress;
            progressSlider.value = 0;
        };

        this.HideUI();
    }

    /// <summary>
    /// Updates timer while game is playing.
    /// </summary>
    private void Update()
    {
        if (this.isTiming)
        {
            this.gameplayTime += Time.deltaTime;
            if (this.timeText != null)
            {
                this.timeText.text = this.FormatTime(gameplayTime);
            }
        }
    }

    /// <summary>
    /// Displays the ui.
    /// </summary>
    private void ShowUI()
    {
        this.isTiming = true;
        this.playerGameplayUI.SetActive(true);
        this.objectiveText.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the ui.
    /// </summary>
    private void HideUI()
    {
        this.isTiming = false;
        this.playerGameplayUI?.SetActive(false);
        this.objectiveText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Formats the time into something readable.
    /// </summary>
    /// <param name="time">The time.</param>
    /// <returns>Returns pretty string of the time.</returns>
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"{minutes:00}:{seconds:00}";
    }

    /// <summary>
    /// Call this when the stage ends to send the total time.
    /// </summary>
    private void StopTimerAndReport()
    {
        this.isTiming = false;

        // Fire the event to let GameManager know how much time was played.
        this.OnStageTimeElapsed?.Invoke(gameplayTime);

        // Reset timer for next stage.
        this.gameplayTime = 0f;

        // Reset failure boxes for next stage.
        this.ResetStrikes();
    }

    /// <summary>
    /// Adds progress to the progress bar.
    /// </summary>
    /// <param name="progress">The progress made.</param>
    /// <param name="winAmount">How much to win.</param>
    private void AddProgress(int progress, int winAmount)
    {
        if (this.progressSlider == null)
        {
            return;
        }

        // Set max value on slider.
        this.progressSlider.maxValue = winAmount;

        this.progressSlider.value = progress;
    }

    /// <summary>
    /// Resets strikes.
    /// </summary>
    private void ResetStrikes()
    {
        this.failureIndex = 0;

        foreach (var box in this.failureBoxes)
        {
            box.enabled = false;
        }
    }

    /// <summary>
    /// Adds a fail strike to ui.
    /// </summary>
    private void AddFailStrike()
    {
        if (this.failureIndex >= this.failureBoxes.Length)
        {
            return;
        }

        this.failureBoxes[failureIndex].enabled = true;
        this.failureIndex++;
    }
}
