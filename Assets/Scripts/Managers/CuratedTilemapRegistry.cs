using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Creates asset menu for storing tilemaps into the registry.
/// </summary>
[CreateAssetMenu(menuName = "Tiles/Curated Tilemap Registry")]
public class CuratedTilemapRegistry : ScriptableObject
{
    /// <summary>
    /// The tilemaps that are curated.
    /// </summary>
    public List<CuratedTilemapEntry> Tilemaps;
}

/// <summary>
/// Tilemap entry.
/// </summary>
[System.Serializable]
public class CuratedTilemapEntry
{
    /// <summary>
    /// The key.
    /// </summary>
    public string Key;

    /// <summary>
    /// The value.
    /// </summary>
    public Tilemap Tilemap;
}
