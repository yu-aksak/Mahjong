using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Object = UnityEngine.Object;

[Serializable]
public class Tile
{
    public event Action<sbyte> BlockerCountChanged;
    public event Action<bool> IsBlockedChanged;
    public event Action Removed;
    
    private bool isSelected;
    private bool isBlocked;
    private sbyte blockerCount;

    [JsonIgnore] public TileView view;

    public bool IsBlocked
    {
        get
        {
            return isBlocked;
        }
        set
        {
            if (isBlocked != value)
            {
                isBlocked = value;
                IsBlockedChanged?.Invoke(value);
            }
        }
    }

    [JsonIgnore]
    public bool IsSelected
    {
        get
        {
            return isSelected;
        }
        private set
        {
            if (isSelected != value)
            {
                isSelected = value;
                OnIsSelectedChanged(IsSelected);
            }
        }
    }

    public sbyte BlockerCount
    {
        get
        {
            return blockerCount;
        }
        set
        {
            if (blockerCount != value)
            {
                blockerCount = value;
                BlockerCountChanged?.Invoke(value);
                IsBlocked = blockerCount > 1;
            }
        }
    }

    [JsonIgnore] public sbyte Id { get; set; } = -1;
    public TileLayerId LayerId { get; }

    public Tile(TileLayerId layerId)
    {
        LayerId = layerId;
    }

    public void StartSettingId()
    {
        BlockerCountChanged = null;
        IsBlockedChanged = null;
        Removed = null;
        var realIsBlocked = isBlocked;
        var realBlockerCount = blockerCount;

        Removed += StopSettingId;
        
        void StopSettingId()
        {
            isBlocked = realIsBlocked;
            blockerCount = realBlockerCount;
        }
    }

    public void Hint()
    {
        view.Hint(OnHintTimeout);
    }

    public void Select()
    {
        IsSelected = true;
    }

    public void Deselect()
    {
        IsSelected = false;
    }
    
    public void Add(in TileMapConfig config)
    {
        config.tiles.Add(LayerId.ToKey(), this);
        UpdateBlockerCount(config, true);
    }

    public void Remove(in TileMapConfig config)
    {
        OnRemove(config);
        Removed?.Invoke();
    }

    private void OnRemove(in TileMapConfig config)
    {
        config.tiles.Remove(LayerId.ToKey());
        UpdateBlockerCount(config, false);
    }

    private void OnIsSelectedChanged(bool isSelected)
    {
        view.SetBySelectionState(isSelected);
    }

    private void OnHintTimeout()
    {
        if (IsSelected)
        {
            view.Select();
        }
        else
        {
            view.Deselect();
        }
    }

    public void Destroy()
    {
        view.OnDestoy();
        Object.Destroy(view.GameObject);
    }

    public void GetBlockers(HashSet<sbyte> blockers, in TileMapConfig config)
    {
        blockers.Clear();
        var maxLayerId = config.maxLayerId;
        var layerId = LayerId;
        var newLayerId = LayerId * 2;
        var gridSizes = config.gridSizes;
        var gridSize = gridSizes[layerId.z] * 2;
        var tiles = config.tiles;
        var offset = new TileLayerId(1, 0, 0);
        
        var layerIdOffset = default(short);
        
        offset.z = 2;

        GridSize neighbourLayer;

        int xOffset;
        int xFactor;
        int yOffset;
        int yFactor;

        sbyte positiveXOffset;
        sbyte negativeXOffset;
        sbyte positiveYOffset;
        sbyte negativeYOffset;
        
        var neighbourLayerGridSizeIndex = layerId.z - 1;

        if (CheckNeighbourLayerGridSizeIndexOutOfRange())
        {
            UpdateOffsetsAndFactors();
            OnAdd();
        }
        else
        {
            OnAdd();
        }

        void OnAdd()
        {
            neighbourLayerGridSizeIndex = layerId.z + 1;
                
            if (CheckNeighbourLayerGridSizeIndexOutOfRange())
            {
                offset.z *= -1;
                UpdateOffsetsAndFactors();
                CheckOnOtherLayer(CheckBlockers);
            }
        }
        
        void UpdateOffsetsAndFactors()
        {
            neighbourLayer = gridSizes[neighbourLayerGridSizeIndex] * 2;

            xOffset = (gridSize.x - neighbourLayer.x) / 2;
            xFactor = 1 * (xOffset & int.MaxValue) % 2;
            
            yOffset = (gridSize.y - neighbourLayer.y) / 2;
            yFactor =  1 * (yOffset & int.MaxValue) % 2;
                
            positiveXOffset = (sbyte)(xOffset + xFactor);
            negativeXOffset = (sbyte)(xOffset - xFactor);
            positiveYOffset = (sbyte)(yOffset + yFactor);
            negativeYOffset = (sbyte)(yOffset - yFactor);
        }

        void CheckOnOtherLayer(Action checkAction)
        {
            if (xFactor + yFactor == 0)
            {
                offset.x = positiveXOffset;
                offset.y = positiveYOffset;
                layerIdOffset = ((newLayerId - offset) / 2).ToKey();
                checkAction();
                return;
            }
                
            if (xFactor + yFactor > 0)
            {
                offset.x = positiveXOffset;
                offset.y = positiveYOffset;
                layerIdOffset = ((newLayerId - offset) / 2).ToKey();
                checkAction();
            
                offset.x = negativeXOffset;
                offset.y = negativeYOffset;
                layerIdOffset = ((newLayerId - offset) / 2).ToKey();
                checkAction();
            }
            
            if (xFactor + yFactor > 1)
            {
                offset.x = positiveXOffset;
                offset.y = negativeYOffset;
                layerIdOffset = ((newLayerId - offset) / 2).ToKey();
                checkAction();
            
                offset.x = negativeXOffset;
                offset.y = positiveYOffset;
                layerIdOffset = ((newLayerId - offset) / 2).ToKey();
                checkAction();
            }
        }
        
        bool CheckNeighbourLayerGridSizeIndexOutOfRange()
        {
            return neighbourLayerGridSizeIndex >= 0 && neighbourLayerGridSizeIndex < maxLayerId;
        }

        void CheckBlockers()
        {
            if (tiles.TryGetValue(layerIdOffset, out var id))
            {
                blockers.Add(id.Id);
            }
        }
    }

