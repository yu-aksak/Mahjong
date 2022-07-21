using UnityEditor;

namespace Core.StoreModule.Editor
{
    public static class EditorStoreResources
    {
        [MenuItem("StoreResources/UpdateStoreConfig")]
        public static void UpdateStoreConfig()
        {
            StoreResources.UpdateStoreConfig();
        }
    }
}
