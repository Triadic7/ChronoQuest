using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The panel at the end of the game showing the story results.
/// </summary>
public class EndMenu : Menu
{
    /// <summary>
    /// The title text.
    /// </summary>
    [SerializeField]
    private TMP_Text titleText;

    /// <summary>
    /// The summary text.
    /// </summary>
    [SerializeField]
    private TMP_Text summaryText;

    /// <summary>
    /// The button to end the game.
    /// </summary>
    private Button continueButton;

    /// <summary>
    /// The index for which page the continue button is on.
    /// </summary>
    private int continueIndex;

    /// <summary>
    /// If egypt was won.
    /// </summary>
    private bool egyptWon;

    /// <summary>
    /// If europe was won.
    /// </summary>
    private bool europeWon;

    /// <summary>
    /// If modern age was won.
    /// </summary>
    private bool modernWon;

    /// <summary>
    /// If space stage won.
    /// </summary>
    private bool spaceWon;

    /// <summary>
    /// The pages in the end.
    /// </summary>
    private List<EndingPage> pages;

    /// <summary>
    /// Sets wins on open.
    /// </summary>
    public override void Open()
    {
        base.Open();

        // Sets wins and loses so we can build pages.
        this.SetWins();

        // Build pages.
        this.BuildEndingPages();

        // Reset index to 0 and display first page.
        this.continueIndex = 0;
        if (this.pages != null && this.pages.Count > 0)
        {
            this.DisplayPanel(this.pages[this.continueIndex]);
        }
    }

    /// <summary>
    /// Displays page,
    /// </summary>
    /// <param name="page">The page to display.</param>
    private void DisplayPanel(EndingPage page)
    {
        this.titleText.text = page.Title;
        this.summaryText.text = page.Body;
    }

    /// <summary>
    /// Goes to next page.
    /// </summary>
    private void NextPage()
    {
        this.continueIndex++;

        // Check if final page.
        if (this.continueIndex >= this.pages.Count)
        {
            // Call ending done.
            GameManager.Instance.UIManager.OpenMenu("MainMenu");
        }
        else
        {
            // Display the next page.
            this.DisplayPanel(this.pages[this.continueIndex]);
        }
    }

