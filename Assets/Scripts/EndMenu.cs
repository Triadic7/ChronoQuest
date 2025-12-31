using UnityEngine;

public class EndMenu : MonoBehaviour, IMenu
{
    /// <summary>
    /// Hides end menu.
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
    /// Shows open menu.
    /// </summary>
    public void Open()
    {
        this.gameObject.SetActive(true);
    }
}
