using UnityEngine;

/// <summary>
/// Enemy for the space game. Just moves down.
/// </summary>
public class SpaceEnemy : MonoBehaviour
{
    /// <summary>
    /// How fast the enemy moves down.
    /// </summary>
    [SerializeField] 
    private float moveSpeed = 5f;

    /// <summary>
    /// The rigidbody.
    /// </summary>
    private Rigidbody2D rb;

    /// <summary>
    /// Move down on awake.
    /// </summary>
    private void Awake()
    {
        this.rb = GetComponent<Rigidbody2D>();
        this.rb.linearVelocity = Vector2.down * moveSpeed;
    }

    /// <summary>
    /// If hits anything, destroy self. If space statin, recieve failure.
    /// </summary>
    /// <param name="collision">The collision.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Collision name: {collision.gameObject.name}");

        // Try to damage space station.
        SpaceStation spaceStation = collision.GetComponent<SpaceStation>();
        if (spaceStation)
        {
            spaceStation.TakeDamage();
        }

        // Try to damage player.
        Player player = collision.GetComponent<Player>();
        if (player) 
        {
            player.TakeDamage(10);
        }

        Destroy(this.gameObject);
    }
}