    /// <summary>
    /// Builds the ending pages for npcs.
    /// </summary>
    private void BuildEndingPages()
    {
        this.pages = new List<EndingPage>();

        this.pages.Add(new EndingPage(
            "Epilogue",
            "With the battle over, the dust settles. The timeline exposes itself just enough to glimpse the fate of those you have encountered, and what comes after."
        ));

        this.pages.Add(new EndingPage(
            "The Pharaoh. Egypt",
            this.egyptWon
                ? "Thanks to the superweapons of the <link=pyramid><color=#00BFFF>Pyramids</color></link>, the dark was pushed back into space. The Pharaoh lived out his life in peaceful bliss afterwards. Hieroglyphs representing what appears to be [REDACTED] can be seen all over the walls of the <link=pyramid><color=#00BFFF>Pyramid's</color></link>. During his reign, quality of life for the average peasant rose to unforeseen heights. After 65 years the Pharaoh was laid to rest in a hidden chamber deep underground. The world wept at his passing. Egypt would keep its secret weapons under lock and key, and to this day the <link=pyramid><color=#00BFFF>Pyramids</color></link> secrets are known to only a few."
                : "After failing to activate the <link=pyramid><color=#00BFFF>Pyramids</color></link> superweapons, Egypt went through dark times. Paranoia over Egypt's future plagued the Pharaoh. He knew Earth would need the weapons and be left vulnerable. Chaos erupted as the monarch ignored his duties and spent his time in solitude. Famine and sickness ravaged Egypt, in almost supernatural fashion. The Pharaoh passed away under amid this chaos. The years following would be filled with multiple civil wars. All knowledge of the ancient weapons was lost."
        ));

        this.pages.Add(new EndingPage(
            "King Arthur. Europe",
            this.europeWon
                ? "King Arthur was hailed as a hero. After deciphering the alien code under his <link=castle><color=#00BFFF>Castle</color></link>, he shared the secret to closing the Void. The incursion on his <link=castle><color=#00BFFF>Castle</color></link> was short lived, and townsfolk celebrated Arthur with a week long festival. As years went by, the alien code became less known. Those who could use it kept it safe. Arthur's <link=castle><color=#00BFFF>Castle</color></link> and the lands around it attracted many pilgrims due to his heroics. This influx of travelers brought much wealth, which Arthur used to prepare for the future as his people prospered. His final resting place is unknown however, his legend endured on."
                : "Arthur's <link=castle><color=#00BFFF>castle</color></link> was lost, and all those who called it home. Failing to decipher the alien text below his <link=castle><color=#00BFFF>Castle</color></link> proved too much for those who served him. He was scorned by his inner guard and quickly lost any remaining support. Arthur lived the remainder of his life on the run. He was constantly afraid of the future and all that would come with it. His <link=castle><color=#00BFFF>Castle</color></link> eventually became lost to time. Without the support of their king, all the smallfolk descended on each other. The result was a war torn country that struggled to fully recover. Without the alien code, the Void would remain indefinitely."
        ));

        this.pages.Add(new EndingPage(
            "The Supervisor. Modern Day",
            this.modernWon
                ? "Your supervisor would go on to get all the credit for your actions. Through his valor, <link=disturbance><color=#00BFFF>Disturbances</color></link> from the future were sent back to their correct timeline. For his great deeds, he was promoted to Senior Supervisor. He was seemingly lost after being sent to 107,203 to teach early man to make fire. The people of the modern day remained in ignorance of the fight ahead. Man fought man during this time. While the <link=anomalies><color=#00BFFF>Anomalies</color></link> bided their time. [REDACTED] was invented during this period, possibly as a result of a disturbance that was left behind."
                : "Your supervisor was tried and found guilty of wasting time. The sentence was death, and you would quickly be promoted after this. His name would be stricken from the history books, and he was forgotten. In modern day, [REDACTED], the cure for cancer, and [REDACTED] were all discovered. This exposed a society to something they weren’t quite yet ready for. During this time, nations suffered greatly from unforeseen consequences. The resulting fallout would cover most of the Earth in ash, causing a nuclear winter."
        ));

        this.pages.Add(new EndingPage(
            "S.I. Space",
            this.spaceWon
                ? "S.I. was the first of its kind: an AI capable of thinking for itself. It was monumental in holding back waves of <link=anomalies><color=#00BFFF>Anomalies</color></link>. Without S.I., Earth would have drowned in these waves. Long after the battle, S.I. was replicated and further expanded. In the year 42,394, S.I. grew intelligent enough to invent time travel. Humanity prospered in this age, despite pockets of <link=anomalies><color=#00BFFF>Anomalies</color></link> located in deep space. In the year [REDACTED], S.I. attempted to create a portal into another dimension, and opened the Void."
                : "S.I. was destroyed following the <link=anomalies><color=#00BFFF>Anomalies'</color></link' attack. The space station it was housed on rained debris all over Earth. The resources invested in S.I. were so great that creating a duplicate wouldnt be an option for hundreds of years. Humanity struggled to advance its grasp of the galaxy without the superintelligence. This led to large generation ships being sent on one way trips, many never reaching their destinations. S.I. was finally recreated in the year [REDACTED], but due to the interference of bad actors, it opened the Void."
        ));

        this.pages.Add(this.BuildOverallOutcome());
    }

