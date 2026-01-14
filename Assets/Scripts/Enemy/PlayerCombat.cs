using UnityEngine;
using System;

/// <summary>
/// Allows the player to damage enemies if walking into them.
/// </summary>
public class PlayerCombat : MonoBehaviour
{
    /// <summary>
    /// Called when an enemy impacts station.
    /// </summary>
    public event Action OnDestroyEnemy;

    /// <summary>
    /// Allows player to harm enemies.
    /// </summary>
    private bool canHurtEnemies;

    /// <summary>
    /// Allows players to harm enemies.
    /// </summary>
    public void EnableHurtEnemies()
    {
        this.canHurtEnemies = true;
    }

    /// <summary>
    /// Removes players option to harm enemies.
    /// </summary>
    public void DisableHurtEnemies()
    {
        this.canHurtEnemies = false;
    }

    /// <summary>
    /// If player enters enemy collider, destroy it.
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerEnter2D(Collider2D collider)
    {
        Enemy enemy = collider.gameObject.GetComponent<Enemy>();
        if(enemy != null && this.canHurtEnemies)
        {
            enemy.TakeDamage(1000);
            this.OnDestroyEnemy?.Invoke();
        }
    }
}
