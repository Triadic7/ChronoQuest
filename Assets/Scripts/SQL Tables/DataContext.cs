using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// Handles interactions with the db.
/// </summary>
public class DataContext
{
    /// <summary>
    /// The database.
    /// </summary>
    private SQLiteConnection db;

    /// <summary>
    /// Constructor for sql.
    /// </summary>
    /// <param name="databaseName">The db file path.</param>
    public DataContext(string databaseName = "chronoquest.db")
    {
        string dbPath = Path.Combine(Application.persistentDataPath, databaseName);
        this.db = new SQLiteConnection(dbPath);

        // Create save table.
        this.db.CreateTable<SaveTable>();

        // Create stage table.
        this.db.CreateTable<StageTable>();

        // Seed stages. [Populate this with a JSON file]
        if (!this.db.Table<StageTable>().Any())
        {
            this.db.Insert(new StageTable { StageId = 1, StageName = "Egypt", ObjectiveText = "To save Egypt, you must match the given pattern sequence to maintain balance. Failure is not an option." });
            this.db.Insert(new StageTable { StageId = 2, StageName = "Medieval Europe", ObjectiveText = "The timeline is starting to decay. Hurry up and grab the fresh moments in time before they disappear and summon anomalies." });
            this.db.Insert(new StageTable { StageId = 3, StageName = "Modern Day", ObjectiveText = "Protect the base at all costs! You're given a new suit that wards away evil forces. Simply coming into contact with anomalies will remove them from the timeline." });
            this.db.Insert(new StageTable { StageId = 4, StageName = "Space Age", ObjectiveText = "Final boss time. Protect the space station and destroy the interlopers once and for all!", IsFinalStage = true });
            Debug.Log($"Stages in DB: {this.db.Table<StageTable>().Count()}");
        }

        // Create npc table.
        this.db.CreateTable<NPCTable>();

        // Seed NPCs. [Populate this with a JSON file]
        if (!this.db.Table<NPCTable>().Any())
        {
            this.db.Insert(new NPCTable { NPCId = 1, Name = "Pharoh" });
            this.db.Insert(new NPCTable { NPCId = 2, Name = "King Arthur" });
            this.db.Insert(new NPCTable { NPCId = 3, Name = "Superior" });
            this.db.Insert(new NPCTable { NPCId = 4, Name = "Super Advanced AI" });
        }

        // Create dialogue table.
        this.db.CreateTable<NPCDialogueTable>();

        #region Dialogue Seeding
        // Seed dialogue table. [Populate this with a JSON file]
        if (!this.db.Table<NPCDialogueTable>().Any())
        {
            // Pharoh.
            this.db.Insert(new NPCDialogueTable
            {
                NPCId = 1,
                Order = 0,
                Text = "Test 1"
            });

            this.db.Insert(new NPCDialogueTable
            {
                NPCId = 1,
                Order = 1,
                Text = "Test 2"
            });

            // King Arthur.
            this.db.Insert(new NPCDialogueTable
            {
                NPCId = 2,
                Order = 0,
                Text = "Test 3"
            });

            this.db.Insert(new NPCDialogueTable
            {
                NPCId = 2,
                Order = 1,
                Text = "Test 4"
            });

            // Superior.
            this.db.Insert(new NPCDialogueTable
            {
                NPCId = 3,
                Order = 0,
                Text = "Test 5"
            });

            this.db.Insert(new NPCDialogueTable
            {
                NPCId = 3,
                Order = 1,
                Text = "Test 6"
            });

            // Advanced AI.
            this.db.Insert(new NPCDialogueTable
            {
                NPCId = 4,
                Order = 0,
                Text = "Test 7"
            });

            this.db.Insert(new NPCDialogueTable
            {
                NPCId = 4,
                Order = 1,
                Text = "Test 8"
            });
        }
        #endregion
    }

