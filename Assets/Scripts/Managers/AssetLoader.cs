using UnityEngine;

public class AssetLoader : MonoBehaviour
{
    /// <summary>
    /// The tile registry.
    /// </summary>
    [SerializeField]
    private TileRegistry tileRegistry;

    /// <summary>
    /// The curated tilemaps.
    /// </summary>
    [SerializeField]
    private CuratedTilemapRegistry curatedTilemapRegistry;

    /// <summary>
    /// The resolver for parsing tiles.
    /// </summary>
    public static TileRegistryResolver Resolver { get; private set; }

    /// <summary>
    /// The curated tilemap resolver.
    /// </summary>
    public static CuratedTilemapResolver CuratedResolver { get; private set; }

    /// <summary>
    /// Cache resolver.
    /// </summary>
    private void Awake()
    {
        Resolver = new TileRegistryResolver(this.tileRegistry.Tiles);
        CuratedResolver = new CuratedTilemapResolver(this.curatedTilemapRegistry.Tilemaps);
    }
}
