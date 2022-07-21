using System;
using Object = UnityEngine.Object;

namespace Core.StoreModule
{
    [Serializable]
    public class StoreResourceData<T> : BaseStoreResourceData where T : Object
    {
        public T asset;
    }
}