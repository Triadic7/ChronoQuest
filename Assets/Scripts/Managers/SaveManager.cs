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
    /// Saves progress by either updating or inserting save data.
    /// </summary>
    /// <param name="save">The save data to insert or update.</param>
    public SaveModel SaveProgress(SaveModel save)
    {
        if (save.Id == 0)
        {
            save.Id = DataContext.InsertSave(save);
        }
        else
        {
            DataContext.UpdateSave(save);
        }

        // Make sure the Stage object is set using StageId.
        save.Stage = DataContext.GetStageById(save.StageId);

        return save;
    }

    /// <summary>
    /// Load save data.
    /// </summary>
    private void Awake()
    {
        // Load save from sql handler.
        this.DataContext = new DataContext();
        this.PlayerSaves = DataContext.GetAllSaves();
    }


}
