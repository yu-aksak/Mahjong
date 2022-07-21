using System;
using UnityEngine;

namespace Core.ObjectPool
{
    public class ObjectPoolEnableDisable<T> : BaseObjectPool<T> where T : Component
    {
        private int count;
        protected override int Length => count;

        protected override void OnAdd(int index)
        {
            base[index].gameObject.SetActive(true);
            count++;
        }

        protected override void OnRemove(int index)
        {
            base[index].gameObject.SetActive(false);
            count--;
        }

        public ObjectPoolEnableDisable(Func<T> create) : base(create) { }
    }
}
