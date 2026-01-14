using UnityEngine;

/// <summary>
/// Shoots upwards while the player is allowed to fire.
/// Handles firing rate, bullet speed, and automatically enables/disables shooting based on stage.
/// </summary>
public class PlayerShootController : MonoBehaviour
{
    /// <summary>
    /// Bullet prefab to instantiate when shooting.
    /// </summary>
    [SerializeField]
    private GameObject bulletPrefab;

    /// <summary>
    /// How fast the player can shoot (seconds per shot).
    /// </summary>
    [SerializeField]
    private float shootRate = 0.2f;

    /// <summary>
    /// Speed of the bullets when fired.
    /// </summary>
    [SerializeField]
    private float bulletSpeed = 10f;

    [Header("SFX")]
    [SerializeField]
    private AudioClip shootSfx;

    [SerializeField]
    [Min(0f)]
    private float shootSfxMinInterval = 0.05f;

    private float lastShootSfxTime;

    /// <summary>
    /// Timer to track shooting cooldown.
    /// </summary>
    private float shootTimer;

    /// <summary>
    /// If the player is allowed to fire.
    /// </summary>
    private bool canFire;

    /// <summary>
    /// Reference to the stage manager to subscribe to game events.
    /// </summary>
    private GameStageManager stageManager;

    /// <summary>
    /// Subscribe to stage events as early as possible in Awake.
    /// </summary>
    private void Awake()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogError("No GameManager found.");
            return;
        }

        this.stageManager = gm.GameStageManager;
        if (this.stageManager == null)
        {
            Debug.LogError("No StageManager found.");
            return;
        }

        // Subscribe to OnGameStarted to enable firing for final stage.
        this.stageManager.OnGameStarted += OnGameStartedHandler;

        // Subscribe to objective stop to disable firing immediately.
        this.stageManager.OnObjectiveStop += () => { this.canFire = false; };
    }

    /// <summary>
    /// Check current game state when enabled to avoid missing events.
    /// Ensures canFire is correct if game already started.
    /// </summary>
    private void OnEnable()
    {
        if (this.stageManager != null && this.stageManager.CurrentGame != null)
        {
            Game currentGame = this.stageManager.CurrentGame;
            if (currentGame.CurrentStage != null)
            {
                // Player can fire only if it's the final stage and objective has started.
                this.canFire = currentGame.CurrentStage.IsFinalStage && currentGame.IsObjectiveActive;
            }
        }
    }

    /// <summary>
    /// Handles shooting input over time.
    /// </summary>
    private void Update()
    {
        if (this.canFire == false)
        {
            return;
        }

        // Increment timer for shooting cooldown.
        this.shootTimer += Time.deltaTime;

        // Fire bullet if timer exceeds shoot rate.
        if (this.shootTimer >= this.shootRate)
        {
            this.Shoot();
            this.shootTimer = 0f;
        }
    }

    /// <summary>
    /// Fires a bullet from the player's current position.
    /// </summary>
    private void Shoot()
    {
        if (this.bulletPrefab == null)
        {
            return;
        }

        if (this.shootSfx != null)
        {
            if (Time.time - this.lastShootSfxTime >= this.shootSfxMinInterval)
            {
                SoundManager.Instance?.Play(this.shootSfx);
                this.lastShootSfxTime = Time.time;
            }
        }

        // Instantiate bullet at player position with no rotation.
        GameObject bullet = Instantiate(this.bulletPrefab, this.transform.position, Quaternion.identity);

        // Assign upward velocity if Rigidbody2D exists.
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.up * this.bulletSpeed;
        }
    }

    /// <summary>
    /// Called when the game starts.
    /// Subscribes to objective start to allow firing if final stage.
    /// </summary>
    /// <param name="game">The current game instance.</param>
    private void OnGameStartedHandler(Game game)
    {
        if (game.CurrentStage.IsFinalStage)
        {
            game.OnObjectiveStart += () => 
            { 
                this.canFire = true; 
            };
        }
        else
        {
            this.canFire = false;
        }
    }
}
