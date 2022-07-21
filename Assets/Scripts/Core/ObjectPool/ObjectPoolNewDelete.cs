using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.ObjectPool
{
    public class ObjectPoolNewDelete<T> : BaseObjectPool<T> where T : MonoBehaviour
    {
        protected override int Length => Count;

        protected override void OnAdd(int index)
        {
            Create(index);
        }

        protected override void OnRemove(int index)
        {
            index = Count - 1;
            RemoveAt(index);
            Object.Destroy(base[index].gameObject);
        }

        public ObjectPoolNewDelete(Func<T> create) : base(create) { }
    }
}