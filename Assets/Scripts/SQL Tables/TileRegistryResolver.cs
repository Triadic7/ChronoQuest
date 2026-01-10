using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Parses strings into tiles.
/// </summary>
public class TileRegistryResolver
{
    /// <summary>
    /// The tile name key, and the tilebase value.
    /// </summary>
    private readonly Dictionary<string, TileBase> lookup;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="tiles">The list of tiles.</param>
    public TileRegistryResolver(IEnumerable<TileBase> tiles)
    {
        // Sets lookup and its values.
        this.lookup = new Dictionary<string, TileBase>();
        foreach (var tile in tiles)
        {
            this.lookup[tile.name.ToLower()] = tile;
        }
    }

    /// <summary>
    /// Tries to parse string into tilebase.
    /// </summary>
    /// <param name="name">The tile name.</param>
    /// <param name="tile">The tilebase.</param>
    /// <returns>Returns true if parsed successfully.</returns>
    public bool TryResolve(string name, out TileBase tile)
    {
        return this.lookup.TryGetValue(name.Trim().ToLower(), out tile);
    }
}
