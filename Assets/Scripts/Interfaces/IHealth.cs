
using System;

/// <summary>
/// Interface for things that can take damage.
/// </summary>
public interface IHealth
{
    /// <summary>
    /// How much health the object has.
    /// </summary>
    int Health { get; }

    /// <summary>
    /// The health currently.
    /// </summary>
    int CurrentHealth { get; }

    /// <summary>
    /// Take damage.
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    void TakeDamage(int damage);

    /// <summary>
    /// Event that fires whenever this object takes damage.
    /// </summary>
    event Action OnTakeDamage;
}
