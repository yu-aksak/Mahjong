using System;
using System.Collections.Generic;

namespace Core.CurrencyModule
{
    [Serializable]
    public class CurrencyDictionary : Dictionary<CurrencyType, Currency>
    {
        public CurrencyDictionary()
        {
            Add(CurrencyType.Coins, new Currency(CurrencyType.Coins));
            Add(CurrencyType.Crystal, new Currency(CurrencyType.Crystal));
        }
    }
}