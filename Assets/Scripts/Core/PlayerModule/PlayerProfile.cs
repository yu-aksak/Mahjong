using System;
using Core.ConfigModule;
using Core.StoreModule;

namespace Core.PlayerModule
{
    [Serializable]
    public class PlayerProfile : JsonConfigData<PlayerProfile>, ISelectedItemsProvider
    {
        public Wallet Wallet { get; } = new Wallet();
        public Inventory Inventory { get; } = new Inventory();
        public ItemDictionary<int> SelectedItems { get; } = new ItemDictionary<int>();
        protected override ConfigName ConfigName { get; } = new ConfigName("profile");

        public PlayerProfile()
        {
            ItemSelector.SetSelectedItems(this);
        }
    }
}
