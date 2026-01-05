using System;
using UnityEngine;

public class Tower : MonoBehaviour
{
    /// <summary>
    /// Called when an enemy touches the tower.
    /// </summary>
    public event Action OnDamageTaken;

    /// <summary>
    /// Take damage and fire event.
    /// </summary>
    public void TakeDamage()
    {
        this.OnDamageTaken?.Invoke();
    }
}
