using System;
using System.Collections.Generic;

namespace Core.StoreModule
{
    [Serializable]
    public abstract class StoreResourceDataCollection<T> : BaseStoreResourceDataCollection where T : BaseStoreResourceData
    {
        public List<T> resources = new List<T>();
    }
}