using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Creates an asset menu for storing all the tiles.
/// </summary>
[CreateAssetMenu(menuName = "Tiles/Tile Registry")]
public class TileRegistry : ScriptableObject
{
    /// <summary>
    /// All tiles available to the game.
    /// </summary>
    public List<TileBase> Tiles;
}