    private void UpdateBlockerCount(in TileMapConfig config, in bool isAdding)
    {
        var factor = isAdding ? 1 : -1;
        var maxLayerId = config.maxLayerId;
        var layerId = LayerId;
        var newLayerId = LayerId * 2;
        var gridSizes = config.gridSizes;
        var gridSize = gridSizes[layerId.z] * 2;
        var tiles = config.tiles;
        var blockWeight = (sbyte)factor;
        var offset = new TileLayerId(1, 0, 0);
        
        var layerIdOffset = (layerId + offset).ToKey();
        CheckBlockers();
        CheckBlocked();
        
        layerIdOffset = (layerId - offset).ToKey();
        CheckBlockers();
        CheckBlocked();

        blockWeight *= 2;
        offset.z = 2;

        GridSize neighbourLayer;

        int xOffset;
        int xFactor;
        int yOffset;
        int yFactor;

        sbyte positiveXOffset;
        sbyte negativeXOffset;
        sbyte positiveYOffset;
        sbyte negativeYOffset;
        
        var neighbourLayerGridSizeIndex = layerId.z - 1;

        if (CheckNeighbourLayerGridSizeIndexOutOfRange())
        {
            UpdateOffsetsAndFactors();
            CheckOnOtherLayer(CheckBlocked);
            
            if (isAdding)
            {
                OnAdd();
            }
        }
        else if (isAdding)
        {
            OnAdd();
        }

        void OnAdd()
        {
            neighbourLayerGridSizeIndex = layerId.z + 1;
                
            if (CheckNeighbourLayerGridSizeIndexOutOfRange())
            {
                offset.z *= -1;
                UpdateOffsetsAndFactors();
                CheckOnOtherLayer(CheckBlockers);
            }
        }
        
        void UpdateOffsetsAndFactors()
        {
            neighbourLayer = gridSizes[neighbourLayerGridSizeIndex] * 2;

            xOffset = (gridSize.x - neighbourLayer.x) / 2;
            xFactor = 1 * (xOffset & int.MaxValue) % 2;
            
            yOffset = (gridSize.y - neighbourLayer.y) / 2;
            yFactor =  1 * (yOffset & int.MaxValue) % 2;
                
            positiveXOffset = (sbyte)(xOffset + xFactor);
            negativeXOffset = (sbyte)(xOffset - xFactor);
            positiveYOffset = (sbyte)(yOffset + yFactor);
            negativeYOffset = (sbyte)(yOffset - yFactor);
        }

        void CheckOnOtherLayer(Action checkAction)
        {
            if (xFactor + yFactor == 0)
            {
                offset.x = positiveXOffset;
                offset.y = positiveYOffset;
                layerIdOffset = ((newLayerId - offset) / 2).ToKey();
                checkAction();
                return;
            }
                
            if (xFactor + yFactor > 0)
            {
                offset.x = positiveXOffset;
                offset.y = positiveYOffset;
                layerIdOffset = ((newLayerId - offset) / 2).ToKey();
                checkAction();
            
                offset.x = negativeXOffset;
                offset.y = negativeYOffset;
                layerIdOffset = ((newLayerId - offset) / 2).ToKey();
                checkAction();
            }
            
            if (xFactor + yFactor > 1)
            {
                offset.x = positiveXOffset;
                offset.y = negativeYOffset;
                layerIdOffset = ((newLayerId - offset) / 2).ToKey();
                checkAction();
            
                offset.x = negativeXOffset;
                offset.y = positiveYOffset;
                layerIdOffset = ((newLayerId - offset) / 2).ToKey();
                checkAction();
            }
        }
        
        bool CheckNeighbourLayerGridSizeIndexOutOfRange()
        {
            return neighbourLayerGridSizeIndex >= 0 && neighbourLayerGridSizeIndex < maxLayerId;
        }
        
        void CheckBlocked()
        {
            if (tiles.TryGetValue(layerIdOffset, out var id))
            {
                id.BlockerCount += blockWeight;
            }
        }
        
        void CheckBlockers()
        {
            if (tiles.TryGetValue(layerIdOffset, out _))
            {
                BlockerCount += blockWeight;
            }
        }
    }
}
