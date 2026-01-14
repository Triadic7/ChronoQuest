using UnityEngine;

/// <summary>
/// Damages IHealth objects on hit.
/// </summary>
public class DamageOnHit : MonoBehaviour
{
    /// <summary>
    /// The damage done.
    /// </summary>
    [SerializeField] 
    private int damage = 10;

    /// <summary>
    /// If the object should destroy self on contact.
    /// </summary>
    [SerializeField] 
    private bool destroySelf = true;

    /// <summary>
    /// On trigger enter, try to damage any IHealth objects.
    /// </summary>
    /// <param name="other">The colision.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Try to get health component.
        if (other.TryGetComponent<IHealth>(out var health))
        {
            this.CheckForDamage(health);
        }

        // Destroy self if set.
        if (this.destroySelf)
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Checks if close enough to do damage.
    /// </summary>
    /// <param name="health">The health.</param>
    private void CheckForDamage(IHealth health)
    {
        if (health != null) 
        {
            health.TakeDamage(this.damage);
        }

    }
}
