using UnityEngine;

/// <summary>
/// Menu for adding to UIManager.
/// </summary>
public class MenuRegister : MonoBehaviour
{
    /// <summary>
    /// Key for the menu.
    /// </summary>
    [SerializeField] private string menuKey;

    private void Start()
    {
        GameManager.Instance.OnGameInitialized += RegisterMenu;
    }

    /// <summary>
    /// Register menu after game is initalized.
    /// </summary>
    private void RegisterMenu()
    {
        // Get menu from gameobject.
        IMenu menu = GetComponent<IMenu>();
        if (menu != null)
        {
            // Get UIManager and subscribe to event.
            UIManager uiManager = GameManager.Instance.UIManager;
            Debug.Log($"Menu registered: {menuKey} ({gameObject.name})");
            if (uiManager != null)
            {
                uiManager.RegisterMenu(menuKey, menu);
            }
        }
        else
        {
            Debug.LogError("No menu interface found.");
        }
    }
}
