using UnityEngine;

public class PauseMenu : MonoBehaviour, IMenu
{
    /// <summary>
    /// Hides pause menu.
    /// </summary>
    public void Close()
    {
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Go back to menu.
    /// </summary>
    public void OnBack() { }

    /// <summary>
    /// Shows pause menu.
    /// </summary>
    public void Open()
    {
        this.gameObject.SetActive(true);
    }
}
