using UnityEngine;

/// <summary>
/// Bullet for the shoot controller.
/// </summary>
public class Bullet : MonoBehaviour
{
    /// <summary>
    /// Particle effect prefab to spawn on impact.
    /// </summary>
    [SerializeField]
    private GameObject impactEffectPrefab;

    /// <summary>
    /// Lifetime of the bullet before it auto-destroys.
    /// </summary>
    [SerializeField]
    private float lifetime = 5f;

    /// <summary>
    /// Destroys bullet after 5 seconds if it didnt hit anything.
    /// </summary>
    private void Start()
    {
        Destroy(this.gameObject, this.lifetime);
    }

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

        // Spawn impact effect at bullet position.
        if (this.impactEffectPrefab != null)
        {
            Instantiate(this.impactEffectPrefab, this.transform.position, Quaternion.identity);
        }

        // Destroy bullet on any hit.
        Destroy(this.gameObject);
    }
}
