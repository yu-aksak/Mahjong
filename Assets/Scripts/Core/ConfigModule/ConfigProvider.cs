using System;
using System.IO;
using Core.SingleService;
using Newtonsoft.Json;
using UnityEngine;

namespace Core.ConfigModule
{
    [Serializable]
    public abstract class ConfigProvider<T> where T : ConfigProvider<T>, new()
    {
        private static Func<T> getter;
        protected static Action<T> onGetter;
        protected static T instance;
        protected static ConfigProvider<T> provider;

        static ConfigProvider()
        {
            ServiceManager.ApplicationPaused += Save;
            provider = new T();
            Load();
            onGetter?.Invoke(instance);
        }

        protected abstract string FullFileName { get; }

        protected abstract string FullPath { get; }

        protected abstract string Ext { get; }
        protected abstract ConfigName ConfigName { get; }

        public static void UpdateName(string newName)
        {
            if (provider.ConfigName.Name.Equals(newName) == false)
            {
                getter = Load;
                provider.ConfigName.Name = newName;
            }
        }
    
        protected static T Load()
        {
            var fullFileName = provider.FullFileName;
        
            if (File.Exists(fullFileName))
            {
                var json = File.ReadAllText(fullFileName);
                instance = JsonConvert.DeserializeObject<T>(json);
                provider = instance;
                getter = GetInstance;
            
                return instance;
            }

            instance = new T
            {
                ConfigName =
                {
                    Name = provider.ConfigName.Name
                }
            };
        
            provider = instance;
            getter = GetInstance;

            return instance;
        }

        private static T GetInstance()
        {
            return instance;
        }

        public static T Config
        {
            get
            {
#if UNITY_EDITOR
                if (Application.isPlaying == false)
                {
                    return Load();
                }
#endif
                return getter();
            }
        }

        public static void Save()
        {
            Set(instance);
        }
    
        public static void Set(T config)
        {
            if (Directory.Exists(config.FullPath) == false)
            {
                Directory.CreateDirectory(config.FullPath);
            }
#if UNITY_EDITOR
            var json = JsonConvert.SerializeObject(config, Formatting.Indented);
#else
        var json = JsonConvert.SerializeObject(config);
#endif
            File.WriteAllText(provider.FullFileName,json);
        }
    }
}