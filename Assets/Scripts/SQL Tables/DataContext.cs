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

        // Create progress table.
        this.db.CreateTable<StageProgressTable>();

        // Create stage table.
        this.db.CreateTable<StageTable>();

        // Seed stages.
        this.db.InsertOrReplace(new StageTable 
        { 
            StageId = 1, 
            StageName = "Egypt", 
            StageDescription = "To save Egypt, you must match the given pattern sequence to maintain balance. Failure is not an option.",
            ObjectiveText = "Match the pattern",
            ObjectiveProgress = 5,
            LocationID = 1,
            NPCId = 1,
        });
        this.db.InsertOrReplace(new StageTable 
        { 
            StageId = 2, 
            StageName = "Medieval Europe", 
            StageDescription = "Protect the base at all costs! You're given a new suit that wards away evil forces. Simply coming into contact with anomalies will remove them from the timeline.",
            ObjectiveText = "Touch the anomalies to banish them",
            LocationID = 2,
            NPCId = 2,
        });
        this.db.InsertOrReplace(new StageTable 
        { 
            StageId = 3, 
            StageName = "Modern Day", 
            StageDescription = "Hurry up and grab the fresh moments in time before they disappear and summon anomalies.",
            ObjectiveText = "Grab the disturbances before they fully appear",
            LocationID = 3,
            NPCId = 3,
        });
        this.db.InsertOrReplace(new StageTable 
        { 
            StageId = 4, 
            StageName = "Space Age", 
            StageDescription = "Final boss time. Protect the space station and destroy the interlopers once and for all!",
            ObjectiveText = "Defeat the boss and protect the station",
            ObjectiveProgress = 20,
            LocationID = 4,
            NPCId = 4,
            IsFinalStage = true 
        });

        // Create npc table.
        this.db.CreateTable<NPCTable>();

        // Allow recache.
        this.db.DeleteAll<NPCTable>();

        // Seed NPCs.
        if (!this.db.Table<NPCTable>().Any())
        {
            this.db.Insert(new NPCTable { NPCId = 1, Name = "Pharaoh", ImagePath = "Pharaoh" });
            this.db.Insert(new NPCTable { NPCId = 2, Name = "King Arthur", ImagePath = "KingArthur" });
            this.db.Insert(new NPCTable { NPCId = 3, Name = "Supervisor", ImagePath = "Supervisor" });
            this.db.Insert(new NPCTable { NPCId = 4, Name = "S.I.", ImagePath = "SI" });
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
            #region Pharaoh Intro Dialogue
            // PPharaoh's dialogue.
            DialogueTable dialogue = new DialogueTable
            {
                NPCID = 1,
                NPCName = "Pharaoh"
            };
            db.Insert(dialogue);
            int dialogueId = dialogue.DialogueID;

            // Nodes.
            var mainNode = new DialogueNodeTable
            {
                DialogueID = dialogueId,
                Order = 0,
                DialogueText = "Are you ready? I'm waiting on you before I start.",
                IsEndNode = false
            };
            this.db.Insert(mainNode);
            int mainNodeId = mainNode.NodeID;

            var savePeopleNode = new DialogueNodeTable
            {
                DialogueID = dialogueId,
                Order = 1,
                DialogueText = "You're here to save my people and I, are you not? Egypt itself is relying on you. I expect you to bring order.",
                IsEndNode = false
            };
            this.db.Insert(savePeopleNode);
            int savePeopleNodeId = savePeopleNode.NodeID;

            var ritualNode = new DialogueNodeTable
            {
                DialogueID = dialogueId,
                Order = 2,
                DialogueText = "I know not the year from which you come from, but I know of your purpose. You will help me complete a ritual summons. I will start the ritual using a series of glyphs. Match the glyphs in the order I summon them. Doing so will activate the <link=pyramid><color=#00BFFF>Pyramid's</color></link> secret weapon and stop the <link=anomalies><color=#00BFFF>Anomalies</color></link>.",
                IsEndNode = false
            };
            this.db.Insert(ritualNode);
            int ritualNodeId = ritualNode.NodeID;

            // Choices.
            this.db.Insert(new DialogueChoiceTable
            {
                NodeID = mainNodeId,
                ChoiceText = "<color=#FFA500>I'm ready to help you. I'll remember the pattern.</color>",
                // Triggers game action.
                NextNodeID = -1
            });

            this.db.Insert(new DialogueChoiceTable
            {
                NodeID = mainNodeId,
                ChoiceText = "Do what again? Can you fill me in on some details?",
                NextNodeID = savePeopleNodeId
            });

            this.db.Insert(new DialogueChoiceTable
            {
                NodeID = savePeopleNodeId,
                ChoiceText = "I've just arrived.",
                NextNodeID = ritualNodeId
            });

            this.db.Insert(new DialogueChoiceTable
            {
                NodeID = savePeopleNodeId,
                ChoiceText = "Yes, I'm on a mission here to stop the <link=anomalies><color=#00BFFF>Anomalies</color></link>. I'm told Egypts <link=pyramid><color=#00BFFF>Pyramids</color></link> will help us.",
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

            #region Pharaoh Outro Dialogue

            // Success outro.
            DialogueTable outroWinDialogue = new DialogueTable
            {
                NPCID = 1,
                NPCName = "Pharaoh",
                IsOutro = true,
                IsWin = true
            };
            db.Insert(outroWinDialogue);
            int outroWinDialogueId = outroWinDialogue.DialogueID;

            var outroWinNode = new DialogueNodeTable
            {
                DialogueID = outroWinDialogueId,
                Order = 0,
                DialogueText = "Yes! You saved me and the people of this land. With this sequence of events we have unlocked a powerful weapon in the coming fight. Although I know I wont be there with you, I wish you luck on your journey ahead.",
                IsEndNode = false
            };
            db.Insert(outroWinNode);
            int outroWinNodeId = outroWinNode.NodeID;

            db.Insert(new DialogueChoiceTable
            {
                NodeID = outroWinNodeId,
                ChoiceText = "Continue",
                NextNodeID = -1
            });

            // Failure outro.
            DialogueTable outroLoseDialogue = new DialogueTable
            {
                NPCID = 1,
                NPCName = "Pharaoh",
                IsOutro = true,
                IsWin = false
            };
            db.Insert(outroLoseDialogue);
            int outroLoseDialogueId = outroLoseDialogue.DialogueID;

            var outroLoseNode = new DialogueNodeTable
            {
                DialogueID = outroLoseDialogueId,
                Order = 0,
                DialogueText = "*The Pharaoh looks in horror* We needed this to work! Without this the future will be uncertain. Mine and yours! My people's! Go! Go back to your time. You have done enough.",
                IsEndNode = false
            };
            db.Insert(outroLoseNode);
            int outroLoseNodeId = outroLoseNode.NodeID;

            db.Insert(new DialogueChoiceTable
            {
                NodeID = outroLoseNodeId,
                ChoiceText = "Continue",
                NextNodeID = -1
            });
            #endregion

            #region King Arthur Intro Dialogue
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
                DialogueText = "You must defend the <link=castle><color=#00BFFF>Castle</color></link> walls. Hold out while I try to decipher the ancient code beneath the castle.",
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
                DialogueText = "I know as much as you I'm afraid. Someone similar to you arrived here and said you would arrive to help. That's all I know.",
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
                ChoiceText = "I will prevent the enemy from getting in here. You focus on your mission.",
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
                ChoiceText = "So I just touch them, and they disappear? Seems like something I can do.",
                // Loops back to main node for start action.
                NextNodeID = mainNodeArthurId
            });

            db.Insert(new DialogueChoiceTable
            {
                NodeID = strategyNodeArthurId,
                ChoiceText = "Tell me more about these <link=anomalies><color=#00BFFF>Anomalies</color></link> and why they exist.",
                NextNodeID = loreNodeArthurId
            });

            // Loops back to main node for start action.
            db.Insert(new DialogueChoiceTable
            {
                NodeID = loreNodeArthurId,
                ChoiceText = "That must have been my peer. I'm here to provide aid to you.",
                NextNodeID = mainNodeArthurId
            });
            #endregion

            #region King Arthur Outro Dialogue

            // Success outro.
            DialogueTable arthurWinDialogue = new DialogueTable
            {
                NPCID = 2,
                NPCName = "King Arthur",
                IsOutro = true,
                IsWin = true
            };
            db.Insert(arthurWinDialogue);
            int arthurWinDialogueId = arthurWinDialogue.DialogueID;

            var arthurWinNode = new DialogueNodeTable
            {
                DialogueID = arthurWinDialogueId,
                Order = 0,
                DialogueText = "You have acted as bravely as any knight I've ever had the honor of knowing. I have decoded the strange langauge In the ruins under my castle. I'm told this will be able to help you in the future. Good luck to you",
                IsEndNode = false
            };
            db.Insert(arthurWinNode);
            int arthurWinNodeId = arthurWinNode.NodeID;

            db.Insert(new DialogueChoiceTable
            {
                NodeID = arthurWinNodeId,
                ChoiceText = "Continue",
                NextNodeID = -1
            });

            // Failure outro.
            DialogueTable arthurLoseDialogue = new DialogueTable
            {
                NPCID = 2,
                NPCName = "King Arthur",
                IsOutro = true,
                IsWin = false
            };
            db.Insert(arthurLoseDialogue);
            int arthurLoseDialogueId = arthurLoseDialogue.DialogueID;

            var arthurLoseNode = new DialogueNodeTable
            {
                DialogueID = arthurLoseDialogueId,
                Order = 0,
                DialogueText = "My knights have left me for this. The castle is destroyed. Any secrets we could have discovered are lost. I suggest you go help mess something else up for someone else. I have no desire to have you in my presense.",
                IsEndNode = false
            };
            db.Insert(arthurLoseNode);
            int arthurLoseNodeId = arthurLoseNode.NodeID;

            db.Insert(new DialogueChoiceTable
            {
                NodeID = arthurLoseNodeId,
                ChoiceText = "Continue",
                NextNodeID = -1
            });

            #endregion

            #region Modern Day Supervisor Intro Dialogue
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
                DialogueText = "There's a high chance that might happen. Remove <link=disturbance><color=#00BFFF>Disturbances</color></link> before they fully appear, society isn't ready for such technology.",
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
                ChoiceText = "Wait, will I be attacked here?",
                NextNodeID = strategyNodeSupervisorId
            });

            db.Insert(new DialogueChoiceTable
            {
                NodeID = strategyNodeSupervisorId,
                ChoiceText = "Got it I guess.",
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
                ChoiceText = "Understood Sir.",
                NextNodeID = mainNodeSupervisorId
            });
            #endregion

            #region Modern Day Supervisor Outro Dialogue

            // Success outro.
            DialogueTable supervisorWinDialogue = new DialogueTable
            {
                NPCID = 3,
                NPCName = "Supervisor",
                IsOutro = true,
                IsWin = true
            };
            db.Insert(supervisorWinDialogue);
            int supervisorWinDialogueId = supervisorWinDialogue.DialogueID;

            var supervisorWinNode = new DialogueNodeTable
            {
                DialogueID = supervisorWinDialogueId,
                Order = 0,
                DialogueText = "Excellent work. I was told to observe you and make sure you didn't make any mistakes. I'll have to put a recommendation in for you. This timeline is saved. Off to the next!",
                IsEndNode = false
            };
            db.Insert(supervisorWinNode);
            int supervisorWinNodeId = supervisorWinNode.NodeID;

            db.Insert(new DialogueChoiceTable
            {
                NodeID = supervisorWinNodeId,
                ChoiceText = "Continue",
                NextNodeID = -1
            });

            // Failure outro.
            DialogueTable supervisorLoseDialogue = new DialogueTable
            {
                NPCID = 3,
                NPCName = "Supervisor",
                IsOutro = true,
                IsWin = false
            };
            db.Insert(supervisorLoseDialogue);
            int supervisorLoseDialogueId = supervisorLoseDialogue.DialogueID;

            var supervisorLoseNode = new DialogueNodeTable
            {
                DialogueID = supervisorLoseDialogueId,
                Order = 0,
                DialogueText = "Tsk. This was a catastrophe and we can't afford to go back and change things. This will go on your record of course. Don't expect any sort of holiday bonus this year.",
                IsEndNode = false
            };
            db.Insert(supervisorLoseNode);
            int supervisorLoseNodeId = supervisorLoseNode.NodeID;

            db.Insert(new DialogueChoiceTable
            {
                NodeID = supervisorLoseNodeId,
                ChoiceText = "Continue",
                NextNodeID = -1
            });

            #endregion

            #region Super Intelligent AI Intro Dialogue
            // Super Intelligent AI dialogue.
            DialogueTable aiDialogue = new DialogueTable
            {
                NPCID = 4,
                NPCName = "S.I."
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
                DialogueText = "Directive: prioritize engagement with the <link=interloper><color=#00BFFF>Interloper</color></link> using all available projectiles until destabilization metrics reach threshold.",
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
                ChoiceText = "There's history of this thing? I've never seen one before.",
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

            #region Super Intelligent AI Outro Dialogue

            // Success outro.
            DialogueTable aiWinDialogue = new DialogueTable
            {
                NPCID = 4,
                NPCName = "S.I.",
                IsOutro = true,
                IsWin = true
            };
            db.Insert(aiWinDialogue);
            int aiWinDialogueId = aiWinDialogue.DialogueID;

            var aiWinNode = new DialogueNodeTable
            {
                DialogueID = aiWinDialogueId,
                Order = 0,
                DialogueText = "I'm registering the mission as a success. The station has been defending and the Interloper is defeated. Scans indicate the other Anomalies fading away after the destruction of the Interloper.",
                IsEndNode = false
            };
            db.Insert(aiWinNode);
            int aiWinNodeId = aiWinNode.NodeID;

            db.Insert(new DialogueChoiceTable
            {
                NodeID = aiWinNodeId,
                ChoiceText = "Continue",
                NextNodeID = -1
            });

            // Failure outro.
            DialogueTable aiLoseDialogue = new DialogueTable
            {
                NPCID = 4,
                NPCName = "S.I.",
                IsOutro = true,
                IsWin = false
            };
            db.Insert(aiLoseDialogue);
            int aiLoseDialogueId = aiLoseDialogue.DialogueID;

            var aiLoseNode = new DialogueNodeTable
            {
                DialogueID = aiLoseDialogueId,
                Order = 0,
                DialogueText = "*The audio from S.I. is hard to make out* Station hull integrity is failing quickly. Recommended course of action is to leave the area immediately. I will self destruct the station to buy you some time. Goodbye. *The comms go silent.*",
                IsEndNode = false
            };
            db.Insert(aiLoseNode);
            int aiLoseNodeId = aiLoseNode.NodeID;

            db.Insert(new DialogueChoiceTable
            {
                NodeID = aiLoseNodeId,
                ChoiceText = "Continue",
                NextNodeID = -1
            });

            #endregion
        }

        // Create location table.
        db.CreateTable<LocationTable>();

        // Seed locations.
        if (!db.Table<LocationTable>().Any())
        {
            db.Insert(new LocationTable
            {
                LocationID = 1,
                LocationName = "Egypt",
                BackgroundTileNames = "sand,sand3,sand4",
                ForegroundTileNames = "cactus2,cactus3"
            });

            db.Insert(new LocationTable
            {
                LocationID = 2,
                LocationName = "Medieval Europe",
                BackgroundTileNames = "grass1,grass2",
                ForegroundTileNames = "tree1,tree2"
            });

            db.Insert(new LocationTable
            {
                LocationID = 3,
                LocationName = "Modern Day",
                BackgroundTileNames = "grass1,grass2",
                ForegroundTileNames = ""
            });

            db.Insert(new LocationTable
            {
                LocationID = 4,
                LocationName = "Space",
                BackgroundTileNames = "space1,space2",
                ForegroundTileNames = ""
            });
        }
    }

    /// <summary>
    /// Gets all locations,
    /// </summary>
    /// <returns>Returns a list of all locations.</returns>
    public List<LocationTable> GetLocations()
    {
        return db.Table<LocationTable>().ToList();
    }

    /// <summary>
    /// Gets location table by id.
    /// </summary>
    /// <param name="locationId"></param>
    /// <returns>Returns a location by id.</returns>
    public LocationTable GetLocationByID(int locationId)
    {
        return this.db.Table<LocationTable>().FirstOrDefault(l => l.LocationID == locationId); 
    }

    /// <summary>
    /// Returns a list of saves.
    /// </summary>
    /// <returns>Returns a list of all saves.</returns>
    public List<SaveModel> GetSaves()
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
        data.LastPlayed = DateTime.UtcNow;

        // Converts save model to save table.
        SaveTable table = this.ToTable(data);

        // Insert into table.
        db.Insert(table);

        // Returns inserted table id.
        return table.Id;
    }

    /// <summary>
    /// Updates an existing save.
    /// </summary>
    /// <param name="data">The data to update.</param>
    public void UpdateSave(SaveModel data)
    {
        // Put last played.
        data.LastPlayed = DateTime.UtcNow;

        // Update table using the model to table method.
        db.Update(this.ToTable(data));
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
    /// Insert stage progress.
    /// </summary>
    /// <param name="saveId">The save id.</param>
    /// <param name="progress">The progress model.</param>
    public void InsertStageProgress(int saveId, StageProgressModel progress)
    {
        var table = new StageProgressTable
        {
            SaveId = saveId,
            StageId = progress.StageID,
            // Parse enum to int.
            Result = (int)progress.Result,
        };

        db.Insert(table);
    }

    /// <summary>
    /// Update stage progress.
    /// </summary>
    /// <param name="saveId">The save id.</param>
    /// <param name="progress">The progress model.</param>
    public void UpdateStageProgress(int saveId, StageProgressModel progress)
    {
        var table = new StageProgressTable
        {
            SaveId = saveId,
            StageId = progress.StageID,
            // Parse enum to int.
            Result = (int)progress.Result,
        };

        db.Update(table);
    }

    /// <summary>
    /// Checks if stage progress exists at save id.
    /// </summary>
    /// <param name="saveId">The save id.</param>
    /// <param name="stageId">The stage id.</param>
    /// <returns>Returns true if found.</returns>
    public bool StageProgressExists(int saveId, int stageId)
    {
        return db.Table<StageProgressTable>()
                 .Where(x => x.SaveId == saveId && x.StageId == stageId)
                 .Count() > 0;
    }

    /// <summary>
    /// Gets all stage progress for a specific save.
    /// </summary>
    /// <param name="saveId">The save ID.</param>
    /// <returns>List of stage progress models.</returns>
    public List<StageProgressModel> GetAllStageProgress(int saveId)
    {
        // Query all rows for the saveId.
        var tableRows = db.Table<StageProgressTable>()
                          .Where(x => x.SaveId == saveId)
                          .ToList();

        // Map database rows to StageProgressModel.
        var progressList = new List<StageProgressModel>();
        foreach (var row in tableRows)
        {
            var progress = new StageProgressModel
            {
                StageID = row.StageId,
                //Parse enum.
                Result = (StageResult)row.Result
            };
            progressList.Add(progress);
        }

        return progressList;
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
    /// Gets the outro dialogue for a specific stage and NPC, optionally by win/loss.
    /// </summary>
    /// <param name="stageId">The stage id.</param>
    /// <param name="won">Whether the stage was won or lost.</param>
    /// <returns>The outro dialogue for the stage.</returns>
    public DialogueModel GetOutroDialogueForStage(int stageId, bool won)
    {
        // Fetch the NPC for this stage.
        StageModel stage = this.GetStageById(stageId);
        if (stage == null)
        {
            return null;
        }

        // Get npc id.
        int npcId = stage.NPCId;

        // Get outro dialogue based on win or lose.
        var dialogueTable = this.db.Table<DialogueTable>()
            .FirstOrDefault(d => d.NPCID == npcId && d.IsOutro == true && d.IsWin == won);

        if (dialogueTable == null)
        {
            return null;
        }

        // Fetch nodes for this dialogue.
        var nodes = this.db.Table<DialogueNodeTable>()
            .Where(n => n.DialogueID == dialogueTable.DialogueID)
            .OrderBy(n => n.Order)
            .ToList();

        // Fetch choices for these nodes.
        var nodeIds = nodes.Select(n => n.NodeID).ToList();
        var choices = this.db.Table<DialogueChoiceTable>()
            .Where(c => nodeIds.Contains(c.NodeID))
            .ToList();

        return new DialogueModel(dialogueTable, nodes, choices);
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

    /// <summary>
    /// Converts SaveModel to table for sqlite.
    /// </summary>
    /// <param name="data">The model.</param>
    /// <returns>Returns a SaveTable.</returns>
    private SaveTable ToTable(SaveModel data)
    {
        return new SaveTable
        {
            Id = data.SaveID,
            StageId = data.CurrentStageId,
            PlayTime = data.PlayTime,
            IsCoop = data.IsCoop,
            LastPlayed = data.LastPlayed
        };
    }


}
