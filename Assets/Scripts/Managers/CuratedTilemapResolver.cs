using System.Collections.Generic;
using UnityEngine.Tilemaps;

/// <summary>
/// Turns keys into tilemaps.
/// </summary>
public class CuratedTilemapResolver
{
    /// <summary>
    /// Dictionary with all tilemaps using keys to get them.
    /// </summary>
    private readonly Dictionary<string, Tilemap> tilemaps;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="entries">The entries to add to tilemaps.</param>
    public CuratedTilemapResolver(List<CuratedTilemapEntry> entries)
    {
        this.tilemaps = new Dictionary<string, Tilemap>();

        // Add entries into tilemaps.
        foreach (var entry in entries)
        {
            if (!this.tilemaps.ContainsKey(entry.Key))
            {
                this.tilemaps.Add(entry.Key, entry.Tilemap);
            }
        }
    }

    /// <summary>
    /// Tries to get tilemap using a key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="tilemap">The tilemap if found.</param>
    /// <returns>Returns true if tilemap found.</returns>
    public bool TryGet(string key, out Tilemap tilemap)
    {
        return this.tilemaps.TryGetValue(key, out tilemap);
    }
}
