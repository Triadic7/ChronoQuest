using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Runtime model for location.
/// </summary>
public class LocationModel
{
    /// <summary>
    /// Gets or sets the location id.
    /// </summary>
    public int LocationID { get; set; }

    /// <summary>
    /// Gets or sets location name.
    /// </summary>
    public string LocationName { get; set; }

    /// <summary>
    /// Gets or sets comma seperated list of tile names for the background.
    /// </summary>
    public string BackgroundTileNames { get; set; }

    /// <summary>
    /// Gets or sets comma seperated list of tile names for the foreground.
    /// </summary>
    public string ForegroundTileNames { get; set; }

    /// <summary>
    /// Gets or sets a list of background tiles.
    /// </summary>
    public List<TileBase> BackgroundTiles { get; set; } = new List<TileBase>();

    /// <summary>
    /// Gets or sets a list of foreground tiles.
    /// </summary>
    public List<TileBase> ForegroundTiles { get; set; } = new List<TileBase>();

    /// <summary>
    /// A curated tilemap.
    /// </summary>
    public string CuratedBackgroundTilemap { get; set; }

    /// <summary>
    /// Checks if location uses curated tilemap.
    /// </summary>
    public bool UsesCuratedTilemap => !string.IsNullOrEmpty(this.CuratedBackgroundTilemap);

    public LocationModel(LocationTable table, TileRegistryResolver resolver)
    {
        this.LocationID = table.LocationID;
        this.LocationName = table.LocationName;
        this.CuratedBackgroundTilemap = table.CuratedBackgroundTilemap;

        // If curated tilemap not found, use background tiles.
        if (string.IsNullOrEmpty(this.CuratedBackgroundTilemap))
        {
            // Turns tile names from string into a collection of strings, seperated by comma.
            IEnumerable<string> backgroundNames = string.IsNullOrEmpty(table.BackgroundTileNames) ? new string[0] : table.BackgroundTileNames.Split(',');

            // Tries to parse into tilebase from the collection of strings.
            AddTiles(backgroundNames, this.BackgroundTiles, resolver);
        }

        // Turns tile names from string into a collection of strings, seperated by comma.
        IEnumerable<string> foregroundNames = string.IsNullOrEmpty(table.ForegroundTileNames) ? new string[0] : table.ForegroundTileNames.Split(',');

        // Tries to parse into tilebase from the collection of strings.
        AddTiles(foregroundNames, this.ForegroundTiles, resolver);
    }

    /// <summary>
    /// Adds tiles to it's tile maps during constructor.
    /// </summary>
    /// <param name="names">The names of the tiles.</param>
    /// <param name="target">Which list it should go to.</param>
    /// <param name="resolver">The resolver to parse tiles.</param>
    private static void AddTiles(IEnumerable<string> names, List<TileBase> target, TileRegistryResolver resolver)
    {
        if (names == null)
        {
            return; 
        }

        // Adds tiles if they can be parsed from string into their Unity asset.
        foreach (var name in names)
        {
            // Trim whitespace.
            string cleanName = name.Trim();

            if (resolver.TryResolve(cleanName, out var tile))
            {
                target.Add(tile);
            }
            else
            {
                Debug.LogWarning($"Tile not found in resolver: {cleanName}");
            }
        }
    }
}
