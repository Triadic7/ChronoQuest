using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Save data for loading a save.
/// This is the runtime model.
/// </summary>
[System.Serializable]
public class SaveModel
{
    /// <summary>
    /// Save Id.
    /// </summary>
    public int SaveID;

    /// <summary>
    /// Gets the stage id.
    /// </summary>
    public int CurrentStageId;

    /// <summary>
    /// Gets the stage.
    /// </summary>
    public StageModel Stage;

    /// <summary>
    /// Playtime on save.
    /// </summary>
    public float PlayTime;

    /// <summary>
    /// If the game is coop or not.
    /// </summary>
    public bool IsCoop;

    /// <summary>
    /// The data last played.
    /// </summary>
    public DateTime LastPlayed { get; set; }

    /// <summary>
    /// The stage progress for each stage.
    /// </summary>
    public Dictionary<int, StageProgressModel> StageProgress;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="table">The sql table.</param>
    /// <param name="stage">The stage.</param>
    public SaveModel(SaveTable table, StageModel stage)
    {
        this.SaveID = table.Id;
        this.CurrentStageId = table.StageId;
        this.PlayTime = table.PlayTime;
        this.IsCoop = table.IsCoop;
        this.Stage = stage;
        this.LastPlayed = table.LastPlayed;
        this.StageProgress = new Dictionary<int, StageProgressModel>();
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="stageId">The stage id.</param>
    /// <param name="playtime">The time played.</param>
    /// <param name="isCoop">If game is coop.</param>
    public SaveModel(int stageId, float playtime, bool isCoop)
    {
        this.CurrentStageId = stageId;
        this.PlayTime = playtime;
        this.IsCoop = isCoop;

        this.StageProgress = new Dictionary<int, StageProgressModel>();
    }

    /// <summary>
    /// Adds stage result to dictionary.
    /// </summary>
    /// <param name="stageId">The stage id.</param>
    /// <param name="won">If the players won.</param>
    public void AddStageResult(int stageId, bool won)
    {
        // Tries to update dictionary first.
        if (!StageProgress.TryGetValue(stageId, out var progress))
        {
            progress = new StageProgressModel
            {
                StageID = stageId,
                Result = StageResult.NotPlayed
            };
            StageProgress[stageId] = progress;
        }

        // Update result.
        progress.Result = won ? StageResult.Won : StageResult.Lost;
    }
}
