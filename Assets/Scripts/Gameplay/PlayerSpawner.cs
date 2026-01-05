using UnityEngine;

/// <summary>
/// Manages how many players will spawn and where.
/// </summary>
public class PlayerSpawner : MonoBehaviour
{
    /// <summary>
    /// Player one GameObject.
    /// </summary>
    [SerializeField]
    private GameObject playerOne;

    /// <summary>
    /// Player two GameObject.
    /// </summary>
    [SerializeField]
    private GameObject playerTwo;

    /// <summary>
    /// Shows or hides player two depending on if it's coop or not.
    /// </summary>
    /// <param name="isCoop"></param>
    private void ShowPlayers(bool isCoop)
    {
        this.playerOne.SetActive(true);

        // If game is coop, show second player.
        if (!isCoop)
        {
            this.playerTwo.SetActive(false);
        }
        else
        {
            this.playerTwo.SetActive(true);
        }
    }

    /// <summary>
    /// Hides both players.
    /// </summary>
    private void HidePlayers()
    {
        this.playerOne.SetActive(false);
        this.playerTwo.SetActive(false);
    }

    /// <summary>
    /// On game start, put players in game.
    /// </summary>
    private void Start()
    {
        GameManager gm = GameManager.Instance;

        if(gm == null)
        {
            Debug.LogError("No GameManager found.");
            return;
        }

        // On game start, check if game is coop.
        gm.OnGameStartMultiplayer += this.ShowPlayers;

        // On game end hide players.
        gm.OnStageEnd += this.HidePlayers;
    }
}
