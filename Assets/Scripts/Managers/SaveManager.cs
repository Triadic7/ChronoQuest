using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    /// <summary>
    /// The saves the player has.
    /// </summary>
    public List<SaveModel> PlayerSaves { get; private set; }

    /// <summary>
    /// DB handler.
    /// </summary>
    public DataContext DataContext { get; private set; }

    /// <summary>
    /// Syncs data from db.
    /// </summary>
    public void SyncData()
    {
        this.PlayerSaves = this.DataContext.GetSaves();
    }

    /// <summary>
    /// Saves progress by either updating or inserting save data.
    /// </summary>
    /// <param name="save">The save data to insert or update.</param>
    public SaveModel SaveProgress(SaveModel save)
    {
        // Store last played.
        save.LastPlayed = DateTime.UtcNow;

        if (save.Id == 0)
        {
            save.Id = this.DataContext.InsertSave(save);
            this.PlayerSaves.Add(save);
        }
        else
        {
            this.DataContext.UpdateSave(save);
        }

        // Saves stage progress here by inserting or updating.
        foreach (var progress in save.StageProgress.Values)
        {
            if (DataContext.StageProgressExists(save.Id, progress.StageID))
            {
                DataContext.UpdateStageProgress(save.Id, progress);
            }
            else
            {
                DataContext.InsertStageProgress(save.Id, progress);
            }
        }

        // Make sure the Stage object is set using StageId.
        save.Stage = this.DataContext.GetStageById(save.CurrentStageId);

        return save;
    }

    /// <summary>
    /// Deletes a save.
    /// </summary>
    /// <param name="save">The save to delete.</param>
    public void DeleteSave(SaveModel save)
    {
        if (save == null)
        {
            Debug.LogError("No save provided to delete.");
            return;
        }

        // Delete from DB.
        this.DataContext.DeleteSave(save.Id);

        // Refresh saves list from DB.
        this.PlayerSaves = this.DataContext.GetSaves();
    }

    /// <summary>
    /// Load save data.
    /// </summary>
    private void Awake()
    {
        // Load save from sql handler.
        this.DataContext = new DataContext();
        this.PlayerSaves = this.DataContext.GetSaves();
    }


}
