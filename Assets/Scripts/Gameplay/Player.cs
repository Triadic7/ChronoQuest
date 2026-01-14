using System;
using UnityEngine;

/// <summary>
/// Handles player getting damaged.
/// </summary>
public class Player : MonoBehaviour, IHealth
{
    /// <summary>
    /// The max health.
    /// </summary>
    [SerializeField]
    private int maxHealth;

    /// <summary>
    /// If the player can be damaged.
    /// </summary>
    private bool canBeDamaged;

    /// <summary>
    /// Get or set the health.
    /// </summary>
    public int Health { get => this.maxHealth; set => this.maxHealth = value; }

    /// <summary>
    /// Get or set the current health.
    /// </summary>
    public int CurrentHealth { get; set; }

    /// <summary>
    /// Event thats fired when the player dies.
    /// </summary>
    public event Action OnPlayerDeath;

    /// <summary>
    /// On player takes damage.
    /// </summary>
    public event Action OnTakeDamage;

    /// <summary>
    /// Sets if the player will be hurt.
    /// </summary>
    /// <param name="willHurt">Will allow things to hurt player.</param>
    public void SetHurt(bool willHurt)
    {
        this.canBeDamaged = willHurt;
    }

    /// <summary>
    /// Enables can be hurt.
    /// </summary>
    public void EnableCanBeHurt()
    {
        this.canBeDamaged = true;
    }

    /// <summary>
    /// Disables can be hurt.
    /// </summary>
    public void DisableCanBeHurt()
    {
        this.canBeDamaged = false;
    }

    /// <summary>
    /// Takes damage to player.
    /// </summary>
    /// <param name="damage">The amount of damage.</param>
    public void TakeDamage(int damage)
    {
        if (this.canBeDamaged)
        {
            this.CurrentHealth -= damage;
            Debug.Log($"Player hit. Remaining health: {this.CurrentHealth}/{this.maxHealth}");
            // Call on player death if player is killed.
            if (this.CurrentHealth <= 0)
            {
                this.OnPlayerDeath?.Invoke();
                this.gameObject.SetActive(false);
            }

            ///Call event on damage taken.
            this.OnTakeDamage?.Invoke();
        }
    }

    /// <summary>
    /// Sets player health to max.
    /// </summary>
    private void RespawnPlayer()
    {
        this.CurrentHealth = this.Health;
    }

    /// <summary>
    /// On enable, respawn player health.
    /// </summary>
    private void OnEnable()
    {
        this.RespawnPlayer();
    }
}
