using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

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
    /// The transition between scenes.
    /// </summary>
    private FadeSlideTransition transition;

    /// <summary>
    /// Registers a menu.
    /// </summary>
    /// <param name="key">The menu key.</param>
    /// <param name="menu">The menu.</param>
    public void RegisterMenu(string key, Menu menu)
    {
        Debug.Log($"Added menu {key}");
        if (!this.menus.ContainsKey(key))
        {
            this.menus.Add(key, menu);

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
        if (!this.menus.ContainsKey(key))
        {
            Debug.LogWarning($"Menu key not found: {key}.");
            return;
        }

        // Get menu from key.
        Menu menu = menus[key];

        // Start transition.
        if (this.transition != null)
        {
            // Wait for transition animation before opening menu.
            this.StartCoroutine(OpenMenuWithTransition(menu));
        }
        else
        {
            this.OpenMenuImmediate(menu);
        }
    }

    /// <summary>
    /// Go back by closing menu.
    /// </summary>
    public void Back()
    {
        if (this.menuStack.Count == 0)
        {
            return;
        }

        // Set current.
        Menu current = this.menuStack.Pop();

        // Closes menu.
        current.Close();

        // Opens next menu.
        if (this.menuStack.Count > 0)
        {
            this.menuStack.Peek().Open();
        }
    }

    /// <summary>
    /// Closes a menu by key, if it exists.
    /// </summary>
    /// <param name="key">The menu key to close.</param>
    public void CloseMenu(string key)
    {
        if (!this.menus.ContainsKey(key))
        {
            Debug.LogWarning($"Menu key not found: {key}.");
            return;
        }

        Menu menuToClose = this.menus[key];

        // Remove from stack if present.
        Stack<Menu> tempStack = new Stack<Menu>();
        while (this.menuStack.Count > 0)
        {
            Menu top = this.menuStack.Pop();
            if (top != menuToClose)
            {
                tempStack.Push(top);
            }
        }
        while (tempStack.Count > 0)
        {
            this.menuStack.Push(tempStack.Pop());
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
                this.menus.Add(reg.MenuKey, menu);
            }
        }

        this.transition = FindFirstObjectByType<FadeSlideTransition>();
        if(this.transition == null)
        {
            Debug.LogError("No fade transition found.");
            return;
        }
    }

    /// <summary>
    /// Closes menus.
    /// </summary>
    private void Start()
    {
        foreach (var menu in this.menus.Values)
        {
            menu.Close();
        }
    }

    /// <summary>
    /// Opens a menu after transition plays.
    /// </summary>
    /// <param name="menu">The menu to open.</param>
    /// <returns>Returns nothing.</returns>
    private IEnumerator OpenMenuWithTransition(Menu menu)
    {
        // Play fade + slide.
        this.transition.PlayTopToBottomTransition();

        // Wait for the transition duration.
        yield return null;

        // Open the menu.
        OpenMenuImmediate(menu);

    }

    /// <summary>
    /// Opens menu right away
    /// </summary>
    /// <param name="menu">The menu to open.</param>
    private void OpenMenuImmediate(Menu menu)
    {
        // Dont open same menu.
        if (this.menuStack.Count > 0 && this.menuStack.Peek() == menu)
        {
            return;
        }

        // If stack is greater than 0, close top menu.
        if (this.menuStack.Count > 0)
        {
            this.menuStack.Peek().Close();
        }

        // Open menu and push to stack.
        menu.Open();
        this.menuStack.Push(menu);
    }

}
