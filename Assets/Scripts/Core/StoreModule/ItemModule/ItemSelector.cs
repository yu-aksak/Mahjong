namespace Core.StoreModule
{
    public static class ItemSelector
    {
        private class ItemSelectingListener
        {
            public InAction<int> selected;
        }
    
        private static readonly ItemDictionary<ItemSelectingListener> selectingListeners;
        private static ItemDictionary<int> selectedItems;
        public static ItemDictionary<int> SelectedItems => selectedItems;

        static ItemSelector()
        {
            selectingListeners = new ItemDictionary<ItemSelectingListener>();
        }

        public static void SetSelectedItems(ISelectedItemsProvider provider)
        {
            selectedItems = provider.SelectedItems;
        }

        public static void AddListener(in ItemType type, InAction<int> action)
        {
            selectingListeners[type].selected += action;
        }
    
        public static void RemoveListener(in ItemType type, InAction<int> action)
        {
            selectingListeners[type].selected -= action;
        }
    
        public static void RemoveAllListener(in ItemType type)
        {
            selectingListeners[type].selected = null;
        }

        public static void Select(in ItemType type, in int itemIndex)
        {
            selectedItems[type] = itemIndex;
            selectingListeners[type].selected?.Invoke(itemIndex);
        }

        public static int GetSelected(in ItemType type)
        {
            return selectedItems[type];
        }
    }
}
