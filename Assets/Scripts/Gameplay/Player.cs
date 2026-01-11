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
    /// The health bar game object.
    /// </summary>
    [SerializeField]
    private GameObject healthbar;

    /// <summary>
    /// The health bar rect transform.
    /// </summary>
    [SerializeField] 
    private RectTransform healthForeground;

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
    /// Takes damage to player.
    /// </summary>
    /// <param name="damage">The amount of damage.</param>
    public void TakeDamage(int damage)
    {
        this.CurrentHealth -= damage;
        Debug.Log($"Player hit. Remaining health: {this.CurrentHealth}/{this.maxHealth}");
        this.UpdateHealthBar();
        // Call on player death if player is killed.
        if (this.CurrentHealth <= 0)
        {
            this.OnPlayerDeath?.Invoke();
            this.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Updates the green foreground based on health.
    /// </summary>
    private void UpdateHealthBar()
    {
        if (this.healthForeground != null)
        {
            // Show if health less than max.
            if (this.CurrentHealth < this.maxHealth)
            {
                this.healthbar.SetActive(true);
                float percent = (float)this.CurrentHealth / (float)this.maxHealth;
                this.healthForeground.localScale = new Vector3(percent, 1f, 1f);
            }
            else
            {
                // Hide if full health.
                this.healthbar.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Sets player health to max.
    /// </summary>
    private void Start()
    {
        if (this.healthForeground == null)
        {
            Debug.LogError("Health bar not assigned.");
        }

        this.RespawnPlayer();
    }

    /// <summary>
    /// Sets player health to max.
    /// </summary>
    private void RespawnPlayer()
    {
        this.CurrentHealth = this.Health;
        this.UpdateHealthBar();
    }
}
