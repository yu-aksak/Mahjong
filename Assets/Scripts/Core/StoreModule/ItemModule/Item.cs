using System;
using Core.CurrencyModule;
using Newtonsoft.Json;

namespace Core.StoreModule
{
    [Serializable]
    public class Item
    {
        public static int id;
        [JsonIgnore] public int Index { get; set; }
        public int ResourceId { get; set; }
        public string name;
        public Currency currency;

        public Item(int resourceId, string name, Currency currency)
        {
            Index = id;
            ResourceId = resourceId;
            this.name = name;
            this.currency = currency;
            id++;
        }
    }
}