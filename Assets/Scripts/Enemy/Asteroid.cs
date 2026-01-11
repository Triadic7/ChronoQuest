using UnityEngine;

/// <summary>
/// Asteroid that moves right, randomizes size, and destroys on impact or timeout.
/// </summary>
public class Asteroid : MonoBehaviour
{
    /// <summary>
    /// Movement speed to the right.
    /// </summary>
    [SerializeField]
    private float moveSpeed = 5f;

    /// <summary>
    /// Minimum scale multiplier.
    /// </summary>
    [SerializeField]
    private float minSize = 0.5f;

    /// <summary>
    /// Maximum scale multiplier.
    /// </summary>
    [SerializeField]
    private float maxSize = 1.5f;

    /// <summary>
    /// Time before the asteroid despawns.
    /// </summary>
    [SerializeField]
    private float lifeTime = 10f;

    /// <summary>
    /// Cached rigidbody.
    /// </summary>
    private Rigidbody2D rb;

    /// <summary>
    /// Cache the rb.
    /// </summary>
    private void Awake()
    {
        this.rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// On start randomize size and move to the right.
    /// </summary>
    private void Start()
    {
        // Randomize size on spawn.
        float randomSize = Random.Range(this.minSize, this.maxSize);
        this.transform.localScale = Vector3.one * randomSize;

        // Move immediately to the right.
        this.rb.linearVelocity = Vector2.right * this.moveSpeed;

        // Destroy after lifetime.
        Destroy(this.gameObject, this.lifeTime);
    }

    /// <summary>
    /// Destroy on collision.
    /// </summary>
    /// <param name="collision">The collision.</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Try to damage player.
        Player player = collision.gameObject.GetComponent<Player>();
        if (player)
        {
            player.TakeDamage(10);
        }

        Destroy(this.gameObject);
    }

    /// <summary>
    /// Destroy on collision.
    /// </summary>
    /// <param name="collision">The collision.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Try to damage player.
        Player player = collision.GetComponent<Player>();
        if (player)
        {
            player.TakeDamage(10);
        }

        Destroy(this.gameObject);
    }
}
