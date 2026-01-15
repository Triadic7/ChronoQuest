using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pattern minigame that tasks players to match the pattern given.
/// </summary>
public class PatternMinigame : Game
{
    /// <summary>
    /// The amount of pressure plates to spawn in.
    /// </summary>
    [SerializeField, Range(1, 8)]
    private int pressurePlateCount;

    /// <summary>
    /// How many plates are in the pattern.
    /// </summary>
    [SerializeField]
    private int patternCount;

    /// <summary>
    /// The distance between plates.
    /// </summary>
    [SerializeField]
    private int pressurePlateOffset;

    /// <summary>
    /// The prefab of the pressure plate.
    /// </summary>
    [SerializeField]
    private GameObject pressurePlatePrefab;

    /// <summary>
    /// The spawn point for the pressure plate.
    /// </summary>
    [SerializeField]
    private Transform pressurePlateSpawnPoint;

    /// <summary>
    /// Speed at which to display the pattern.
    /// </summary>
    [SerializeField]
    private float patternDisplaySpeed = 1f;

    /// <summary>
    /// The delay between displaying each part of the pattern.
    /// </summary>
    [SerializeField]
    private float patternDisplayDelay = 0.5f;

    [Header("SFX")]
    [SerializeField]
    private AudioClip pressurePlateHighlightSfx;

    [SerializeField]
    private AudioClip pressurePlateWrongSfx;

    private List<PressurePlate> currentPattern = new List<PressurePlate>();

    /// <summary>
    /// The current pressure plates.
    /// </summary>
    private List<PressurePlate> instancedPressurePlates = new List<PressurePlate>();

    private int currentPatternIndex;

    private Coroutine displayPatternCoroutine;

    [SerializeField]
    private List<Sprite> heiroglyphs;

    [SerializeField]
    private List<Sprite> activatedHeiroglyphs;

    /// <summary>
    /// Initializes and begins the pattern game, enabling player interaction with pressure plates and starting the game
    /// sequence.
    /// </summary>
    public override void StartGame()
    {
        base.StartGame();

        this.InstancePressurePlates();
        Debug.Log("Starting pattern game!");
    }

    /// <summary>
    /// Begins the game objective by generating and displaying the required pattern to the player.
    /// </summary>
    public override void StartGameObjective()
    {
        base.StartGameObjective();
        this.GeneratePattern();
        this.displayPatternCoroutine = StartCoroutine(this.DisplayPattern());
    }

    /// <summary>
    /// Remove plates and enemies.
    /// </summary>
    public override void CleanUp()
    {
        foreach (PressurePlate plate in this.instancedPressurePlates)
        {
            if(plate != null)
            {
                Destroy(plate.gameObject);
            }
        }
    }

    /// <summary>
    /// Handles cleanup when the object is destroyed by unsubscribing from all pressure plate events.
    /// </summary>
    private void OnDestroy()
    {
        foreach (PressurePlate plate in this.instancedPressurePlates)
        {
            if (plate != null)
            {
                plate.OnPressed -= this.OnPlatePressed;
            }
        }

        // Clear list of pressure plates.
        this.instancedPressurePlates.Clear();
    }

    /// <summary>
    /// Handles cleanup operations when the game has ended, such as stopping any active display pattern coroutines.
    /// </summary>
    public override void GameEnded()
    {
        if (this.displayPatternCoroutine != null)
        {
            this.StopCoroutine(this.displayPatternCoroutine);
        }
    }

    /// <summary>
    /// Instance the panels at game start.
    /// </summary>
    private void InstancePressurePlates()
    {
        for (int i = 0; i < this.pressurePlateCount; i++)
        {
            Vector3 spawnPos = new Vector3(this.pressurePlateSpawnPoint.position.x + (i * this.pressurePlateOffset), this.pressurePlateSpawnPoint.position.y, 0);

            // Instance pressure plates from spwan point, going to the right.
            GameObject pressurePlate = Instantiate(this.pressurePlatePrefab, spawnPos, Quaternion.identity);
            PressurePlate plate = pressurePlate.GetComponent<PressurePlate>();
            this.instancedPressurePlates.Add(plate);

            // Assign heiroglyph sprite.
            if (i < this.heiroglyphs.Count)
            {
                SpriteRenderer renderer = pressurePlate.GetComponent<SpriteRenderer>();
                renderer.sprite = this.heiroglyphs[i];
                plate.SetSprites(this.heiroglyphs[i], this.activatedHeiroglyphs[i]);
            }

            // Add event listener.
            plate.OnPressed += this.OnPlatePressed;
        }
    }

    /// <summary>
    /// Generates a new random pattern of pressure plates based on the current stage's objective progress.
    /// </summary>
    private void GeneratePattern()
    {
        this.currentPattern.Clear();
        this.currentPatternIndex = 0;

        // Use pattern length from stage data, but ensure it's at least 3 for testing.
        int patternLength = Mathf.Max(3, this.patternCount);

        for (int i = 0; i < patternLength; i++)
        {
            this.currentPattern.Add(this.instancedPressurePlates[Random.Range(0, this.instancedPressurePlates.Count)]);
        }
    }

    /// <summary>
    /// Displays the current pressure plate pattern by highlighting each plate in sequence with a timed delay between
    /// highlights.
    /// </summary>
    /// <returns>An enumerator that performs the pattern display sequence when iterated. Each iteration waits for the specified
    /// display speed and delay before highlighting the next plate.</returns>
    private IEnumerator DisplayPattern()
    {
        // Wait a moment before starting the pattern display.
        yield return new WaitForSeconds(1f);

        foreach (PressurePlate plate in this.currentPattern)
        {
            if(plate != null)
            {
                plate.Highlight(this.patternDisplaySpeed);
                if (this.pressurePlateHighlightSfx != null)
                {
                    GameManager.Instance.SFXManager.PlaySound(this.pressurePlateHighlightSfx);
                }
                yield return new WaitForSeconds(this.patternDisplaySpeed + this.patternDisplayDelay);
            }
        }
    }

    /// <summary>
    /// Processes a pressure plate press as part of the current pattern sequence, updating progress and handling success
    /// or failure conditions.
    /// </summary>
    /// <param name="plate">The pressure plate that was pressed. Must not be null.</param>
    private void OnPlatePressed(PressurePlate plate)
    {
        if (this.currentPattern.Count == 0 || this.currentPatternIndex >= this.currentPattern.Count)
        {
            return;
        }

        plate.Highlight(0.5f);
        if (this.pressurePlateHighlightSfx != null)
        {
            GameManager.Instance.SFXManager.PlaySound(this.pressurePlateHighlightSfx);
        }

        if (this.currentPattern[this.currentPatternIndex] == plate)
        {
            this.currentPatternIndex++;

            if (this.currentPatternIndex >= this.currentPattern.Count)
            {
                this.RegisterSuccess();

                if (!this.IsGameFinished())
                {
                    this.StartGameObjective();
                }
            }
        }
        else
        {
            if (this.pressurePlateWrongSfx != null)
            {
                GameManager.Instance.SFXManager.PlaySound(this.pressurePlateWrongSfx);
            }

            this.ReceiveFailStrike();

            if (!this.IsGameFinished())
            {
                this.currentPatternIndex = 0;
                this.displayPatternCoroutine = this.StartCoroutine(this.DisplayPattern());
            }
        }
    }
}