    /// <summary>
    /// Returns a list of saves.
    /// </summary>
    /// <returns>Returns a list of all saves.</returns>
    public List<SaveModel> GetAllSaves()
    {
        // The max stage id.
        int maxStageId = this.db.Table<StageTable>().Max(s => s.StageId);

        // Returns a list of SaveModels with their stage clamped to avoid corrupt saves.
        return this.db.Table<SaveTable>()
            .Select(s =>
            {
                int safeStageId = s.StageId;

                // Clamp invalid stages to final stage.
                if (safeStageId > maxStageId || safeStageId <= 0)
                {
                    Debug.LogWarning($"Invalid StageId {s.StageId} detected in save {s.Id}. Clamping to {maxStageId}.");

                    safeStageId = maxStageId;

                    // Fix incorrect stage id..
                    s.StageId = safeStageId;
                    this.db.Update(s);
                }

                return new SaveModel(s, this.GetStageById(safeStageId));
            })
            .ToList();
    }

    /// <summary>
    /// Inserts a new save.
    /// </summary>
    /// <param name="data">The data to save.</param>
    /// <returns>Returns id of new save.</returns>
    public int InsertSave(SaveModel data)
    {
        SaveTable model = new SaveTable
        {
            PlayTime = data.PlayTime,
            IsCoop = data.IsCoop,

            // Use stage id or default 1.
            StageId = data.Stage != null ? data.Stage.StageId : 1
        };

        // Insert new data into db.
        this.db.Insert(model);
        return model.Id;
    }

    /// <summary>
    /// Updates an existing save.
    /// </summary>
    /// <param name="data">The data to update.</param>
    public void UpdateSave(SaveModel data)
    {
        SaveTable model = new SaveTable
        {
            Id = data.Id,
            StageId = data.StageId,
            PlayTime = data.PlayTime,
            IsCoop = data.IsCoop,
        };

        // Updates data.
        this.db.Update(model);
    }

    /// <summary>
    /// Gets a stage by id.
    /// </summary>
    /// <param name="id">The stages id.</param>
    /// <returns>Returns the stage id.</returns>
    public StageModel GetStageById(int id)
    {
        // Get table from db.
        var tableRow = this.db.Table<StageTable>().FirstOrDefault(s => s.StageId == id);

        if (tableRow == null)
        {
            Debug.LogWarning($"Stage not found for StageId: {id}");
        }
        else
        {
            Debug.Log($"Loaded Stage: {tableRow.StageName} for StageId: {id}");
        }

        // If table isn't null, return new stage model.
        return tableRow != null ? new StageModel(tableRow) : null;
    }

    /// <summary>
    /// Gets a save by id.
    /// </summary>
    /// <param name="id">The save id.</param>
    /// <returns>Returns save data from save id.</returns>
    public SaveModel GetSaveById(int id)
    {
        // Get table from db.
        var tableRow = this.db.Table<SaveTable>().FirstOrDefault(s => s.Id == id);

        // If table isn't null, return new save model.
        return tableRow != null ? new SaveModel(tableRow, this.GetStageById(tableRow.StageId)) : null;
    }

    /// <summary>
    /// Deletes a save by id from the database.
    /// </summary>
    /// <param name="saveId">The save ID.</param>
    public void DeleteSave(int saveId)
    {
        var saveRow = this.db.Table<SaveTable>().FirstOrDefault(s => s.Id == saveId);
        if (saveRow != null)
        {
            this.db.Delete(saveRow);
            Debug.Log($"Deleted save with ID {saveId}");
        }
        else
        {
            Debug.LogWarning($"No save found with ID {saveId} to delete");
        }
    }

    /// <summary>
    /// Gets all dialogue from npc by id.
    /// </summary>
    /// <param name="npcId">The id of the npc.</param>
    /// <returns>Returns list of npc dialogue.</returns>
    public List<NPCDialogueModel> GetDialoguesForNPC(int npcId)
    {
        return this.db.Table<NPCDialogueTable>()
            .Where(d => d.NPCId == npcId)
            .OrderBy(d => d.Order)
            .Select(d => new NPCDialogueModel(d))
            .ToList();
    }

    /// <summary>
    /// Gets npc from stage id.
    /// </summary>
    /// <param name="stageId">The stage id.</param>
    /// <returns>Returns npc model based ons tage id.</returns>
    public NPCModel GetNPCFromStage(int stageId)
    {
        // Get table from db.
        var tableRow = this.db.Table<NPCTable>().FirstOrDefault(n => n.NPCId == stageId);

        // If table isn't null, return new save model.
        return tableRow != null ? new NPCModel(tableRow, this.GetDialoguesForNPC(tableRow.NPCId)) : null;
    }

}
