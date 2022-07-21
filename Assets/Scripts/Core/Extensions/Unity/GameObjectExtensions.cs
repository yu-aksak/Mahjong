using System.Collections.Generic;
using UnityEngine;

namespace Core.Extensions.Unity
{
    public static class GameObjectExtensions
    {
        public static T Get<T>(this GameObject component, string path) where T : Object
        {
            return component.transform.Get<T>(path);
        }
    
        public static GameObject GetGameObject(this GameObject component, string path)
        {
            return component.transform.GetGameObject(path);
        }
    
        public static void GetAllChild(this Transform transform, List<Transform> childs)
        {
            var childCount = transform.childCount;
        
            if (childCount == 0)
            {
                return;
            }
        
            for (int i = 0; i < childCount; i++)
            {
                var child = transform.GetChild(i);
                childs.Add(child);
                child.GetAllChild(childs);
            }
        }
    }
}