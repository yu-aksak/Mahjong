using System;
using Core.CurrencyModule;
using Newtonsoft.Json;
using UnityEngine;

namespace Core.PlayerModule
{
    [Serializable]
    public class Wallet
    {
        public event Action<int> Earned;
        public event Action<bool> Purchased;
        
        [JsonProperty] private readonly CurrencyDictionary currencies = new CurrencyDictionary();

        public Currency GetCurrency(CurrencyType type)
        {
            return currencies[type];
        }

        public void Earn(CurrencyType type, in int price)
        {
            currencies[type].Value += price;
            Earned?.Invoke(price);
        }
    
        public bool TryBuy(CurrencyType type, in int price)
        {
            var currency = currencies[type];

            if (currency.Value >= price)
            {
                currency.Value -= price;
                Purchased?.Invoke(true);
                return true;
            }

            Purchased?.Invoke(false);
            return false;
        }
    }
}
