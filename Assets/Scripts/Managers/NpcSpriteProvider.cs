using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// Provides sprites using addressables for displaying them to th UI.
/// </summary>
public class NpcSpriteProvider
{
    /// <summary>
    /// The path name for key, and the sprite for image.
    /// </summary>
    private Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();

    /// <summary>
    /// Loads a sprite by DB key.
    /// The key should match the sprite's name inside an Addressable sprite sheet.
    /// </summary>
    /// <param name="spriteName">The DB key e.g. "Pharaoh".</param>
    /// <param name="sheetAddress">The Addressable key for the sheet. e.g. "CharactersAndLocations.png".</param>
    /// <param name="onLoaded">Callback with the loaded sprite.</param>
    public void Load(string spriteName, string sheetAddress, Action<Sprite> onLoaded)
    {
        // Return cached sprite if already loaded.
        if (cache.TryGetValue(spriteName, out Sprite cached))
        {
            onLoaded?.Invoke(cached);
            return;
        }

        // Load the sprite sheet.
        Addressables.LoadAssetAsync<Sprite[]>(sheetAddress).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Sprite[] sprites = handle.Result;

                // Find the sprite in the sheet by its name.
                Sprite sprite = Array.Find(sprites, s => s.name == spriteName);

                if (sprite != null)
                {
                    // Cache it.
                    cache[spriteName] = sprite;
                    onLoaded?.Invoke(sprite);
                }
                else
                {
                    Debug.LogWarning($"Sprite '{spriteName}' not found in sheet '{sheetAddress}'.");
                    onLoaded?.Invoke(null);
                }
            }
            else
            {
                Debug.LogWarning($"Failed to load sprite sheet '{sheetAddress}'.");
                onLoaded?.Invoke(null);
            }
        };
    }
}
