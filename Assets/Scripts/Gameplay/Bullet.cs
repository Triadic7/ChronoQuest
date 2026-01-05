using UnityEngine;

/// <summary>
/// Bullet for the shoot controller.
/// </summary>
public class Bullet : MonoBehaviour
{
    /// <summary>
    /// Destroys on collision.
    /// </summary>
    /// <param name="collision">The collision.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        BossEnemy enemy = collision.gameObject.GetComponent<BossEnemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(1);
        }

        // Destroy bullet on any hit.
        Destroy(gameObject);
    }
}
