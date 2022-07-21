using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public struct TileMapConfig
{
    public Dictionary<short, Tile> tiles;
    public List<GridSize> gridSizes;
    public sbyte maxLayerId;
    public int gameDuration;

    public TileMapConfig(Dictionary<short, Tile> tiles, List<GridSize> gridSize, sbyte maxLayerId = 0, int gameDuration = 0)
    {
        this.tiles = tiles;
        gridSizes = gridSize;
        this.maxLayerId = maxLayerId;
        this.gameDuration = gameDuration;
    }

    public TileMapConfig(TileMapConfig config) : this()
    {
        tiles = new Dictionary<short, Tile>(config.tiles);
        gridSizes = new List<GridSize>(config.gridSizes);
        maxLayerId = config.maxLayerId;
        gameDuration = config.gameDuration;
    }

    public TileMapConfig Copy()
    {
        return new TileMapConfig(this);
    }
    
    public void SortTiles()
    {
        var tempTiles = new Dictionary<short, Tile>();

        var sorted = tiles.OrderBy(pair => pair.Value.BlockerCount);
        
        foreach (var pair in sorted)
        {
            tempTiles.Add(pair.Key, pair.Value);
        }
        
        tiles.Clear();
        tiles = tempTiles;
    }
}