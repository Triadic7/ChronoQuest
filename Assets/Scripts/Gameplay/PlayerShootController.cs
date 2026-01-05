using UnityEngine;

/// <summary>
/// Shoots up while able to fire.
/// </summary>
public class PlayerShootController : MonoBehaviour
{
    /// <summary>
    /// Bullet prefab.
    /// </summary>
    [SerializeField] 
    private GameObject bulletPrefab;

    /// <summary>
    /// How fast the player shoots.
    /// </summary>
    [SerializeField] 
    private float shootRate = 0.2f;

    /// <summary>
    /// How fast the bullets are.
    /// </summary>
    [SerializeField] 
    private float bulletSpeed = 10f;

    /// <summary>
    /// Timer for shooting.
    /// </summary>
    private float shootTimer;

    /// <summary>
    /// If the player can fire.
    /// </summary>
    private bool canFire;

    /// <summary>
    /// Cache event to enable firing.
    /// </summary>
    private void Start()
    {
        this.canFire = false;

        GameStageManager stageManager = GameManager.Instance.GameStageManager;
        if (stageManager == null)
        {
            Debug.LogError("No StageManager found.");
            return;
        }

        // Allow firing on the final stage.
        stageManager.OnGameSet += (game) =>
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
        };

        // Prevent firing.
        stageManager.OnStageFinished += (_) =>
        {
            this.canFire = false;
        };
    }

    /// <summary>
    /// Shoots bullets over time.
    /// </summary>
    private void Update()
    {
        if (this.canFire)
        {
            // Increment timer.
            this.shootTimer += Time.deltaTime;

            // Check if it's time to shoot.
            if (this.shootTimer >= this.shootRate)
            {
                this.Shoot();
                this.shootTimer = 0f;
            }
        }
    }

    // Shoot a bullet.
    private void Shoot()
    {
        if (this.bulletPrefab == null)
        {
            return;
        }

        // Spawn bullet at player's position.
        GameObject bullet = Instantiate(this.bulletPrefab, this.transform.position, Quaternion.identity);

        // Give it upward velocity.
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.up * this.bulletSpeed;
        }
    }
}
