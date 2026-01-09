using UnityEngine;

/// <summary>
/// Parent class for menus.
/// </summary>
public abstract class Menu : MonoBehaviour
{
    /// <summary>
    /// If the menu is open.
    /// </summary>
    private bool isOpen = true;

    /// <summary>
    /// Opens menu.
    /// </summary>
    public virtual void Open()
    {
        if (this.isOpen)
        {
            return;
        }
        this.isOpen = true;
        this.gameObject.SetActive(true);
    }

    /// <summary>
    /// Closes menu.
    /// </summary>
    public virtual void Close()
    {
        if (!this.isOpen)
        {
            return;
        }
        this.isOpen = false;
        this.gameObject.SetActive(false);
    }
}
