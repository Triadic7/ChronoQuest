/// <summary>
/// SQLite table for location.
/// </summary>
public class LocationTable
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
    /// Gets or sets background tiles.
    /// </summary>
    public string BackgroundTileNames { get; set; }

    /// <summary>
    /// Gets or sets foreground tiles.
    /// </summary>
    public string ForegroundTileNames { get; set; }

    /// <summary>
    /// A curated tilemap.
    /// </summary>
    public string CuratedBackgroundTilemap { get; set; }
}
