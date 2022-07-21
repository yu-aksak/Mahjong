using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.SingleService
{
    public class ServiceManager : MonoBehaviour
    {
        public static event Action ApplicationPaused;
        [SerializeField] private List<BaseSingleService> services;
        private static readonly Dictionary<Type, BaseSingleService> Services = new Dictionary<Type, BaseSingleService>();

        private void Awake()
        {
            DontDestroyOnLoad(this);
            Debug.Log("[ServiceManager] Awake");

            for (int i = 0; i < services.Count; i++)
            {
                var window = services[i];

                Services.Add(window.Type, window);
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                ApplicationPaused?.Invoke();
            }
        }

        private void OnApplicationQuit()
        {
            OnApplicationPause(true);
        }

        public static T GetService<T>() where T : BaseSingleService
        {
            return (T)Services[typeof(T)];
        }
    }
}
