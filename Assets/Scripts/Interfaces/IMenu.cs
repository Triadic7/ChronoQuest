/// <summary>
/// Interface for menus.
/// </summary>
public interface IMenu
{
    /// <summary>
    /// Open menu.
    /// </summary>
    void Open();

    /// <summary>
    /// Close menu.
    /// </summary>
    void Close();

    /// <summary>
    /// On back button pressed.
    /// </summary>
    void OnBack();
}
