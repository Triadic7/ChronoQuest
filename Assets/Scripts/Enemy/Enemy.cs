using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    /// <summary>
    /// The health of the boss.
    /// </summary>
    public int Health { get; set; } = 100;

    /// <summary>
    /// Event thats fired whenever the boss takes damage.
    /// </summary>
    public event Action OnTakeDamage;

    [Header("SFX")]
    [SerializeField]
    private AudioClip defeatedSfx;

    /// <summary>
    /// The current health.
    /// </summary>
    public int CurrentHealth { get; set; }

    /// <summary>
    /// Take damage, and fire event.
    /// </summary>
    /// <param name="damage">The amount of damage.</param>
    /// <returns>Returns true if damage killed boss.</returns>
    public void TakeDamage(int damage)
    {
        // Check for death.
        this.CurrentHealth -= damage;
        if (this.CurrentHealth < 0)
        {
            this.CurrentHealth = 0;
            this.Defeated();
        }

        // Fire event.
        this.OnTakeDamage?.Invoke();
    }

    /// <summary>
    /// On defeated destroy object.
    /// </summary>
    public void Defeated()
    {
        Debug.Log("Enemy defeated.");

        if (this.defeatedSfx != null)
        {
            SoundManager.Instance?.Play(this.defeatedSfx);
        }

        Destroy(gameObject);
    }
}
