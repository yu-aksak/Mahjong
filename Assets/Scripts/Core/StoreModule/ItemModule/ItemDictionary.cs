using System;
using System.Collections.Generic;

namespace Core.StoreModule
{
    [Serializable]
    public class ItemDictionary<T> : Dictionary<ItemType, T> where T : new()
    {
        public ItemDictionary()
        {
            Add(ItemType.Background, new T());
            Add(ItemType.Mahjong, new T());
        }

        public new void Clear()
        {
            base[ItemType.Background] = new T();
            base[ItemType.Mahjong] = new T();
        }
    }
}
