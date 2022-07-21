using System;
using System.Collections.Generic;
using Core.StoreModule;
using Newtonsoft.Json;

namespace Core.PlayerModule
{
    [Serializable]
    public class Inventory
    {
        [JsonProperty] private ItemDictionary<HashSet<int>> items = new ItemDictionary<HashSet<int>>();

        public void PutItem(ItemType type, int itemResourceId)
        {
            items[type].Add(itemResourceId);
        }

        public bool HasItem(ItemType type, int itemResourceId)
        {
            return  items[type].Contains(itemResourceId);
        }
    }
}