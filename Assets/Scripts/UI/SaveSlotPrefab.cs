using TMPro;
using UnityEngine;

/// <summary>
/// Displays a save slot on a prefab.
/// </summary>
public class SaveSlotPrefab : MonoBehaviour
{
    /// <summary>
    /// The location text.
    /// </summary>
    [SerializeField]
    private TMP_Text locationText;

    /// <summary>
    /// The stage id text.
    /// </summary>
    [SerializeField]
    private TMP_Text stageIdText;

    /// <summary>
    /// The time text.
    /// </summary>
    [SerializeField]
    private TMP_Text timeText;

    /// <summary>
    /// Displays save data if present, otherwise displays new game.
    /// </summary>
    /// <param name="save"></param>
    public void DisplaySave(SaveModel save = null)
    {
        if(save != null)
        {
            this.locationText.text = save.Stage.StageName;
            this.stageIdText.text = $"Level: {save.Stage.StageID}";
            this.timeText.text = $"Time: {this.FormatTime(save.PlayTime)}";
        }
        else
        {
            this.locationText.text = "New Save";
            this.stageIdText.text = string.Empty;
            this.timeText.text = string.Empty;
        }
    }

    /// <summary>
    /// Displays default save stuff.
    /// </summary>
    private void Awake()
    {
        this.DisplaySave();
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
}
