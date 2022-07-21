using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.ObjectPool
{
    public abstract class BaseObjectPool<T> : List<T> where T : Component
    {
        public event Action<T, int> Created;
        private Func<T> create;
        protected abstract int Length { get; }
        protected abstract void OnAdd(int index);
        protected abstract void OnRemove(int index);

        protected BaseObjectPool(Func<T> create)
        {
            SetCreateMethod(create);
        }

        public void SetCreateMethod(Func<T> create)
        {
            this.create = create;
        }

        protected void Create(int index)
        {
            var item = create();
            Add(item);
            Created?.Invoke(item, index);
        }

        public T Get(out int index)
        {
            if (Length == Count)
            {
                Create(Length);
            }
        
            index = Length;
            OnAdd(Length);
            return this[index];
        }
    
        public void StopUse(int index)
        {
            OnRemove(index);
        }
    
        public void Fill(int itemCount)
        {
            int i = Count;

            if (itemCount > i)
            {
                for (; i < itemCount; i++)
                {
                    Create(i);
                }
            }

            var lenght = Length;
        
            if (itemCount > lenght)
            {
                for (i = lenght; i < itemCount; i++)
                {
                    OnAdd(i);
                }
            }
            else if (lenght > itemCount)
            {
                for (i = itemCount; i < lenght; i++)
                {
                    OnRemove(i);
                }
            }
        }
    }
}
