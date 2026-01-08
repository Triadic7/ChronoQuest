using UnityEngine;

public class AssetLoader : MonoBehaviour
{
    /// <summary>
    /// The tile registry.
    /// </summary>
    [SerializeField]
    private TileRegistry tileRegistry;

    /// <summary>
    /// The resolver for parsing tiles.
    /// </summary>
    public static TileRegistryResolver Resolver { get; private set; }

    /// <summary>
    /// Cache resolver.
    /// </summary>
    private void Awake()
    {
        Resolver = new TileRegistryResolver(tileRegistry.Tiles);
    }
}
