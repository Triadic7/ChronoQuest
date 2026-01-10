using UnityEngine;
using System;

public class PlayerCombat : MonoBehaviour
{
    /// <summary>
    /// Called when an enemy impacts station.
    /// </summary>
    public event Action OnDestroyEnemy;

    public Collider2D hitbox;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Enemy")
        {
            DestroyEnemy(collider);
        }
    }

    public void DestroyEnemy(Collider2D enemyCollider)
    {
        Destroy(enemyCollider.gameObject);

        this.OnDestroyEnemy?.Invoke();
    }
}
