using UnityEngine;

public class TalkMenu : MonoBehaviour, IMenu
{
    /// <summary>
    /// Hides talk menu.
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
    /// Shows talk menu.
    /// </summary>
    public void Open()
    {
        this.gameObject.SetActive(true);
    }
}
