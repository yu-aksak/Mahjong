using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.StoreModule
{
    [CreateAssetMenu(fileName = "StoreResources", menuName = "Store Resources", order = 0)]
    public class StoreResources : ScriptableObject
    {
        public TextAssetResourceDataCollection textAssetResourceDataCollection;
        public SpriteResourceDataCollection spriteResourceDataCollection;

        private readonly Dictionary<Type, BaseStoreResourceDataCollection> storeResourceDataCollection = new Dictionary<Type, BaseStoreResourceDataCollection>();

        public void Init()
        {
            storeResourceDataCollection.Add(typeof(TextAsset), textAssetResourceDataCollection);
            storeResourceDataCollection.Add(typeof(Sprite), spriteResourceDataCollection);
        }

        public T Get<T>(int resourceId) where T : Object
        {
            return GetResourceData<T>(resourceId).asset;
        }
    
        public StoreResourceData<T> GetResourceData<T>(int resourceId) where T : Object
        {
            if (storeResourceDataCollection.TryGetValue(typeof(T), out var storeCollection))
            {
                if (storeCollection is StoreResourceDataCollection<StoreResourceData<T>> collection)
                {
                    return collection.resources[resourceId];
                }

                throw new Exception($"Incompatible types of {storeCollection.GetType()} and {typeof(StoreResourceDataCollection<StoreResourceData<T>>)}");
            }
        
            throw new Exception($"Resource with the same id: {resourceId} is not found");
        }
    
        public static void UpdateStoreConfig()
        {
            var storeResources = StoreResourceProvider.GetStoreResourcesConfig();
            var textAssets = storeResources.textAssetResourceDataCollection;
            var sprite = storeResources.spriteResourceDataCollection;
            var config = Store.Config;
            config.Clear();
            AddItems(textAssets);
            AddItems(sprite);
        
            Store.Save();
        
            void AddItems<T>(StoreResourceDataCollection<T> storeResource) where T : BaseStoreResourceData
            {
                for (int i = 0; i < storeResource.resources.Count; i++)
                {
                    var item = storeResource.resources[i].item;
                    item.ResourceId = i;
                    item.Index = i;
                    config.AddItem(storeResource.type, item);
                }
            }
        }
    }
}
