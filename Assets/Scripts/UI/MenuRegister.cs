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

    private void Awake()
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.OnGameInitialized += RegisterMenu;
        }
        else
        {
            Debug.LogError("GameManager Instance not found.");
        }
    }

    /// <summary>
    /// Register menu after game is initalized.
    /// </summary>
    private void RegisterMenu()
    {
        // Get menu from gameobject.
        Menu menu = GetComponent<Menu>();
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
            Debug.LogError("No menu found.");
        }
    }
}
