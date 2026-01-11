using UnityEngine;

/// <summary>
/// Handles spawning players and calling events if they die.
/// </summary>
public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private GameObject player1;

    [SerializeField]
    private GameObject player2;

    /// <summary>
    /// Add event listeners.
    /// </summary>
    private void Start()
    {
        GameStageManager stageManager = GameManager.Instance.GameStageManager;

        if(stageManager == null)
        {
            Debug.Log("No Stage Manager.");
            return;
        }

        // Turn off players at start.
        this.player1.SetActive(false);
        this.player2.SetActive(false);

        // On game start, spawn players at position.
        stageManager.OnGameSet += (game) =>
        {
            this.SpawnPlayers(game, GameManager.Instance.CurrentSave.IsCoop);
        };

        // On game end, deactivate players.
        stageManager.OnObjectiveStop += () =>
        {
            this.DeactivatePlayers(1f);
        };
    }

    /// <summary>
    /// Activate players and move them to the position.
    /// </summary>
    /// <param name="game">The game the players will spawn in.</param>
    /// <param name="isCoop">Will spawn in second player if true.</param>
    private void SpawnPlayers(Game game, bool isCoop)
    {
        Vector3 spawnPosition = game.PlayerSpawnPoint.position;

        // Ensure the GameObject is active first.
        this.player1.SetActive(true);
        this.player1.GetComponent<PlayerVisuals>().FadeIn(0.25f);

        // Add player 2 if is coop.
        if (isCoop)
        {
            this.player2.SetActive(true);
            this.player2.GetComponent<PlayerVisuals>().FadeIn(0.25f);

            // Add offset for player 2.
            Vector3 offset = new Vector3(spawnPosition.x + 2, spawnPosition.y, 0);
            this.player2.transform.position = offset;
        }

        this.player1.transform.position = spawnPosition;
    }

    /// <summary>
    /// Fades away players game objects.
    /// </summary>
    /// <param name="duration">How fast to deactivate.</param>
    private void DeactivatePlayers(float duration = 1f)
    {
        if (this.player1.gameObject.activeInHierarchy)
        {
            this.player1.GetComponent<PlayerVisuals>().FadeOutAndDisable(duration);
        }
        if (this.player2.gameObject.activeInHierarchy)
        {
            this.player2.GetComponent<PlayerVisuals>().FadeOutAndDisable(duration);
        }
    }
}
