using System;
using System.Collections.Generic;
using Core.CurrencyModule;
using Core.PlayerModule;
using Core.SoundModule;
using Core.StoreModule;
using DG.Tweening;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class TileMap : MonoBehaviour, ITileSelector
{
    private class UnblockTilesList : List<Tile>
    {
        private readonly HashSet<Transform> destroyedTilesOrderIds = new HashSet<Transform>();
        
        public void OnTileDestroy(Tile tile)
        {
            destroyedTilesOrderIds.Add(tile.view.Transform);
        }
        
        public bool Contains(Transform tileTransform)
        {
            return destroyedTilesOrderIds.Contains(tileTransform);
        }
    }
    
    [Serializable]
    private struct TileAnimationData
    {
        public float duration;
        public float delay;
    }

    [SerializeField] [Range(0, 1)] private float tileWidthPercent = 1;
    [SerializeField] [Range(0, 1)] private float tileHeightPercent = 1;
    
    [SerializeField] private int hintPrice;
    [SerializeField] private int bombPrice;
    [SerializeField] private int shufflePrice;
    [SerializeField] private int undoPrice;
    
    [SerializeField] private BonusGame bonusGame;
    [SerializeField] private ParticleSystem destroyFx;
    [SerializeField] private SpriteRenderer tilePrefab;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private TileAnimationData tileAnimationData;

    public static event Action<float> CameraSizeChanged;
    public event Action ShuffleAnimationEnded;
    public event Action WasUndo;
    public event Action Matched;
    public event Action Won;
    public event Action OutOfMoves;
    public event Action Lost;
    
    private readonly UnblockTilesList unblockedTiles = new UnblockTilesList();
    private Transform layersParent;
    private Tile selectedTile;
    private Camera cam;
    private Transform[] tilesLayerParents;
    private Action undo;
    private Tile[] allTiles;
    private TileMapConfig config;
    private Vector2 tileSpacing;
    private Bounds[] bounds;
    private bool isStarted;
    private static bool CanSelect => EventSystem.current.IsPointerOverGameObject() == false;
    
    public ITileSelector Selector { get; set; }
    private bool CanShuffle => config.tiles.Count > 3;
    private int ShufflePrice => shufflePrice;
    public Timer.BaseTimer GameTimer { get; private set; }

    private void Awake()
    {
        Debug.Log("[TileMap] Awake");
        Init();
        Build();
        InitAllUnblockedTiles();
        StartBuildAnimation();

        isStarted = true;
    }
    
    private void OnEnable()
    {
        GameWindow.HintButtonClicked += Hint;
    }    
    
    private void OnDisable()
    {
        GameWindow.HintButtonClicked -= Hint;
    }

    private void Update()
    {
        if (CanSelect && Input.GetMouseButtonDown(0))
        {
            var mouseWorldPosition = cam.ScreenToWorldPoint(Input.mousePosition);

            if (TryGetTileByPosition(mouseWorldPosition, out var tile))
            {
                Selector.TouchTile(tile);
            }
        }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.H))
        {
            Hint();
        }
        
        if (Input.GetKeyDown(KeyCode.M))
        {
            MatchTwoSameTiles();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Win();
        }
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            Lose();
        }
        
        if (Input.GetKeyDown(KeyCode.O))
        {
            OutOfMovesWindow.Show();
        }