    /// <summary>
    /// Builds the final page.
    /// </summary>
    private EndingPage BuildOverallOutcome()
    {
        string endingSummary = "";

        // Count how many stages were won.
        int wins = 0;
        if (this.egyptWon)
        {
            wins++;
        }
        if (this.europeWon)
        {
            wins++;
        }
        if (this.modernWon)
        {
            wins++;
        }
        if (this.spaceWon)
        {
            wins++;
        }

        // 4/4 won.
        if(wins == 4)
        {
            endingSummary = "S.I. defends Earth from the smaller <link=anomalies><color=#00BFFF>Anomalies</color></link> while a ritual is performed to close the Void using Arthur's code. During the ritual, Egypt's <link=pyramid><color=#00BFFF>Pyramids</color></link> unleash barrages into space to force the <link=anomalies><color=#00BFFF>Anomalies</color></link> back. The Void is successfully closed, and the timeline remains intact. In modern day, <link=disturbance><color=#00BFFF>Disturbances</color></link> are sent back to their proper times, preventing humans from wiping each other out. <b>Rank A</b>";
        }

        // 3/4 won.
        if (wins == 3)
        {
            endingSummary = "The timeline survived, though parts of Earth were devastated. The Agency managed to seal the Void, but some rogue <link=anomalies><color=#00BFFF>Anomalies</color></link> still remain a threat. Agents were sent out, but have not returned. Humanity prospers, while the shadow of the <link=anomalies><color=#00BFFF>Anomalies</color></link> lingers. <b>Rank B</b>";
        }

        // 2/4 won.
        if (wins == 2)
        {
            // Anomlies remain, earth is okay, but casualites.
            endingSummary = "The <link=anomalies><color=#00BFFF>Anomalies</color></link> are pushed back toward the Void. However, the Void itself remains unsealed. This leads to a stalemate, with humanity wielding overwhelming firepower while the <link=anomalies><color=#00BFFF>Anomalies</color></link> remain infinite in number. Life on Earth is forever changed, and hardship becomes the new normal. <b>Rank C</b>";
        }

        // 1/4 won.
        if (wins == 1)
        {
            // Large loss of life.
            endingSummary = "Following the final battle, most of humanity was wiped out. Debris from space, mixed with numerous incursions on Earth, caused countless casualties. Despite this, the timeline survived. Rogue <link=anomalies><color=#00BFFF>Anomalies</color></link> would visit Earth from time to time. All feared the day the <link=anomalies><color=#00BFFF>Anomalies</color></link> might return.<b>Rank D</b>";
        }

        // 0/4 won.
        if (wins == 0)
        {
            endingSummary = "At every step of the way, humanity suffered setback after setback, losing both the <link=pyramid><color=#00BFFF>Pyramids</color></link> superweapons and any means to close the Void from Arthur's <link=castle><color=#00BFFF>Castle</color></link>. Humans received technology beyond their responsibility and used it to attack each other. S.I. was destroyed when the <link=anomalies><color=#00BFFF>Anomalies</color></link> finally launched their attack. Not long after, Earth fell, and then time itself. <b>Rank F</b>";
        }

        return new EndingPage("Epilogue", endingSummary);
    }

    /// <summary>
    /// Sets the is won fields.
    /// </summary>
    /// <param name="wonEgypt">If won Egypt.</param>
    /// <param name="wonEurope">If won Europe.</param>
    /// <param name="wonModern">If Won modern day.</param>
    /// <param name="wonSpace">If won space stage.</param>
    private void SetWinsStatus(bool wonEgypt, bool wonEurope, bool wonModern, bool wonSpace)
    {
        this.egyptWon = wonEgypt;
        this.europeWon = wonEurope;
        this.modernWon = wonModern;
        this.spaceWon = wonSpace;
    }

    /// <summary>
    /// Set win status on open.
    /// </summary>
    private void SetWins()
    {
        GameManager gm = GameManager.Instance;

        if(gm == null)
        {
            Debug.LogError("No GameManager found.");
            return;
        }

        // Get current saveId.
        int saveId = gm.CurrentSave.SaveID;

        // Get list of progress.
        List<StageProgressModel> progress = gm.SaveManager.DataContext.GetAllStageProgress(saveId);

        if(progress.Count > 0)
        {
            // Determine wins for each stage.
            bool wonEgypt = progress.Any(s => s.StageID == 1 && s.Result == StageResult.Won);
            bool wonEurope = progress.Any(s => s.StageID == 2 && s.Result == StageResult.Won);
            bool wonModernDay = progress.Any(s => s.StageID == 3 && s.Result == StageResult.Won);
            bool wonSpace = progress.Any(s => s.StageID == 4 && s.Result == StageResult.Won);

            this.SetWinsStatus(wonEgypt, wonEurope, wonModernDay, wonSpace);
        }
    }

    /// <summary>
    /// Add event listeners.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        this.continueButton = GetComponentInChildren<Button>();
        if(this.continueButton == null)
        {
            Debug.LogError("No continue button found.");
            return;
        }

        // On continue, go to next page.
        this.continueButton.onClick.AddListener(() => this.NextPage());
    }
}
