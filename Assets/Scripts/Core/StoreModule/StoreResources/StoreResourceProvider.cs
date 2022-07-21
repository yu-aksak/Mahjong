using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.StoreModule
{
    public static class StoreResourceProvider
    {
        private static StoreResources storeResources;

        static StoreResourceProvider()
        {
            storeResources = Resources.Load<StoreResources>("ScriptableObjects/StoreResources/StoreResources");
            storeResources.Init();
        }
    
        public static T Get<T>(int resourceId) where T : Object
        {
            return storeResources.Get<T>(resourceId);
        }
    
        public static StoreResourceData<T> GetResourceData<T>(int resourceId) where T : Object
        {
            return storeResources.GetResourceData<T>(resourceId);
        }

        public static StoreResources GetStoreResourcesConfig()
        {
            return storeResources;
        }
    }
}
