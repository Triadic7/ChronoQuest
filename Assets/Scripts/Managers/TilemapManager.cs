using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    /// <summary>
    /// How wide the tilemap is.
    /// </summary>
    [SerializeField]
    private int width;

    /// <summary>
    /// How tall the tilemap is.
    /// </summary>
    [SerializeField] 
    private int height;

    /// <summary>
    /// Chance from 0 to 1 that a foreground tile will spawn at each position.
    /// For example, 0.5 means each tile position has a 50% chance of having a foreground tile.
    /// </summary>
    [SerializeField, Range(0f, 1f)]
    private float foregroundSpawnChance = 0.5f;

    /// <summary>
    /// The background tilemap.
    /// </summary>
    private Tilemap backgroundTilemap;

    /// <summary>
    /// The foreground tilemap.
    /// </summary>
    private Tilemap foregroundTilemap;

    /// <summary>
    /// Caches tilemaps and adds event listeners.
    /// </summary>
    private void Awake()
    {
        Transform gridChild = this.transform.GetChild(0);

        // Get tilemaps from children.
        this.backgroundTilemap = gridChild.GetChild(0).GetComponent<Tilemap>();
        this.foregroundTilemap = gridChild.GetChild(1).GetComponent<Tilemap>();

        // Add event listners to gamemanager.
        GameManager gm = GameManager.Instance;

        if(gm == null)
        {
            Debug.LogError("No GameManager found.");
            return;
        }

        // On stage selected, display the tilemap to go with it.
        gm.OnLocationSelected += (location) =>
        {
            this.ClearTilemap();

            if (location.UsesCuratedTilemap && AssetLoader.CuratedResolver.TryGet(location.CuratedBackgroundTilemap, out Tilemap curated))
            {
                this.CopyTilemap(curated, backgroundTilemap);
            }
            else
            {
                DisplayTilemap(location.BackgroundTiles, location.ForegroundTiles);
            }
        };

        // On game end, clear tilemaps.
        gm.OnStageEnd += () => this.ClearTilemap();

        // Clear tilemap on going to main menu.
        gm.OnMainMenu += () => this.ClearTilemap();

    }

    /// <summary>
    /// Copies tilemap and displays it.
    /// </summary>
    /// <param name="tilemap">The tilemap that will be copied.</param>
    /// <param name="target">The tilemap that will be set.</param>
    private void CopyTilemap(Tilemap tilemap, Tilemap target)
    {
        BoundsInt bounds = tilemap.cellBounds;

        // Find bottom left occupied tile.
        Vector3Int min = bounds.min;

        // Set all tiles in tilemap on the targer tilemap.
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile != null)
            {
                // Shift so bottom left starts at 0,0. This is done because otherwise the tilemap goes bottom left.
                Vector3Int targetPos = pos - min;
                target.SetTile(targetPos, tile);
            }
        }
    }


    /// <summary>
    /// Displays the tilemap with the given arguements.
    /// </summary>
    /// <param name="background">The background tilemap.</param>
    /// <param name="foreground">The foreground tilemap.</param>
    private void DisplayTilemap(List<TileBase> background, List<TileBase> foreground)
    {
        for (int i = 0; i < this.width; i++) 
        {
            for(int j = 0; j < this.height; j++)
            {
                // Get position to spawn in tile.
                Vector3Int tilePos = new Vector3Int(i, j, 0);

                // Spawn background tile at pos.
                this.backgroundTilemap.SetTile(tilePos, background[Random.Range(0, background.Count)]);

                // Spawn foreground tile based on chance.
                if (foreground.Count > 0 && Random.value <= this.foregroundSpawnChance)
                {
                    TileBase tile = foreground[Random.Range(0, foreground.Count)];

                    if(tile != null)
                    {
                        this.foregroundTilemap.SetTile(tilePos, tile);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Resets the tilemaps.
    /// </summary>
    private void ClearTilemap()
    {
        this.backgroundTilemap.ClearAllTiles();
        this.foregroundTilemap.ClearAllTiles();
    }
}
