using SQLite;
using System;
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
        Debug.Log(Application.persistentDataPath);

        // Create save table.
        this.db.CreateTable<SaveTable>();

        // Create stage table.
        this.db.CreateTable<StageTable>();

        // Seed stages. [Populate this with a JSON file?]
        this.db.InsertOrReplace(new StageTable 
        { 
            StageId = 1, 
            StageName = "Egypt", 
            StageDescription = "To save Egypt, you must match the given pattern sequence to maintain balance. Failure is not an option.",
            ObjectiveText = "Match the pattern"
        });
        this.db.InsertOrReplace(new StageTable 
        { 
            StageId = 2, 
            StageName = "Medieval Europe", 
            StageDescription = "Protect the base at all costs! You're given a new suit that wards away evil forces. Simply coming into contact with anomalies will remove them from the timeline.",
            ObjectiveText = "Touch the anomalies to banish them"
        });
        this.db.InsertOrReplace(new StageTable 
        { 
            StageId = 3, 
            StageName = "Modern Day", 
            StageDescription = "Hurry up and grab the fresh moments in time before they disappear and summon anomalies.",
            ObjectiveText = "Grab the disturbances before they fully appear"
        });
        this.db.InsertOrReplace(new StageTable 
        { 
            StageId = 4, 
            StageName = "Space Age", 
            StageDescription = "Final boss time. Protect the space station and destroy the interlopers once and for all!",
            ObjectiveText = "Defeat the boss and protect the station",
            ObjectiveProgress = 20,
            IsFinalStage = true 
        });
        Debug.Log($"Stages in DB: {this.db.Table<StageTable>().Count()}");

        // Create npc table.
        this.db.CreateTable<NPCTable>();

        // Seed NPCs. [Populate this with a JSON file?]
        if (!this.db.Table<NPCTable>().Any())
        {
            this.db.Insert(new NPCTable { NPCId = 1, Name = "Pharoh" });
            this.db.Insert(new NPCTable { NPCId = 2, Name = "King Arthur" });
            this.db.Insert(new NPCTable { NPCId = 3, Name = "Superior" });
            this.db.Insert(new NPCTable { NPCId = 4, Name = "Super Advanced AI" });
        }

        // Create tables for dialogue if not present.
        this.db.CreateTable<DialogueTable>();
        this.db.CreateTable<DialogueNodeTable>();
        this.db.CreateTable<DialogueChoiceTable>();

        // Always reset dialogue data if data is present to prevent caching old conversation data.
        db.DeleteAll<DialogueChoiceTable>();
        db.DeleteAll<DialogueNodeTable>();
        db.DeleteAll<DialogueTable>();

        // Inserts dialogue into the dialogue table for npcs to use.
        if (!this.db.Table<DialogueTable>().Any())
        {
            #region Pharoh Dialogue
            // Pharoh's dialogue.
            DialogueTable dialogue = new DialogueTable
            {
                NPCID = 1,
                NPCName = "Pharoh"
            };
            db.Insert(dialogue);
            int dialogueId = dialogue.DialogueID;

            // Nodes.
            var mainNode = new DialogueNodeTable
            {
                DialogueID = dialogueId,
                Order = 0,
                DialogueText = "Well? Are you gonna do it?",
                IsEndNode = false
            };
            this.db.Insert(mainNode);
            int mainNodeId = mainNode.NodeID;

            var savePeopleNode = new DialogueNodeTable
            {
                DialogueID = dialogueId,
                Order = 1,
                DialogueText = "You're here to save my people and I, are you not?",
                IsEndNode = false
            };
            this.db.Insert(savePeopleNode);
            int savePeopleNodeId = savePeopleNode.NodeID;

            var ritualNode = new DialogueNodeTable
            {
                DialogueID = dialogueId,
                Order = 2,
                DialogueText = "I know not the year from which you come from, but I know of your purpose. You are the only ones who can stop them. I will start a ritual using a series of colors. Match the colors in the order I summon them, to activate the <link=pyramid><color=#00BFFF>Pyramid's</color></link> secret weapon and stop the <link=anomalies><color=#00BFFF>Anomalies</color></link>.",
                IsEndNode = false
            };
            this.db.Insert(ritualNode);
            int ritualNodeId = ritualNode.NodeID;

            // Choices.
            this.db.Insert(new DialogueChoiceTable
            {
                NodeID = mainNodeId,
                ChoiceText = "<color=#FFA500>Let's do it.</color>",
                // Triggers game action.
                NextNodeID = -1
            });

            this.db.Insert(new DialogueChoiceTable
            {
                NodeID = mainNodeId,
                ChoiceText = "Do what again?",
                NextNodeID = savePeopleNodeId
            });

            this.db.Insert(new DialogueChoiceTable
            {
                NodeID = savePeopleNodeId,
                ChoiceText = "I don't know what I'm doing here",
                NextNodeID = ritualNodeId
            });

            this.db.Insert(new DialogueChoiceTable
            {
                NodeID = savePeopleNodeId,
                ChoiceText = "Yes, I'm here to stop the <link=anomalies><color=#00BFFF>Anomalies</color></link>. They're breaching the timeline more than ever before.",
                NextNodeID = mainNodeId
            });

            // Choice loops back to main.
            this.db.Insert(new DialogueChoiceTable
            {
                NodeID = ritualNodeId,
                ChoiceText = "Okay, I think I can handle it from here.",
                NextNodeID = mainNodeId
            });

            #endregion

            #region King Arthur Dialogue
            // King Arthur's dialogue.
            DialogueTable arthurDialogue = new DialogueTable
            {
                NPCID = 2,
                NPCName = "King Arthur"
            };
            db.Insert(arthurDialogue);
            int arthurDialogueId = arthurDialogue.DialogueID;

            // Nodes.
            var mainNodeArthur = new DialogueNodeTable
            {
                DialogueID = arthurDialogueId,
                Order = 0,
                DialogueText = "The <link=anomalies><color=#00BFFF>Anomalies</color></link> are attacking the <link=castle><color=#00BFFF>Castle</color></link>! We await your help, hurry!",
                IsEndNode = false
            };
            db.Insert(mainNodeArthur);
            int mainNodeArthurId = mainNodeArthur.NodeID;

            var explainNodeArthur = new DialogueNodeTable
            {
                DialogueID = arthurDialogueId,
                Order = 1,
                DialogueText = "You must defend the <link=castle><color=#00BFFF>Castle</color></link> walls. Hold out long enough for me to decipher the ancient code beneath the castle. We can use it to close the <link=void><color=#00BFFF>Void</color></link>. Fail, and we will never be able to close the rift.",
                IsEndNode = false
            };
            db.Insert(explainNodeArthur);
            int explainNodeArthurId = explainNodeArthur.NodeID;

            var strategyNodeArthur = new DialogueNodeTable
            {
                DialogueID = arthurDialogueId,
                Order = 2,
                DialogueText = "I've heard in this timeline, travelers such as yourself are able to pierce their armor. Simply touching them sends them back to the <link=void><color=#00BFFF>Void</color></link>.",
                IsEndNode = false
            };
            db.Insert(strategyNodeArthur);
            int strategyNodeArthurId = strategyNodeArthur.NodeID;

            var loreNodeArthur = new DialogueNodeTable
            {
                DialogueID = arthurDialogueId,
                Order = 3,
                DialogueText = "These anomalies are fragments of corrupted timelines. Failed experiments by powerful time mages. They seek to unravel catastrophe unless stopped.",
                IsEndNode = false
            };
            db.Insert(loreNodeArthur);
            int loreNodeArthurId = loreNodeArthur.NodeID;

            // Choices.
            db.Insert(new DialogueChoiceTable
            {
                NodeID = mainNodeArthurId,
                ChoiceText = "<color=#FFA500>I will defend the <link=castle><color=#00BFFF>Castle</color></link>!</color>",
                // Starts game.
                NextNodeID = -1
            });

            db.Insert(new DialogueChoiceTable
            {
                NodeID = mainNodeArthurId,
                ChoiceText = "What is happening here?",
                NextNodeID = explainNodeArthurId
            });

            db.Insert(new DialogueChoiceTable
            {
                NodeID = explainNodeArthurId,
                ChoiceText = "Understood, I will hold the walls!",
                // Loops back to main node for start action.
                NextNodeID = mainNodeArthurId
            });

            db.Insert(new DialogueChoiceTable
            {
                NodeID = explainNodeArthurId,
                ChoiceText = "Uh, how am I supposed to defend a whole <link=castle><color=#00BFFF>Castle</color></link>? Shouldn't you give me reinforcements?",
                NextNodeID = strategyNodeArthurId
            });

            db.Insert(new DialogueChoiceTable
            {
                NodeID = strategyNodeArthurId,
                ChoiceText = "I understand. Let's defend!",
                // Loops back to main node for start action.
                NextNodeID = mainNodeArthurId
            });

            db.Insert(new DialogueChoiceTable
            {
                NodeID = strategyNodeArthurId,
                ChoiceText = "Tell me more about these anomalies and why they exist.",
                NextNodeID = loreNodeArthurId
            });

            // Loops back to main node for start action.
            db.Insert(new DialogueChoiceTable
            {
                NodeID = loreNodeArthurId,
                ChoiceText = "I see. Let's defend the castle!",
                NextNodeID = mainNodeArthurId
            });
            #endregion

            #region Modern Day Supervisor Dialogue
            // Supervisor's dialogue.
            DialogueTable supervisorDialogue = new DialogueTable
            {
                NPCID = 3,
                NPCName = "Supervisor"
            };
            db.Insert(supervisorDialogue);
            int supervisorDialogueId = supervisorDialogue.DialogueID;

            // Nodes.
            var mainNodeSupervisor = new DialogueNodeTable
            {
                DialogueID = supervisorDialogueId,
                Order = 0,
                DialogueText = "<link=disturbance><color=#00BFFF>Disturbances</color></link> are appearing around the city! You need to remove them before they fully materialize. Otherwise who knows what crazy tech random people will find. The last thing we need is another <link=incident><color=#00BFFF>incident</color></link>.",
                IsEndNode = false
            };
            db.Insert(mainNodeSupervisor);
            int mainNodeSupervisorId = mainNodeSupervisor.NodeID;

            var explainNodeSupervisor = new DialogueNodeTable
            {
                DialogueID = supervisorDialogueId,
                Order = 1,
                DialogueText = "These <link=anomalies><color=#00BFFF>Anomalies</color></link> spawn over time and can overwhelm you if ignored. Remove disturbances before they fully appear, society isn't ready for such technology.",
                IsEndNode = false
            };
            db.Insert(explainNodeSupervisor);
            int explainNodeSupervisorId = explainNodeSupervisor.NodeID;

            var strategyNodeSupervisor = new DialogueNodeTable
            {
                DialogueID = supervisorDialogueId,
                Order = 2,
                DialogueText = "Each <link=disturbance><color=#00BFFF>Disturbance</color></link> are equally dangerous to leave unchecked. They will have to run out of <link=disturbance><color=#00BFFF>Disturbances</color></link> eventully, so just keep grabbing them until they stop coming.",
                IsEndNode = false
            };
            db.Insert(strategyNodeSupervisor);
            int strategyNodeSupervisorId = strategyNodeSupervisor.NodeID;

            var loreNodeSupervisor = new DialogueNodeTable
            {
                DialogueID = supervisorDialogueId,
                Order = 3,
                DialogueText = "That's above your <link=pay><color=#00BFFF>paygrade</color></link>. How about we focus on the objective at hand? You ask too many questions.",
                IsEndNode = false
            };
            db.Insert(loreNodeSupervisor);
            int loreNodeSupervisorId = loreNodeSupervisor.NodeID;

            // Choices.
            db.Insert(new DialogueChoiceTable
            {
                NodeID = mainNodeSupervisorId,
                ChoiceText = "<color=#FFA500>I will do my job, just point me at them.</color>",
                // Triggers gameplay start.
                NextNodeID = -1
            });

            db.Insert(new DialogueChoiceTable
            {
                NodeID = mainNodeSupervisorId,
                ChoiceText = "What can I do?",
                NextNodeID = explainNodeSupervisorId
            });

            db.Insert(new DialogueChoiceTable
            {
                NodeID = explainNodeSupervisorId,
                ChoiceText = "I will prioritize the <link=disturbance><color=#00BFFF>Disturbance</color></link> above my own life.",
                NextNodeID = mainNodeSupervisorId
            });

            db.Insert(new DialogueChoiceTable
            {
                NodeID = explainNodeSupervisorId,
                ChoiceText = "Wait, how do these anomalies spawn?",
                NextNodeID = strategyNodeSupervisorId
            });

            db.Insert(new DialogueChoiceTable
            {
                NodeID = strategyNodeSupervisorId,
                ChoiceText = "Got it.",
                NextNodeID = mainNodeSupervisorId
            });

            db.Insert(new DialogueChoiceTable
            {
                NodeID = strategyNodeSupervisorId,
                ChoiceText = "Tell me more about these anomalies.",
                NextNodeID = loreNodeSupervisorId
            });

            db.Insert(new DialogueChoiceTable
            {
                NodeID = loreNodeSupervisorId,
                ChoiceText = "Understood.",
                NextNodeID = mainNodeSupervisorId
            });
            #endregion

            #region Super Intelligent AI Dialogue
            // Super Intelligent AI dialogue.
            DialogueTable aiDialogue = new DialogueTable
            {
                NPCID = 4,
                NPCName = "Super Intelligent AI"
            };
            db.Insert(aiDialogue);
            int aiDialogueId = aiDialogue.DialogueID;

            // Nodes.
            var mainNodeAI = new DialogueNodeTable
            {
                DialogueID = aiDialogueId,
                Order = 0,
                DialogueText = "Alert! A massive <link=interloper><color=#00BFFF>Interloper</color></link> has materialized from the <link=void><color=#00BFFF>Void</color></link> near orbital station coordinates. Engagement protocol FXD-938 is in effect.",
                IsEndNode = false
            };
            db.Insert(mainNodeAI);
            int mainNodeAIId = mainNodeAI.NodeID;

            var explainNodeAI = new DialogueNodeTable
            {
                DialogueID = aiDialogueId,
                Order = 1,
                DialogueText = "The <link=interloper><color=#00BFFF>Interloper</color></link> exhibits unstable quantum behavior. If not dispatched quickly, it will induce secondary <link=anomalies><color=#00BFFF>Anomalies</color></link> via recursive spacetime perturbations, exceeding station defenses.",
                IsEndNode = false
            };
            db.Insert(explainNodeAI);
            int explainNodeAIId = explainNodeAI.NodeID;

            var strategyNodeAI = new DialogueNodeTable
            {
                DialogueID = aiDialogueId,
                Order = 2,
                DialogueText = "Directive: prioritize engagement with the <link=interloper><color=#00BFFF>Interloper</color></link> using all available projectile and energy arrays until destabilization metrics reach threshold.",
                IsEndNode = false
            };
            db.Insert(strategyNodeAI);
            int strategyNodeAIId = strategyNodeAI.NodeID;

            var loreNodeAI = new DialogueNodeTable
            {
                DialogueID = aiDialogueId,
                Order = 3,
                DialogueText = "For context: this type of <link=interloper><color=#00BFFF>Interloper</color></link> has appeared only twice in recorded history. Its energy signatures are highly unusual, and despite years of research, little is known.",
                IsEndNode = false
            };
            db.Insert(loreNodeAI);
            int loreNodeAIId = loreNodeAI.NodeID;

            // Choices.
            db.Insert(new DialogueChoiceTable
            {
                NodeID = mainNodeAIId,
                ChoiceText = "<color=#FFA500>Engaging the <link=interloper><color=#00BFFF>Interloper</color></link> now.</color>",
                NextNodeID = -1
            });

            db.Insert(new DialogueChoiceTable
            {
                NodeID = mainNodeAIId,
                ChoiceText = "Wait… I need more info before engaging. You're smart, tell me something.",
                NextNodeID = explainNodeAIId
            });

            db.Insert(new DialogueChoiceTable
            {
                NodeID = explainNodeAIId,
                ChoiceText = "Tell me what this Interloper is. What am I actually fighting?",
                NextNodeID = loreNodeAIId
            });

            db.Insert(new DialogueChoiceTable
            {
                NodeID = explainNodeAIId,
                ChoiceText = "Okay… so what's my strategy?",
                NextNodeID = strategyNodeAIId
            });

            db.Insert(new DialogueChoiceTable
            {
                NodeID = strategyNodeAIId,
                ChoiceText = "Okay, I think I'm ready to fire at will.",
                NextNodeID = mainNodeAIId
            });

            db.Insert(new DialogueChoiceTable
            {
                NodeID = strategyNodeAIId,
                ChoiceText = "Wait… what’s the history of this thing?",
                NextNodeID = loreNodeAIId
            });

            db.Insert(new DialogueChoiceTable
            {
                NodeID = loreNodeAIId,
                ChoiceText = "Got it. I’ll focus on the mission now.",
                NextNodeID = mainNodeAIId
            });

            db.Insert(new DialogueChoiceTable
            {
                NodeID = loreNodeAIId,
                ChoiceText = "So how do I actually take it down?",
                NextNodeID = strategyNodeAIId
            });

            #endregion

        }
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
            StageId = data.Stage != null ? data.Stage.StageId : 1,

            LastPlayed = DateTime.UtcNow,
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
            LastPlayed = data.LastPlayed,
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
    /// Returns the last played save.
    /// </summary>
    /// <returns>Returns the last played save.</returns>
    public SaveModel GetLastPlayedSave()
    {
        // Sort by most recently played.
        var lastSaveRow = this.db.Table<SaveTable>()
            .OrderByDescending(s => s.LastPlayed)
            .FirstOrDefault();

        // If last row is null, return null.
        if (lastSaveRow == null)
        {
            return null;
        }

        return new SaveModel(lastSaveRow, GetStageById(lastSaveRow.StageId));
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
    public List<DialogueModel> GetDialoguesForNPC(int npcId)
    {
        // Fetch all dialogues for this NPC.
        var dialogues = this.db.Table<DialogueTable>()
            .Where(d => d.NPCID == npcId)
            .ToList();

        // Fetch all nodes for these dialogues.
        var dialogueIds = dialogues.Select(d => d.DialogueID).ToList();
        var nodes = this.db.Table<DialogueNodeTable>()
            .Where(n => dialogueIds.Contains(n.DialogueID))
            .OrderBy(n => n.Order)
            .ToList();

        // Fetch all choices for these nodes.
        var nodeIds = nodes.Select(n => n.NodeID).ToList();
        var choices = this.db.Table<DialogueChoiceTable>()
            .Where(c => nodeIds.Contains(c.NodeID))
            .ToList();

        // Return dialogues.
        return dialogues
            .Select(d => new DialogueModel(d, nodes, choices))
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
