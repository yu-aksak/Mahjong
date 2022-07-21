namespace Core.StoreModule
{
    public interface ISelectedItemsProvider
    {
        public ItemDictionary<int> SelectedItems { get; }
    }
}