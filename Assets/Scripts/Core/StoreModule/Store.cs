using System;
using System.Collections.Generic;
using Core.ConfigModule;
using Core.PlayerModule;
using Newtonsoft.Json;

namespace Core.StoreModule
{
    [Serializable]
    public class Store : JsonConfigData<Store>
    {
        [Serializable]
        public class Items : List<Item>
        {
            public Items()
            {
                Item.id = 0;
            }
        }

        [JsonProperty] private readonly ItemDictionary<Items> items = new ItemDictionary<Items>();
    
        protected override ConfigName ConfigName { get; } = new ConfigName("store");

        static Store()
        {
            onGetter = OnGet;
        }

        private static void OnGet(Store store)
        {
            if (store.items[ItemType.Mahjong].Count == 0)
            {
                StoreResources.UpdateStoreConfig();
            }
        }

        public Items GetItems(ItemType type)
        {
            return items[type];
        }
    
        public void AddItem(ItemType itemType, Item item)
        {
            items[itemType].Add(item);
        }

        public void RemoveItem(ItemType itemType, int index)
        {
            // TODO
        }

        public Item GetItem(ItemType itemType, int index)
        {
            return items[itemType][index];
        }

        public static bool TryBuy(in ItemType type, Item item, Wallet wallet, Inventory inventory)
        {
            var currency = instance.GetItem(type, item.Index).currency;

            if (inventory.HasItem(type, item.ResourceId))
            {
                return false;
            }

            if (wallet.TryBuy(currency.Type, currency.Value))
            {
                inventory.PutItem(type, item.ResourceId);

                return true;
            }

            return false;
        }

        public void Clear()
        {
            items.Clear();
        }
    }
}
