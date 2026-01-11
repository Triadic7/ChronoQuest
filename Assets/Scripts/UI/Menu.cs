using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

/// <summary>
/// Parent class for menus.
/// </summary>
public abstract class Menu : MonoBehaviour
{
    /// <summary>
    /// The panel to close.
    /// </summary>
    [SerializeField]
    private GameObject panel;

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
        this.panel.SetActive(true);
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
        this.panel.SetActive(false);
    }

    /// <summary>
    /// Assign panel if null and initialize menu state.
    /// </summary>
    protected virtual void Awake()
    {
        // If no panel is assigned, default to this GameObject.
        if (this.panel == null)
        {
            this.panel = this.gameObject;
        }

        // Ensure panel matches current isOpen state.
        this.panel.SetActive(isOpen);
    }
}
