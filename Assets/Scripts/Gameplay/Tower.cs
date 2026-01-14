using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tower for the europe stage. 
/// If enemy gets close, take damage.
/// </summary>
public class Tower : MonoBehaviour, IHealth
{
    /// <summary>
    /// The max health.
    /// </summary>
    [SerializeField]
    private int maxHealth;

    /// <summary>
    /// The max health.
    /// </summary>
    public int Health { get => this.maxHealth; set => this.maxHealth = value; }

    /// <summary>
    /// The current health.
    /// </summary>
    public int CurrentHealth { get; set; }

    /// <summary>
    /// Called when an enemy gets through the health.
    /// </summary>
    public event Action OnDamageTaken;

    /// <summary>
    /// Event that gets fired on any damage taken.
    /// </summary>
    public event Action OnTakeDamage;

    /// <summary>
    /// Takes damage and invokes on damage taken when health is 0..
    /// </summary>
    /// <param name="damage">The amount of damage.</param>
    public void TakeDamage(int damage)
    {
        this.CurrentHealth -= damage;

        // If health is less than 0, take damage.
        if (this.CurrentHealth <= 0) 
        {
            this.CurrentHealth = 0;
            this.OnDamageTaken?.Invoke();
        }

        this.OnTakeDamage?.Invoke();
    }

    /// <summary>
    /// On enable, heal to full.
    /// </summary>
    private void OnEnable()
    {
        this.CurrentHealth = this.Health;
    }
}

