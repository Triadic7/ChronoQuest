using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI manager for handling menus.
/// </summary>
public class UIManager : MonoBehaviour
{
    /// <summary>
    /// Store menus by key.
    /// </summary>
    private Dictionary<string, IMenu> menus = new Dictionary<string, IMenu>();

    /// <summary>
    /// Stack for navigation.
    /// </summary>
    private Stack<IMenu> menuStack = new Stack<IMenu>();

    /// <summary>
    /// Registers a menu.
    /// </summary>
    /// <param name="key">The menu key.</param>
    /// <param name="menu">The menu.</param>
    public void RegisterMenu(string key, IMenu menu)
    {
        if (!menus.ContainsKey(key))
        {
            menus.Add(key, menu);

            // Hides menu right away.
            menu.Close();
            Debug.Log($"UIManager: Menu added to dictionary: {key}");
        }
    }

    /// <summary>
    /// Opens a menu based on key.
    /// </summary>
    /// <param name="key">The menu key to open.</param>
    public void OpenMenu(string key)
    {
        if (!menus.ContainsKey(key))
        {
            return;
        }

        // Hide current menu if any.
        if (menuStack.Count > 0)
        {
            menuStack.Peek().Close();
        }

        // Opens menu based on key.
        IMenu menu = menus[key];
        menu.Open();
        menuStack.Push(menu);
    }

    /// <summary>
    /// Go back by closing menu.
    /// </summary>
    public void Back()
    {
        if (menuStack.Count == 0)
        {
            return;
        }

        // Set current.
        IMenu current = menuStack.Pop();

        // Calls on back.
        current.OnBack();

        // Closes menu.
        current.Close();

        // Opens next menu.
        if (menuStack.Count > 0)
        {
            menuStack.Peek().Open();
        }
    }
}
