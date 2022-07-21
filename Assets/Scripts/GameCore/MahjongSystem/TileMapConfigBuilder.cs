using System.Collections.Generic;
using System.IO;
using Core.ConfigModule;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using File = System.IO.File;

public class TileMapConfigBuilder : MonoBehaviour
{
    [SerializeField] private string fileName;
    [SerializeField] private string fileExt;
    [SerializeField] private Canvas layersParent;
    [SerializeField] private Slider layerSelectorSlider;
    [SerializeField] private TextMeshProUGUI tileCountText;
    [SerializeField] private GridLayoutGroup togglesLayerGridTemplate;
    [SerializeField] private Button addLayerButton;
    [SerializeField] private Button serializeButton;
    [SerializeField] private Toggle toggleTemplate;
    [SerializeField] private List<GridSize> gridSizes;
    [SerializeField] private Vector2 cellSize;

    private List<GridLayoutGroup> layers = new List<GridLayoutGroup>();
    private Dictionary<GameObject, TileLayerId> ids = new Dictionary<GameObject, TileLayerId>();
    private Dictionary<short, Tile> tiles = new Dictionary<short, Tile>();
    private TileMapConfig config;
    private string fullFileName;
    
    private void Awake()
    {
        togglesLayerGridTemplate.gameObject.SetActive(false);
        toggleTemplate.gameObject.SetActive(false);
        
        addLayerButton.onClick.AddListener(AddLayer);
        serializeButton.onClick.AddListener(Serialize);
        layerSelectorSlider.onValueChanged.AddListener(SelectLayer);

        config = new TileMapConfig(tiles, gridSizes);
        AddLayer();
    }
    
    private void AddLayer()
    {
        BuildGrid();
        config.maxLayerId++;
    }
    
    private void Serialize()
    {
        config.SortTiles();

        var json = JsonConvert.SerializeObject(config, Formatting.Indented);
        fullFileName = Path.Combine(Application.dataPath, FolderName.Configs, "MahjongLevels", $"{fileName}.{fileExt}");
        File.WriteAllText(fullFileName, json);
    }

    private void SelectLayer(float layerId)
    {
        var layerIdInt = (int)layerId;
        var layerCount = layers.Count;

        for (int i = 0; i <= layerIdInt; i++)
        {
            layers[i].gameObject.SetActive(true);
        }
        
        for (int i = layerIdInt + 1; i < layerCount; i++)
        {
            layers[i].gameObject.SetActive(false);
        }
    }

    private void BuildGrid()
    {
        var layerGrid = Instantiate(togglesLayerGridTemplate, layersParent.transform);
        var layerGridRectTransform = layerGrid.GetComponent<RectTransform>();
        var currentLayerId = config.maxLayerId;
        var xSize = gridSizes[currentLayerId].x;
        var ySize = gridSizes[currentLayerId].y;
        
        layerGridRectTransform.sizeDelta = new Vector2(xSize * cellSize.x, ySize * cellSize.y);
        layerGrid.cellSize = cellSize;

        for (sbyte y = 0; y < ySize; y++)
        {
            for (sbyte x = 0; x < xSize; x++)
            {
                var toggle = Instantiate(toggleTemplate, layerGridRectTransform);
                var toggleGameObject = toggle.gameObject;
                var id = new TileLayerId(x, y, currentLayerId);

                toggle.isOn = false;
                toggle.onValueChanged.AddListener(OnToggleSwitch);
                var texts = toggle.GetComponentsInChildren<TextMeshProUGUI>();
                texts[0].text = $"{id.x}\n{id.y}\n{id.z}";
                texts[1].text = "0";
                toggleGameObject.SetActive(true);
                ids.Add(toggleGameObject, id);
            }
        }
        
        layerGrid.gameObject.SetActive(true);
        layers.Add(layerGrid);
        layerSelectorSlider.maxValue = currentLayerId;
    }

    private void OnToggleSwitch(bool active)
    {
        var selectedGameObject = EventSystem.current.currentSelectedGameObject;
        var id = ids[selectedGameObject]; 
        var texts = selectedGameObject.GetComponentsInChildren<TextMeshProUGUI>();
        
        if (active)
        {
            var tile = new Tile(id);
            
            tile.BlockerCountChanged += blockerCount =>
            {
                texts[1].text = blockerCount.ToString();
            };
            
            AddTile(tile);
        }
        else
        {
            RemoveTile(id);
        }

        var tileCount = tiles.Count;
        var isEven = tileCount % 2 == 0;
        
        tileCountText.text = $"Tile Count: {tileCount}";
        serializeButton.targetGraphic.color = isEven ? Color.green : Color.red;
        serializeButton.interactable = isEven;
    }

    private void AddTile(Tile tile)
    {
        tile.Add(config);
    }

    private void RemoveTile(TileLayerId id)
    {
        var tileId = id.ToKey();
        var tile = tiles[tileId];
        tile.Remove(config);
        tile.BlockerCount = 0;
    }
}
