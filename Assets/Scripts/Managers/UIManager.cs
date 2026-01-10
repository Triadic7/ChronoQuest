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
    private Dictionary<string, Menu> menus = new Dictionary<string, Menu>();

    /// <summary>
    /// Stack for navigation.
    /// </summary>
    private Stack<Menu> menuStack = new Stack<Menu>();

    /// <summary>
    /// Registers a menu.
    /// </summary>
    /// <param name="key">The menu key.</param>
    /// <param name="menu">The menu.</param>
    public void RegisterMenu(string key, Menu menu)
    {
        if (!menus.ContainsKey(key))
        {
            menus.Add(key, menu);

            // Hides menu right away.
            menu.Close();
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
            Debug.LogWarning($"Menu key not found: {key}.");
            return;
        }

        Menu menu = menus[key];

        // Don't reopen if already on top.
        if (menuStack.Count > 0 && menuStack.Peek() == menu)
        {
            return;
        }

        // Hide current menu if any.
        if (menuStack.Count > 0)
        {
            menuStack.Peek().Close();
        }

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
        Menu current = menuStack.Pop();

        // Closes menu.
        current.Close();

        // Opens next menu.
        if (menuStack.Count > 0)
        {
            menuStack.Peek().Open();
        }
    }

    /// <summary>
    /// Closes a menu by key, if it exists.
    /// </summary>
    /// <param name="key">The menu key to close.</param>
    public void CloseMenu(string key)
    {
        if (!menus.ContainsKey(key))
        {
            Debug.LogWarning($"Menu key not found: {key}.");
            return;
        }

        Menu menuToClose = menus[key];

        // Remove from stack if present.
        Stack<Menu> tempStack = new Stack<Menu>();
        while (menuStack.Count > 0)
        {
            Menu top = menuStack.Pop();
            if (top != menuToClose)
            {
                tempStack.Push(top);
            }
        }
        while (tempStack.Count > 0)
        {
            menuStack.Push(tempStack.Pop());
        }

        // Close the menu.
        menuToClose.Close();
    }

    /// <summary>
    /// Get all menus and register them.
    /// </summary>
    private void Awake()
    {
        // Find all MenuRegister components in children, even if inactive.
        MenuRegister[] allRegisters = FindObjectsByType<MenuRegister>(FindObjectsSortMode.None);
        foreach (var reg in allRegisters)
        {
            Menu menu = reg.GetComponent<Menu>();
            if (menu != null && !menus.ContainsKey(reg.MenuKey))
            {
                menus.Add(reg.MenuKey, menu);
                menu.Close();
            }
        }
    }

}
