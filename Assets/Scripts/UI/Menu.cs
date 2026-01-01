using UnityEngine;

/// <summary>
/// Parent class for menus.
/// </summary>
public abstract class Menu : MonoBehaviour
{
    /// <summary>
    /// Opens menu.
    /// </summary>
    public virtual void Open()
    {
        this.gameObject.SetActive(true);
        Debug.Log($"{name} opened");
    }

    /// <summary>
    /// Closes menu.
    /// </summary>
    public virtual void Close()
    {
        this.gameObject.SetActive(false);
        Debug.Log($"{name} closed");
    }
}
