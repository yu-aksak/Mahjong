public interface ITileSelector
{
    ITileSelector Selector { get; set; }
    void TouchTile(in Tile tile);
    bool TryGetSameTile(Tile tile, out Tile sameTile);
    bool TryGetTwoSameRandomTiles(out (Tile, Tile) tiles);
}