#endif
    }

    private void Init()
    {
        bonusGame.Init(this);
        tilePrefab.gameObject.SetActive(false);
        cam = Camera.main;
        var item = Store.Config.GetItem(ItemType.Mahjong, ItemSelector.GetSelected(ItemType.Mahjong));
        var resourceData = StoreResourceProvider.GetResourceData<TextAsset>(item.ResourceId).asset;
        config = JsonConvert.DeserializeObject<TileMapConfig>(resourceData.text);
        Selector = this;
        tileSpacing = tilePrefab.bounds.size * new Vector2(tileWidthPercent, tileHeightPercent);
        GameTimer = Timer.CreateCountDown(config.gameDuration).SetId(this);
        GameTimer.Stoped += OnTimeOut;
        CopyTiles();
        GameWindow.SetActiveHints(true);
        
        Matched += CheckGameResult; 
    }

    private void CopyTiles()
    {
        var tiles = config.tiles.Values;
        allTiles = new Tile[tiles.Count];
        var i = 0;
        
        foreach (var tile in tiles)
        {
            allTiles[i] = tile;
            i++;
        }
    }

    private void InitAllUnblockedTiles()
    {
        foreach (var tile in config.tiles.Values)
        {
            tile.IsBlockedChanged += OnIsBlockedChanged;
            tile.Removed += OnRemove;
            OnIsBlockedChanged(tile.IsBlocked);

            void OnRemove()
            {
                tile.IsBlockedChanged -= OnIsBlockedChanged;
                tile.Removed -= OnRemove;
                unblockedTiles.Remove(tile);
                unblockedTiles.OnTileDestroy(tile);
            }

            void OnIsBlockedChanged(bool isBlocked)
            {
                if (isBlocked)
                {
                    unblockedTiles.Remove(tile);
                }
                else
                {
                    unblockedTiles.Add(tile);
                }
                
                tile.view.SetBlockState(isBlocked);
            }
        }
    }

    private void StartBuildAnimation()
    {
        Debug.Log($"[{nameof(TileMap)}] StartBuildAnimation");
        SoundManager.SoundMixer.PlayOneShot(4);
        var newStartPosY = cam.orthographicSize + cam.transform.position.y + tilePrefab.bounds.size.y;
        var i = 0;
        var j = 0;
        var maxLayerId = config.maxLayerId;
        var timer = Timer.CreateCountDown(tileAnimationData.delay, true);
        timer.Stoped += Animate;

        Transform tilesParent;
        int tileCount;
        
        void Animate()
        {
            if (j < maxLayerId)
            {
                tilesParent = tilesLayerParents[j];
                tileCount = tilesParent.childCount;
                
                if (i < tileCount)
                {
                    var child = tilesParent.GetChild(i).transform;
                    
                    if (unblockedTiles.Contains(child) == false)
                    {
                        child.gameObject.SetActive(true);
                        var childEndPos = child.position;
                        child.position = new Vector3(childEndPos.x, newStartPosY);
                        child.DOMove(childEndPos, tileAnimationData.duration).SetId(child);
                    }

                    i++;
                    timer.Restart();
                    return;
                }

                j++;
                i = 0;
                timer.Restart();
            }
            else
            {
                ShuffleAnimationEnded?.Invoke();
                GameTimer.Start();
            }
        }
    }

    private Vector2 GetLayerSize(in GridSize gridSize)
    {
        return new Vector2(gridSize.x * tileSpacing.x, gridSize.y * tileSpacing.y);
    }
    
    private bool TryGetTileByPosition(in Vector3 position, out Tile tile)
    {
        var mouseX = position.x;
        var mouseY = position.y;

        for (var z = (sbyte)(bounds.Length - 1); z >= 0; z--)
        {
            var bounds = this.bounds[z];
            var minX = bounds.min.x;
            var maxX = bounds.max.x;
            var minY = bounds.min.y;
            var maxY = bounds.max.y;
                
            if (mouseX < minX || mouseX > maxX || mouseY < minY || mouseY > maxY)
            {
                continue;
            }
                    
            var width = Mathf.Abs(mouseX - minX);
            var height = Mathf.Abs(mouseY - maxY);

            var x = (sbyte) (width / tileSpacing.x);
            var y = (sbyte) (height / tileSpacing.y);
            var tileKey = new TileLayerId(x, y, z).ToKey();

            if (config.tiles.TryGetValue(tileKey, out tile))
            {
                return true;
            }
        }

        tile = null;
        return false;
    }

    public void TouchTile(in Tile tile)
    {
        SoundManager.SoundMixer.PlayOneShot(6, 0.35f);
        
        if (tile.IsBlocked)
        {
            return;
        }
                        
        if (tile.IsSelected)
        {
            tile.Deselect();
            selectedTile = null;
        }
        else
        {
            tile.Select();

            if (selectedTile != null)
            {
                if (Match(selectedTile, tile) == false)
                {
                    tile.Select();
                    selectedTile.Deselect();
                    selectedTile = tile;
                }
                else
                {
                    selectedTile = null;  
                    OnMatchingEnded();
                }
            }
            else
            {
                selectedTile = tile;
            }
        }
    }

    private void MatchTwoSameTiles()
    {
        if (TryGetTwoSameRandomTiles(out var tiles))
        {
            tiles.Item1.Hint();
            tiles.Item2.Hint();
            Match(tiles.Item1, tiles.Item2);
        }
    }

    public void UpdateTilesView()
    {
        foreach (var tile in config.tiles.Values)
        {
            tile.view.SetBlockState(tile.IsBlocked);
        }
    }

    public void Hint()
    {
        if (PlayerProfile.Config.Wallet.TryBuy(CurrencyType.Coins, hintPrice))
        {
            if (TryGetTwoSameRandomTiles(out var tiles))
            {
                tiles.Item1.Hint();
                tiles.Item2.Hint();
            }
        }
    }
    
    public void Bomb()
    {
        if (PlayerProfile.Config.Wallet.TryBuy(CurrencyType.Coins, bombPrice))
        {
            var tiles = new List<(Tile, Tile)>();
        
            for (int i = 0; i < 3; i++)
            {
                if (TryGetTwoSameRandomTiles(out var tilePair))
                {
                    tilePair.Item1.Remove(config);
                    tilePair.Item2.Remove(config);
                    tiles.Add(tilePair);
                    Matched?.Invoke();
                    continue;
                }

                break;
            }
        
            var tileCount = tiles.Count;

            if (tileCount > 0)
            {
                var i = 0;

                var timer = Timer.CreateCountDown(0.5f, true);
                timer.Stoped += Animate;
            
                Animate();

                void Animate()
                {
                    if (i < tileCount)
                    {
                        var (tile1, tile2) = tiles[i];
                        
                        tile1.view.StartDestroyAnimation();
                        tile2.view.StartDestroyAnimation();
                        
                        timer.Restart();
                        i++;
                    }
                    else
                    {
                        OnMatchingEnded();
                        timer.Stop();
                    }
                }
            }
        }
    }
    
    public void Shuffle()
    {
        if (PlayerProfile.Config.Wallet.TryBuy(CurrencyType.Coins, shufflePrice))
        {
            foreach (var tile in config.tiles.Values)
            {
                tile.Destroy();
            }

            GameTimer.Resume();
            Shuffle(tilePrefab.bounds.size);
            InitAllUnblockedTiles();
            StartBuildAnimation();
        }
    }
    
    public void Undo()
    {
        undo();
        WasUndo?.Invoke();
    }

    public bool TryGetTwoSameRandomTiles(out (Tile, Tile) tiles)
    {
        var unblockedTilesCount = unblockedTiles.Count;
        var randomIndex = Random.Range(0, unblockedTilesCount);

        for (int j = 0; j < unblockedTilesCount; j++)
        {
            randomIndex %= unblockedTilesCount;

            if (TryGetSameTile(unblockedTiles[randomIndex], out var sameTile))
            {
                tiles = (unblockedTiles[randomIndex], sameTile);
                return true;
            }

            randomIndex++;
        }
        
        tiles = (null, null);
        return false;
    }

    public bool TryGetSameTile(Tile tile, out Tile sameTile)
    {
        var unblockedTilesCount = unblockedTiles.Count;
        var tileKey = tile.LayerId.ToKey();
        var id = tile.Id;

        for (int i = 0; i < unblockedTilesCount; i++)
        {
            i %= unblockedTilesCount;
            sameTile = unblockedTiles[i];

            if (sameTile.Id == id && tileKey != sameTile.LayerId.ToKey())
            {
                return true;
            }
        }

        sameTile = null;
        return false;
    }

    private void Build()
    {
        var maxLayerId = config.maxLayerId;
        bounds = new Bounds[maxLayerId];
        tilesLayerParents = new Transform[maxLayerId];
        layersParent = new GameObject("Tiles").transform;
        var tileSize = tilePrefab.bounds.size;
        var layerOffset = tileSize - (Vector3) (tileSize * new Vector2(tileWidthPercent, tileHeightPercent));

        for (sbyte i = 0; i < maxLayerId; i++)
        {
            var layerCollider = new Bounds
            {
                center = cam.transform.position - layerOffset * i,
                size = GetLayerSize(config.gridSizes[i])
            };

            bounds[i] = layerCollider;
            var layerParent = new GameObject($"Tile Layer {i}").transform;
            layerParent.parent = layersParent;
            tilesLayerParents[i] = layerParent;
        }

        Shuffle(tileSize);
    }

    private void Shuffle(in Vector3 tileSize)
    {
        var tempConfig = config.Copy();
        var tempTiles = tempConfig.tiles;
        unblockedTiles.Clear();

        foreach (var tile in tempTiles)
        {
            var value = tile.Value;
            value.StartSettingId();

            if (value.IsBlocked == false)
            {
                unblockedTiles.Add(value);
            }
            else
            {
                value.IsBlockedChanged += AddUnblockedTile;
            }

            void AddUnblockedTile(bool isBlocked)
            {
                if (isBlocked == false)
                {
                    unblockedTiles.Add(value);
                    value.IsBlockedChanged -= AddUnblockedTile;
                }
            }
        }

        var layerSize = GetLayerSize(config.gridSizes[0]);
        float aspect = (float) Screen.width / Screen.height;
        float newCamSize;

        if (layerSize.x / Screen.width > layerSize.y / Screen.height)
        {
            newCamSize = (layerSize.x / 2  + tileSize.x) / aspect;
        }
        else
        {
            newCamSize = layerSize.y / 2  + tileSize.x;
        }

        var ratio = newCamSize / cam.orthographicSize;
        cam.orthographicSize = newCamSize;
        CameraSizeChanged?.Invoke(ratio);

        var id = Random.Range(0, sprites.Length);
        var spriteCount = sprites.Length;
        var ids = new HashSet<sbyte>();
        
        while (unblockedTiles.Count > 0)
        {
            id++;
            id %= spriteCount;
            Tile prevTile = null;
            
            for (int j = 0; j < 2; j++)
            {
                var randomIndex = Random.Range(0, unblockedTiles.Count);
                var tile = unblockedTiles[randomIndex];
                tile.Remove(tempConfig);
                unblockedTiles.RemoveAt(randomIndex);
                tile.Id = (sbyte) id;

                var copyTileRef = tile;
                copyTileRef.GetBlockers(ids, config);

                while (ids.Contains(copyTileRef.Id))
                {
                    id++;
                    id %= spriteCount;
                    copyTileRef.Id = (sbyte) id;

                    if (prevTile != null)
                    {
                        (copyTileRef, prevTile) = (prevTile, copyTileRef);
                        copyTileRef.Id = (sbyte) id;
                        copyTileRef.view.SetSprite(sprites[id]);
                        copyTileRef.GetBlockers(ids, config);
                    }
                }
                
                var layerId = tile.LayerId;
                var tileSprite = Instantiate(tilePrefab, tilesLayerParents[layerId.z]);
                tileSprite.name = $"Tile {id} {layerId.ToKey()}";
                tileSprite.sprite = sprites[id];
                var sortingOrder = layerId.x - config.gridSizes[0].x * layerId.y + layerId.z * 3;
                tileSprite.sortingOrder = sortingOrder;
                var child = tileSprite.transform.GetChild(0).GetComponent<SpriteRenderer>();
                child.sortingOrder = sortingOrder - 2;

                var sprite = tilePrefab.sprite;
                var rect = sprite.rect;
                var pivot = sprite.pivot;
                var size = sprite.bounds.size;
                var xOffset = size.x * Mathf.InverseLerp(0, rect.width, pivot.x);
                var yOffset = size.y * Mathf.InverseLerp(0, rect.height, pivot.y);
                var bound = bounds[layerId.z];
                var startPoint = new Vector3(bound.min.x, bound.max.y);
                var position = startPoint + new Vector3(layerId.x * tileSpacing.x + xOffset, layerId.y * tileSpacing.y * -1 - yOffset);
                tileSprite.transform.position = position;
                tile.view = new TileView(tileSprite, destroyFx);
                
                prevTile = tile;
            }
        }
    }

    private void OnTimeOut()
    {
        Debug.Log("OnTimeOut");
        config.tiles.Clear();
        Lose();
    }

    private void CheckGameResult()
    {
        if (config.tiles.Count == 0)
        {
            GameTimer.Pause();
            Win();
        }
        else if(TryGetTwoSameRandomTiles(out _) == false)
        {
            GameTimer.Pause();
            Lose();
        }
    }

    private void Lose()
    {
        var coinsCount = PlayerProfile.Config.Wallet.GetCurrency(CurrencyType.Coins).Value;
        
        if (CanShuffle && (coinsCount > ShufflePrice || Advertiser.IsRewardReady))
        {
            Timer.CreateCountDown(2, true).Stoped += OutOfMoves;
        }
        else
        {
            OnGameEnd();
            Timer.CreateCountDown(2, true).Stoped += Lost;
        }
    }

    private void Win()
    {
        OnGameEnd();
        Timer.CreateCountDown(2, true).Stoped += Won;
    }

    private void OnGameEnd()
    {
        TimerExtensions.Kill(this);
        GameWindow.SetActiveHints(false);
    }

    private bool Match(Tile src, Tile dst)
    {
        var srcTileId = src.Id;
        var dstTileId = dst.Id;

        var isMatch = srcTileId == dstTileId;

        if (isMatch)
        {
            src.Remove(config);
            dst.Remove(config);
            src.view.StartDestroyAnimation();
            dst.view.StartDestroyAnimation();
            Matched?.Invoke();
        }
        
        return isMatch;
    }
    
    private void OnMatchingEnded()
    {
        if (config.tiles.Count >= 10)
        {
            bonusGame.TryStart();
        }
    }

    public void Finish()
    {
        if (isStarted)
        {
            Debug.Log("Finish");
            OnGameEnd();

            for (int i = 0; i < tilesLayerParents.Length; i++)
            {
                var layerParent = tilesLayerParents[i];
                
                for (int j = 0; j < layerParent.childCount; j++)
                {
                    DOTween.Kill(layerParent.GetChild(j));
                }
            }
            
            for (int i = 0; i < allTiles.Length; i++)
            {
                allTiles[i].Destroy();
                allTiles[i] = null;
            }
            
            Destroy(layersParent.gameObject);
            Destroy(gameObject);
            bonusGame.Destroy();
            
            config.tiles.Clear();
            unblockedTiles.Clear();
            allTiles = null;

            isStarted = false;
        }
    }
}
