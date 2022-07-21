using Core.PlayerModule;
using Core.StoreModule;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class StoreWindow : Window<StoreWindow>
{
    [SerializeField] private Color selectedColor;
    [SerializeField] private Transform backgroundItemsParent;
    [SerializeField] private Transform mahjongItemsParent;
    [SerializeField] private Button slotPrefab;
    [SerializeField] private Button settings;
    [SerializeField] private Canvas backgroundPrefab;
    private Canvas background;
    public static Color SelectedColor => Instance.selectedColor;

    protected override void Init()
    {
        needBindSounsAfterInit = true;
        base.Init();
        
        TileMap.CameraSizeChanged += OnCameraSizeChanged;
        background = Instantiate(backgroundPrefab);
        settings.onClick.AddListener(SettingsWindow.Show);
        
        ItemSelector.AddListener(ItemType.Background, OnSelect);

        var storeConfig = Store.Config;
        var profile = PlayerProfile.Config;

        InitStore<BackgroundSlotView, Sprite>(backgroundItemsParent, ItemType.Background);
        InitStore<MahjongSlotView, TextAsset>(mahjongItemsParent, ItemType.Mahjong);

        void InitStore<T, T1>(Transform parent, ItemType type) where T : SlotView<T>, new() where T1 : Object
        {
            foreach (var item in storeConfig.GetItems(type))
            {
                var resourceData = StoreResourceProvider.GetResourceData<T1>(item.ResourceId);
                var buyButtom = SlotView<T>.Create(slotPrefab, parent, item, resourceData.preview, out var slotView);
                buyButtom.onClick.AddListener(Buy);

                if (item.currency.Value == 0 && !profile.Inventory.HasItem(type, item.ResourceId))
                {
                    profile.Inventory.PutItem(type, item.ResourceId);
                }
                
                if (profile.Inventory.HasItem(type, item.ResourceId))
                {
                    slotView.OnBuy();
                    buyButtom.onClick.AddListener(slotView.OnClick);
                }

                if (ItemSelector.GetSelected(type) == item.Index)
                {
                    slotView.OnSelect();
                }

                void Buy()
                {
                    if (Store.TryBuy(type, item, profile.Wallet, profile.Inventory))
                    {
                        slotView.OnBuy();
                        slotView.OnClick();
                        
                        Store.Set(storeConfig);
                        PlayerProfile.Set(profile);
                        buyButtom.onClick.RemoveListener(Buy);
                        buyButtom.onClick.AddListener(slotView.OnClick);
                    }
                }
            }
        }
        
        BindButtonsSound(); 
        BindTogglesSound();
    }
    
    protected override void OnShow()
    {
        base.OnShow();
        StatsWindow.Show();
        background.transform.GetChild(0).GetComponent<Image>().sprite = StoreResourceProvider.Get<Sprite>(ItemSelector.GetSelected(ItemType.Background));
    }

    protected override void OnHided()
    {
        base.OnHided();
        StatsWindow.Hide();
    }

    private void OnSelect(in int resourceId)
    {
        background.transform.GetChild(0).GetComponent<Image>().sprite = StoreResourceProvider.Get<Sprite>(resourceId);
    }
    
    private void OnCameraSizeChanged(float ratio)
    {
        background.transform.localScale *= ratio;
    }
}