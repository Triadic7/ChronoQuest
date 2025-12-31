using UnityEngine;

public class LoadMenu : MonoBehaviour, IMenu
{
    /// <summary>
    /// Hides load menu.
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
    /// Shows load menu.
    /// </summary>
    public void Open()
    {
        this.gameObject.SetActive(true);
    }
}
