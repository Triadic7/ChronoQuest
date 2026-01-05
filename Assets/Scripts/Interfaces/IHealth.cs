
/// <summary>
/// Interface for things that can take damage.
/// </summary>
public interface IHealth
{
    /// <summary>
    /// How much health the object has.
    /// </summary>
    int Health { get; set; }

    /// <summary>
    /// The health currently.
    /// </summary>
    int CurrentHealth { get; set; }

    /// <summary>
    /// Take damage.
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    void TakeDamage(int damage);
}
