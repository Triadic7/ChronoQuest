using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class for managing input from the players keyboard.
/// </summary>
public class InputManager : MonoBehaviour
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
    /// How fast the players go.
    /// </summary>
    [SerializeField]
    private float playerSpeed;

    /// <summary>
    /// Player one's keybinds.
    /// </summary>
    private PlayerKeybinds playerOneKeybinds;

    /// <summary>
    /// Player two's keybinds.
    /// </summary>
    private PlayerKeybinds playerTwoKeybinds;

    /// <summary>
    /// Rigid body for player one.
    /// </summary>
    private Rigidbody2D playerOneRb;

    /// <summary>
    /// Rigid body for player two.
    /// </summary>
    private Rigidbody2D playerTwoRb;

    /// <summary>
    /// Fires once at game start.
    /// </summary>
    private void Start()
    {
        // Subscribe to keybind event.
        if (GameManager.Instance != null)
        {
            SettingsManager settingsManager = GameManager.Instance.Settings;
            settingsManager.OnKeybindsUpdated += SetPlayerKeybinds;

            // Set keybinds.
            this.playerOneKeybinds = settingsManager.PlayerOneKeybinds;
            this.playerTwoKeybinds = settingsManager.PlayerTwoKeybinds;
        }
        else
        {
            Debug.LogError("No GameManager found.");
            return;
        }

        // Checks for missing players.
        if (playerOne == null || playerTwo == null)
        {
            Debug.LogError("Missing player GameObject.");
            return;
        }
        else
        {
            // Set rigidbodies if players found.
            playerOneRb = playerOne.GetComponent<Rigidbody2D>();
            playerTwoRb = playerTwo.GetComponent<Rigidbody2D>();
        }
    }

    /// <summary>
    /// Updates every frame to manage input.
    /// </summary>
    private void FixedUpdate()
    {
        // Check for player one movement.
        MovePlayer(playerOneRb, playerOneKeybinds);

        // Check for player two movement.
        MovePlayer(playerTwoRb, playerTwoKeybinds);
    }

    /// <summary>
    /// Moves player based on input.
    /// </summary>
    /// <param name="rb">The players rigidbody for moving.</param>
    /// <param name="playerKeybinds">The players keybinds.</param>
    private void MovePlayer(Rigidbody2D rb, PlayerKeybinds playerKeybinds)
    {
        Vector2 movement = Vector2.zero;
        Keyboard keyboard = Keyboard.current;

        // Check for null keyboard.
        if (keyboard == null)
        {
            return;
        }

        // Up.
        if (keyboard[playerKeybinds.up].isPressed)
        {
            movement.y += 1;
        }

        // Down.
        if (keyboard[playerKeybinds.down].isPressed)
        {
            movement.y -= 1;
        }

        // Left.
        if (keyboard[playerKeybinds.left].isPressed)
        {
            movement.x -= 1;
        }

        // Right.
        if (keyboard[playerKeybinds.right].isPressed)
        {
            movement.x += 1;
        }

        // Normalize movement using player speed.
        movement = movement.normalized * playerSpeed;

        // Move rigidbody based on movement.
        rb.linearVelocity = movement;
    }

    /// <summary>
    /// Sets the players keybinds.
    /// </summary>
    /// <param name="p1">Player one keybinds.</param>
    /// <param name="p2">Player two keybinds.</param>
    private void SetPlayerKeybinds(PlayerKeybinds p1, PlayerKeybinds p2)
    {
        this.playerOneKeybinds = p1;
        this.playerTwoKeybinds = p2;
    }
}
