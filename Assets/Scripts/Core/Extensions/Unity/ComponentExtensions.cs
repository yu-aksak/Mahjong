using UnityEngine;

namespace Core.Extensions.Unity
{
    public static class ComponentExtensions
    {
        public static GameObject GetGameObject(this Component component, string path)
        {
            var gameObjectNames = path.Split('/');
            var transform = component.transform;

            for (int i = 0; i < gameObjectNames.Length; i++)
            {
                for (int j = 0; j < transform.childCount; j++)
                {
                    var child = transform.GetChild(j);
                
                    if (child.name.Equals(gameObjectNames[i]))
                    {
                        transform = child;
                        break;
                    }
                }
            }

            return transform.gameObject;
        }
    
        public static T Get<T>(this Component component, string path) where T : Object
        {
            return component.GetGameObject(path).GetComponent<T>();
        }
    
        public static bool TryGet<T>(this Component component, string path, out T gameComponent) where T : Object
        {
            gameComponent = component.Get<T>(path);
            
            return gameComponent != null;
        }
    
    }
}
