using UnityEngine.InputSystem;
using System.Text.RegularExpressions;

/// <summary>
/// Extension for input keys.
/// </summary>
public static class KeyExtensions
{
    /// <summary>
    /// Puts spaces between uppercased key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>Returns string of space seperated keyword.</returns>
    public static string PrettyName(this Key key)
    {
        string name = key.ToString();
        return Regex.Replace(name, "(\\B[A-Z])", " $1");
    }
}
