using System;
using UnityEngine;

/// <summary>
/// Script for the space station for the space minigame.
/// </summary>
public class SpaceStation : MonoBehaviour
{
    /// <summary>
    /// Called when an enemy impacts station.
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
